using Coding.DTOS.RefreshToken;
using Coding.Models;
using Coding.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Coding.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class RefreshTokenController : CrudControllerBase<RefreshToken, RefreshTokenCreateDTO, RefreshTokenUpdateDTO, RefreshTokenGetDTO>
    {
        public RefreshTokenController(ICrudService<RefreshToken, RefreshTokenCreateDTO, RefreshTokenUpdateDTO, RefreshTokenGetDTO> service) : base(service) { }
    }
}
