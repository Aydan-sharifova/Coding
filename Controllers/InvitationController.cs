using Coding.DTOS.Invitation;
using Coding.Models;
using Coding.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Coding.Controllers
{
    [Route("api/[controller]")]
    public class InvitationController : CrudControllerBase<Invitation, InvitationCreateDTO, InvitationUpdateDTO, InvitationGetDTO>
    {
        public InvitationController(ICrudService<Invitation, InvitationCreateDTO, InvitationUpdateDTO, InvitationGetDTO> service) : base(service) { }
    }
}
