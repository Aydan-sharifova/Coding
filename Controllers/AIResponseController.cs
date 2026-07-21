using Coding.DTOS.AIResponse;
using Coding.Models;
using Coding.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Coding.Controllers
{
    [Route("api/[controller]")]
    public class AIResponseController : CrudControllerBase<AIResponse, AIResponseCreateDTO, AIResponseUpdateDTO, AIResponseGetDTO>
    {
        public AIResponseController(ICrudService<AIResponse, AIResponseCreateDTO, AIResponseUpdateDTO, AIResponseGetDTO> service) : base(service) { }
    }
}
