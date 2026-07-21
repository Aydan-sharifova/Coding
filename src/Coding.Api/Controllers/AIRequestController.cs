using Coding.DTOS.AIRequest;
using Coding.Models;
using Coding.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Coding.Controllers
{
    [Route("api/[controller]")]
    public class AIRequestController
        : CrudControllerBase<AIRequest, AIRequestCreateDTO, AIRequestUpdateDTO, AIRequestGetDTO>
    {
        public AIRequestController(
            ICrudService<AIRequest, AIRequestCreateDTO, AIRequestUpdateDTO, AIRequestGetDTO> service)
            : base(service)
        {
        }
    }
}
