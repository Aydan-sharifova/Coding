using Coding.Application.Features.AiAgent;
using Coding.Enums;
using Coding.Models;

namespace Coding.Infrastructure.AiAgent;

/// <summary>
/// Centralized risk policy for AI tool calls. All risk decisions live here so
/// callers and controllers do not invent their own thresholds.
/// </summary>
public static class AiRiskPolicy
{
    /// <summary>
    /// Classify a tool based on its name and the calling run. Critical tools
    /// are blocked in this version: there is no implementation and no path to
    /// approval that re-enables them.
    /// </summary>
    public static AiToolRiskLevel Classify(string toolName, AiAgentRun run)
    {
        // Blocked categories — the system must reject these regardless of mode.
        if (IsCriticalBlockedTool(toolName))
            return AiToolRiskLevel.Critical;

        return toolName switch
        {
            // Read-only inspection — always ReadOnly.
            "get_project_tree"        => AiToolRiskLevel.ReadOnly,
            "read_file"               => AiToolRiskLevel.ReadOnly,
            "read_file_range"         => AiToolRiskLevel.ReadOnly,
            "search_code"             => AiToolRiskLevel.ReadOnly,
            "get_symbol"              => AiToolRiskLevel.ReadOnly,
            "find_references"         => AiToolRiskLevel.ReadOnly,
            "get_file_versions"       => AiToolRiskLevel.ReadOnly,
            "get_project_members"     => AiToolRiskLevel.ReadOnly,
            "get_database_schema"     => AiToolRiskLevel.ReadOnly,
            "get_recent_activity"     => AiToolRiskLevel.ReadOnly,
            "get_execution_result"    => AiToolRiskLevel.ReadOnly,

            // Reversible writes — Low risk; eligible for run-scoped auto-approval.
            "create_file"             => AiToolRiskLevel.Low,
            "create_patch"            => AiToolRiskLevel.Low,
            "create_task"             => AiToolRiskLevel.Low,

            // Approval-once — Medium risk.
            "apply_patch"             => AiToolRiskLevel.Medium,
            "update_task"             => AiToolRiskLevel.Medium,
            "run_file"                => AiToolRiskLevel.Medium,

            // Always explicit approval — High risk.
            "rename_file"             => AiToolRiskLevel.High,
            "delete_file"             => AiToolRiskLevel.High,
            "run_build"               => AiToolRiskLevel.High,
            "run_tests"               => AiToolRiskLevel.High,
            "create_git_branch"       => AiToolRiskLevel.High,

            // Default unknown tool to High so unrecognized names cannot slip
            // through to lower-risk classifications.
            _ => AiToolRiskLevel.High
        };
    }

    /// <summary>
    /// Tools in this set must never be exposed to the model. Any attempt to
    /// register or call one of them is rejected.
    /// </summary>
    public static bool IsCriticalBlockedTool(string toolName) => toolName switch
    {
        "run_shell" or "exec_command" or "execute_command" => true,
        "execute_sql" or "run_sql" or "query_database"     => true,
        "read_env" or "dump_env" or "list_environment"      => true,
        "delete_project" or "purge_project"                 => true,
        "disable_authorization" or "bypass_auth"            => true,
        "modify_system_role" or "grant_admin"               => true,
        "open_network" or "fetch_url"                       => true,
        _ => false
    };

    /// <summary>
    /// Maps the AI mode to the set of risk levels it may execute without
    /// raising an approval. Ask and Plan are read-only; Agent can do anything
    /// Critical-tier allows; Review is inspection only.
    /// </summary>
    public static bool ModeAllowsRisk(AiAgentMode mode, AiToolRiskLevel risk) => mode switch
    {
        AiAgentMode.Ask    => risk == AiToolRiskLevel.ReadOnly,
        AiAgentMode.Plan   => risk == AiToolRiskLevel.ReadOnly,
        AiAgentMode.Review => risk == AiToolRiskLevel.ReadOnly,
        AiAgentMode.Agent  => risk != AiToolRiskLevel.Critical,
        _ => false
    };
}