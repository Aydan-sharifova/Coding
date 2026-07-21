using Coding.DTOS.Responses;
using Coding.Enums;
using Coding.Models;
using Coding.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Coding.Controllers
{
    [ApiController]
    public abstract class CrudControllerBase<TEntity, TCreate, TUpdate, TGet> : ControllerBase
        where TEntity : Base
    {
        private readonly ICrudService<TEntity, TCreate, TUpdate, TGet> _service;

        protected CrudControllerBase(ICrudService<TEntity, TCreate, TUpdate, TGet> service) => _service = service;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TCreate dto) => Result(await _service.CreateAsync(dto));

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] ViewType type = ViewType.notdeleted) => Result(await _service.GetAllAsync(type));

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id) => Result(await _service.GetByIdAsync(id));

        [HttpPatch("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] TUpdate dto) => Result(await _service.UpdateAsync(id, dto));

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id) => Result(await _service.DeleteAsync(id));

        [HttpPatch("toggle/{id:guid}")]
        public async Task<IActionResult> Toggle(Guid id) => Result(await _service.ToggleAsync(id));

        private IActionResult Result(ApiResponse response) => StatusCode(response.StatusCode, response);
    }
}
