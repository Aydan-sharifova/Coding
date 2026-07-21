using Coding.DTOS.CodeHistory;
using Coding.Models;
using Coding.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Coding.Controllers
{
    [Route("api/[controller]")]
    public class CodeHistoryController : CrudControllerBase<CodeHistory, CodeHistoryCreateDTO, CodeHistoryUpdateDTO, CodeHistoryGetDTO>
    {
        public CodeHistoryController(ICrudService<CodeHistory, CodeHistoryCreateDTO, CodeHistoryUpdateDTO, CodeHistoryGetDTO> service) : base(service) { }
    }
}
