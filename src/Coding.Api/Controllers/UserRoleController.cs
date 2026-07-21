using Coding.DTOS.UserRole;
using Coding.Models;
using Coding.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Coding.Controllers
{
    [Route("api/[controller]")]
    public class UserRoleController : CrudControllerBase<UserRole, UserRoleCreateDTO, UserRoleUpdateDTO, UserRoleGetDTO>
    {
        public UserRoleController(ICrudService<UserRole, UserRoleCreateDTO, UserRoleUpdateDTO, UserRoleGetDTO> service) : base(service) { }
    }
}
