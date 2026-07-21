using Coding.DTOS.RefreshToken;
using Coding.Models;
using Coding.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Coding.Controllers
{
    [Route("api/[controller]")]
    public class RefreshTokenController : CrudControllerBase<RefreshToken, RefreshTokenCreateDTO, RefreshTokenUpdateDTO, RefreshTokenGetDTO>
    {
        public RefreshTokenController(ICrudService<RefreshToken, RefreshTokenCreateDTO, RefreshTokenUpdateDTO, RefreshTokenGetDTO> service) : base(service) { }
    }
}
