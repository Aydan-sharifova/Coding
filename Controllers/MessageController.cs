using Coding.DTOS.Message;
using Coding.Models;
using Coding.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Coding.Controllers
{
    [Route("api/[controller]")]
    public class MessageController : CrudControllerBase<Message, MessageCreateDTO, MessageUpdateDTO, MessageGetDTO>
    {
        public MessageController(ICrudService<Message, MessageCreateDTO, MessageUpdateDTO, MessageGetDTO> service) : base(service) { }
    }
}
