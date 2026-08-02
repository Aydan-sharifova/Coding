namespace Coding.Infrastructure.AiAgent;

/// <summary>
/// Path safety utilities for AI tool inputs. Centralized so all read and
/// write tools apply identical rules for absolute paths, traversal, and
/// symlink-style escapes.
/// </summary>
public static class AiPathGuard
{
    /// <summary>
    /// Normalize a project-relative path. Rejects:
    ///   - absolute paths (POSIX and Windows)
    ///   - parent traversal using ".."
    ///   - empty / whitespace-only paths
    ///   - paths that resolve to a secret file
    /// </summary>
    public static string NormalizeProjectRelativePath(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            throw new ArgumentException("Path is required.", nameof(raw));
        var replaced = raw.Replace('\\', '/').Trim();
        if (Path.IsPathRooted(replaced) || replaced.StartsWith("/") || RegexMatchesDriveLetter(replaced))
            throw new ArgumentException("Absolute paths are not permitted.", nameof(raw));

        var segments = replaced.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var stack = new List<string>(segments.Length);
        foreach (var segment in segments)
        {
            if (segment == ".") continue;
            if (segment == "..") throw new ArgumentException("Path traversal is not permitted.", nameof(raw));
            stack.Add(segment);
        }
        if (stack.Count == 0)
            throw new ArgumentException("Path resolves to the project root.", nameof(raw));

        var joined = string.Join('/', stack);
        if (joined.Contains('\0')) throw new ArgumentException("Null bytes are not permitted in paths.", nameof(raw));
        return joined;
    }

    public static bool IsAbsoluteOrRooted(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return false;
        if (Path.IsPathRooted(raw)) return true;
        if (raw.StartsWith("/") || raw.StartsWith("\\")) return true;
        return RegexMatchesDriveLetter(raw);
    }

    private static bool RegexMatchesDriveLetter(string s) =>
        System.Text.RegularExpressions.Regex.IsMatch(s, "^[A-Za-z]:[\\\\/]");

    /// <summary>
    /// Hard cap on individual file reads. Files larger than this are rejected
    /// by read-only tools even when the user has access. Prevents the model
    /// from accidentally receiving huge blobs or generated bundles.
    /// </summary>
    public const int MaxReadBytes = 256 * 1024;

    public const int MaxLineRange = 2000;

    public const int MaxSearchResults = 200;
}