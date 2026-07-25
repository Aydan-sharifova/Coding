using Coding.Application.Features.UserSettings;
using Microsoft.AspNetCore.Hosting;

namespace Coding.Infrastructure.UserSettings;

public sealed class LocalFileStorageService(IWebHostEnvironment environment) : IFileStorageService
{
    public async Task<string> SaveAsync(Stream content, string extension, string contentType, CancellationToken ct)
    {
        var directory = Path.Combine(environment.WebRootPath ?? Path.Combine(environment.ContentRootPath, "wwwroot"), "uploads", "avatars");
        Directory.CreateDirectory(directory);
        var name = $"{Guid.NewGuid():N}{extension}";
        await using var output = File.Create(Path.Combine(directory, name));
        await content.CopyToAsync(output, ct);
        return $"/uploads/avatars/{name}";
    }
    public Task DeleteAsync(string path, CancellationToken ct)
    {
        if (!path.StartsWith("/uploads/avatars/", StringComparison.Ordinal)) return Task.CompletedTask;
        var name = Path.GetFileName(path); var root = environment.WebRootPath ?? Path.Combine(environment.ContentRootPath, "wwwroot"); var target = Path.Combine(root, "uploads", "avatars", name);
        if (File.Exists(target)) File.Delete(target);
        return Task.CompletedTask;
    }
}
