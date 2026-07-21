using Coding.DTOS.Notification;
using Coding.Models;
using Coding.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Coding.Controllers
{
    [Route("api/[controller]")]
    public class NotificationController : CrudControllerBase<Notification, NotificationCreateDTO, NotificationUpdateDTO, NotificationGetDTO>
    {
        public NotificationController(ICrudService<Notification, NotificationCreateDTO, NotificationUpdateDTO, NotificationGetDTO> service) : base(service) { }
    }
}
