using Coding.Data;
using Coding.DTOS.Auth;
using Coding.Exceptions;
using Coding.Models;
using Coding.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Coding.Infrastructure.Authentication;

public sealed class RoleService : IRoleService
{
    private readonly AppDbContext context;

    public RoleService(AppDbContext context)
    {
        this.context = context;
    }

    public async Task AssignRoleAsync(AssignRoleRequest request, CancellationToken cancellationToken)
    {
        if (!SystemRoles.All.Contains(request.Role, StringComparer.OrdinalIgnoreCase))
            throw new NotFoundException("Role not found.");

        var userExists = await context.Users.AnyAsync(item => item.ID == request.UserId, cancellationToken);
        if (!userExists) throw new NotFoundException("User not found.");

        var role = await context.Roles.SingleAsync(
            item => item.Name.ToLower() == request.Role.ToLower(),
            cancellationToken);

        var exists = await context.UserRoles.AnyAsync(
            item => item.UserId == request.UserId && item.RoleId == role.ID,
            cancellationToken);
        if (exists) return;

        context.UserRoles.Add(new UserRole { UserId = request.UserId, RoleId = role.ID });
        await context.SaveChangesAsync(cancellationToken);
    }
}
