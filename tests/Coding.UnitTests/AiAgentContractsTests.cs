using System.Security.Cryptography;
using System.Text;
using Coding.Application.Features.AiAgent;
using Coding.Enums;
using Coding.Models;
using FluentAssertions;
using Xunit;

namespace Coding.UnitTests;

public sealed class AiAgentContractsTests
{
    [Fact]
    public void Approval_is_valid_when_status_match_hash_match_and_not_expired()
    {
        var now = DateTime.UtcNow;
        var hash = "abc";
        var policy = new TestApprovalPolicy();

        var ok = policy.IsApprovalValid(AiApprovalStatus.ApprovedOnce, hash, hash, now.AddMinutes(5), now);

        ok.Should().BeTrue();
    }

    [Fact]
    public void Approval_is_invalid_when_status_is_pending()
    {
        var now = DateTime.UtcNow;
        var policy = new TestApprovalPolicy();

        policy.IsApprovalValid(AiApprovalStatus.Pending, "h", "h", now.AddMinutes(5), now)
            .Should().BeFalse();
    }

    [Fact]
    public void Approval_is_invalid_when_status_is_rejected()
    {
        var now = DateTime.UtcNow;
        var policy = new TestApprovalPolicy();

        policy.IsApprovalValid(AiApprovalStatus.Rejected, "h", "h", now.AddMinutes(5), now)
            .Should().BeFalse();
    }

    [Fact]
    public void Approval_is_invalid_when_status_is_expired()
    {
        var now = DateTime.UtcNow;
        var policy = new TestApprovalPolicy();

        policy.IsApprovalValid(AiApprovalStatus.Expired, "h", "h", now.AddMinutes(5), now)
            .Should().BeFalse();
    }

    [Fact]
    public void Approval_is_invalid_when_already_expired_by_clock()
    {
        var now = DateTime.UtcNow;
        var policy = new TestApprovalPolicy();

        policy.IsApprovalValid(AiApprovalStatus.ApprovedOnce, "h", "h", now.AddSeconds(-1), now)
            .Should().BeFalse();
    }

    [Fact]
    public void Approval_is_invalid_when_arguments_hash_differs()
    {
        var now = DateTime.UtcNow;
        var policy = new TestApprovalPolicy();

        policy.IsApprovalValid(AiApprovalStatus.ApprovedOnce, "approved-hash", "actual-hash", now.AddMinutes(5), now)
            .Should().BeFalse();
    }

    [Fact]
    public void Redaction_service_redacts_common_secret_shapes()
    {
        var svc = new TestSecretRedactionService();

        svc.Redact("Authorization: Bearer abcdefghijklmnop")
            .Should().Contain("[REDACTED]");
        svc.Redact("AWS_SECRET_ACCESS_KEY=AKIAIOSFODNN7EXAMPLE")
            .Should().Contain("[REDACTED]");
        svc.Redact("the quick brown fox")
            .Should().NotContain("[REDACTED]");
    }

    [Theory]
    [InlineData(".env")]
    [InlineData("appsettings.Production.json")]
    [InlineData("./secrets/private.key")]
    [InlineData("id_rsa")]
    [InlineData("../credentials/credentials.json")]
    public void Redaction_service_flags_secret_files(string path)
    {
        var svc = new TestSecretRedactionService();
        svc.IsSecretFile(path).Should().BeTrue();
    }

    [Theory]
    [InlineData("Program.cs")]
    [InlineData("src/index.ts")]
    [InlineData("README.md")]
    public void Redaction_service_does_not_flag_normal_source_files(string path)
    {
        var svc = new TestSecretRedactionService();
        svc.IsSecretFile(path).Should().BeFalse();
    }

    [Fact]
    public void Idempotency_key_hashes_are_stable_for_identical_inputs()
    {
        var runId = Guid.NewGuid();
        var args = "{\"path\":\"Program.cs\"}";

        var first = ComputeIdempotencyKey(runId, "apply_patch", args);
        var second = ComputeIdempotencyKey(runId, "apply_patch", args);

        first.Should().Be(second);
    }

    [Fact]
    public void Idempotency_key_changes_when_arguments_change()
    {
        var runId = Guid.NewGuid();

        ComputeIdempotencyKey(runId, "apply_patch", "{\"path\":\"a.cs\"}")
            .Should().NotBe(ComputeIdempotencyKey(runId, "apply_patch", "{\"path\":\"b.cs\"}"));
    }

    [Fact]
    public void Idempotency_key_changes_when_run_changes()
    {
        var runA = Guid.NewGuid();
        var runB = Guid.NewGuid();

        ComputeIdempotencyKey(runA, "apply_patch", "{\"path\":\"a.cs\"}")
            .Should().NotBe(ComputeIdempotencyKey(runB, "apply_patch", "{\"path\":\"a.cs\"}"));
    }

    internal static string ComputeIdempotencyKey(Guid runId, string toolName, string argumentsJson)
    {
        var bytes = Encoding.UTF8.GetBytes($"{runId:N}|{toolName}|{argumentsJson}");
        return Convert.ToHexString(SHA256.HashData(bytes));
    }

    private sealed class TestApprovalPolicy : IAiToolApprovalPolicy
    {
        public bool RequiresApproval(AiToolDescriptor descriptor) => descriptor.RiskLevel >= AiToolRiskLevel.Medium;

        public bool CanAutoApproveLowRisk(AiAgentRun run, AiToolDescriptor descriptor) =>
            descriptor.RiskLevel == AiToolRiskLevel.Low;

        public bool IsApprovalValid(AiApprovalStatus status, string approvalHash, string callHash, DateTime expiresAt, DateTime nowUtc)
        {
            if (status is not (AiApprovalStatus.ApprovedOnce or AiApprovalStatus.ApprovedForRun))
                return false;
            if (expiresAt <= nowUtc)
                return false;
            return string.Equals(approvalHash, callHash, StringComparison.Ordinal);
        }
    }

    private sealed class TestSecretRedactionService : IAiSecretRedactionService
    {
        private static readonly (string Pattern, string Replacement)[] Patterns =
        {
            ("Bearer [A-Za-z0-9\\-_]{8,}", "[REDACTED]"),
            ("AKIA[0-9A-Z]{8,}", "[REDACTED]"),
            ("AWS_SECRET_ACCESS_KEY=[A-Za-z0-9/+=]{8,}", "AWS_SECRET_ACCESS_KEY=[REDACTED]"),
            ("Authorization: [^\\s]+", "Authorization: [REDACTED]")
        };

        private static readonly string[] SecretFileNames =
        {
            ".env", "id_rsa", "credentials.json", "secrets.json", "appsettings.Production.json",
            "private.key", "credentials", ".npmrc", "secrets"
        };

        private static readonly string[] SecretFileNameSuffixes =
        {
            ".key", ".pem", ".pfx"
        };

        public string Redact(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            foreach (var (pattern, replacement) in Patterns)
            {
                input = System.Text.RegularExpressions.Regex.Replace(
                    input, pattern, replacement, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            }
            return input;
        }

        public bool IsSecretFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath)) return false;
            var name = Path.GetFileName(filePath).ToLowerInvariant();
            if (SecretFileNames.Any(s => name == s.ToLowerInvariant())) return true;
            return SecretFileNameSuffixes.Any(suffix => name.EndsWith(suffix, StringComparison.OrdinalIgnoreCase));
        }
    }
}