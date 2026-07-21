using Coding.DTOS.Role;
using Coding.Models;
using Coding.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Coding.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class RoleController : CrudControllerBase<Role, RoleCreateDTO, RoleUpdateDTO, RoleGetDTO>
    {
        public RoleController(ICrudService<Role, RoleCreateDTO, RoleUpdateDTO, RoleGetDTO> service) : base(service) { }
    }
}
