using Coding.DTOS.Responses;
using Coding.Enums;
using Coding.Models;

namespace Coding.Services.Interfaces
{
    public interface ICrudService<TEntity, in TCreate, in TUpdate, TGet>
        where TEntity : Base
    {
        Task<ApiResponse> CreateAsync(TCreate dto);
        Task<ApiResponse> GetAllAsync(ViewType type);
        Task<ApiResponse> GetByIdAsync(Guid id);
        Task<ApiResponse> UpdateAsync(Guid id, TUpdate dto);
        Task<ApiResponse> DeleteAsync(Guid id);
        Task<ApiResponse> ToggleAsync(Guid id);
    }
}
