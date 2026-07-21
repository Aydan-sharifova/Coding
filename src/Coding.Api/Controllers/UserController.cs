using Coding.DTOS.User;
using Coding.Models;
using Coding.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Coding.Controllers
{
    [Route("api/[controller]")]
    public class UserController : CrudControllerBase<User, UserCreateDTO, UserUpdateDTO, UserGetDTO>
    {
        public UserController(ICrudService<User, UserCreateDTO, UserUpdateDTO, UserGetDTO> service) : base(service) { }
    }
}
