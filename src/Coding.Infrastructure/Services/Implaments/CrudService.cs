using System.Reflection;
using Coding.Data;
using Coding.DTOS.Responses;
using Coding.Enums;
using Coding.Models;
using Coding.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Coding.Services.Implaments
{
    public class CrudService<TEntity, TCreate, TUpdate, TGet>
        : ICrudService<TEntity, TCreate, TUpdate, TGet>
        where TEntity : Base, new()
        where TGet : new()
    {
        private readonly AppDbContext _context;
        private readonly DbSet<TEntity> _entities;

        public CrudService(AppDbContext context)
        {
            _context = context;
            _entities = context.Set<TEntity>();
        }

        public async Task<ApiResponse> CreateAsync(TCreate dto)
        {
            var entity = new TEntity();
            CopyMatchingProperties(dto, entity, false);
            SetUserPassword(dto, entity);
            entity.CreatAt = DateTime.UtcNow;
            await _entities.AddAsync(entity);
            await _context.SaveChangesAsync();
            return Response(StatusCodes.Status201Created, "Created successfully!", Map(entity));
        }

        public async Task<ApiResponse> GetAllAsync(ViewType type)
        {
            IQueryable<TEntity> query = _entities.AsNoTracking();
            query = type switch
            {
                ViewType.deleted => query.Where(x => x.IsDeleted),
                ViewType.notdeleted => query.Where(x => !x.IsDeleted),
                _ => query
            };
            var entities = await query.OrderByDescending(x => x.CreatAt).ToListAsync();
            var dtos = entities.Select(Map).ToList();
            return Response(StatusCodes.Status200OK, $"Total: {dtos.Count}", dtos);
        }

        public async Task<ApiResponse> GetByIdAsync(Guid id)
        {
            var entity = await _entities.AsNoTracking().FirstOrDefaultAsync(x => x.ID == id);
            return entity is null
                ? Response(StatusCodes.Status404NotFound, $"{typeof(TEntity).Name} not found!")
                : Response(StatusCodes.Status200OK, data: Map(entity));
        }

        public async Task<ApiResponse> UpdateAsync(Guid id, TUpdate dto)
        {
            var entity = await _entities.FirstOrDefaultAsync(x => x.ID == id && !x.IsDeleted);
            if (entity is null)
                return Response(StatusCodes.Status404NotFound, $"{typeof(TEntity).Name} not found!");
            CopyMatchingProperties(dto, entity, true);
            entity.UpdateAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return Response(StatusCodes.Status200OK, "Updated successfully!", Map(entity));
        }

        public async Task<ApiResponse> DeleteAsync(Guid id)
        {
            var entity = await _entities.FindAsync(id);
            if (entity is null)
                return Response(StatusCodes.Status404NotFound, $"{typeof(TEntity).Name} not found!");
            _entities.Remove(entity);
            await _context.SaveChangesAsync();
            return Response(StatusCodes.Status200OK, "Deleted permanently!");
        }

        public async Task<ApiResponse> ToggleAsync(Guid id)
        {
            var entity = await _entities.FindAsync(id);
            if (entity is null)
                return Response(StatusCodes.Status404NotFound, $"{typeof(TEntity).Name} not found!");
            entity.IsDeleted = !entity.IsDeleted;
            entity.DeletedAt = entity.IsDeleted ? DateTime.UtcNow : null;
            entity.UpdateAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return Response(StatusCodes.Status200OK, entity.IsDeleted ? "Deleted temporarily!" : "Restored successfully!", Map(entity));
        }

        private static TGet Map(TEntity entity)
        {
            var dto = new TGet();
            CopyMatchingProperties(entity, dto, false);
            SetIfPresent(dto, "Id", entity.ID);
            SetIfPresent(dto, "CreatedOn", entity.CreatAt);
            SetIfPresent(dto, "UpdatedOn", entity.UpdateAt);
            SetIfPresent(dto, "DeletedOn", entity.DeletedAt);
            SetIfPresent(dto, "IsDeleted", entity.IsDeleted);
            return dto;
        }

        private static void CopyMatchingProperties<TSource, TTarget>(TSource source, TTarget target, bool ignoreNull)
        {
            var targetProperties = typeof(TTarget).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.CanWrite).ToDictionary(x => x.Name);
            foreach (var sourceProperty in typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(x => x.CanRead))
            {
                if (!targetProperties.TryGetValue(sourceProperty.Name, out var targetProperty)) continue;
                var value = sourceProperty.GetValue(source);
                if (ignoreNull && value is null) continue;
                var underlying = Nullable.GetUnderlyingType(sourceProperty.PropertyType);
                if (underlying is not null && targetProperty.PropertyType == underlying && value is not null)
                    targetProperty.SetValue(target, value);
                else if (targetProperty.PropertyType.IsAssignableFrom(sourceProperty.PropertyType))
                    targetProperty.SetValue(target, value);
            }
        }

        private static void SetIfPresent<T>(T target, string propertyName, object? value)
        {
            typeof(T).GetProperty(propertyName)?.SetValue(target, value);
        }

        private static void SetUserPassword(TCreate dto, TEntity entity)
        {
            if (entity is not User user) return;

            var password = typeof(TCreate).GetProperty("Password")?.GetValue(dto) as string;
            if (!string.IsNullOrWhiteSpace(password))
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
        }

        private static ApiResponse Response(int statusCode, string? message = null, object? data = null) => new()
        {
            StatusCode = statusCode,
            Message = message,
            Data = data
        };
    }
}
