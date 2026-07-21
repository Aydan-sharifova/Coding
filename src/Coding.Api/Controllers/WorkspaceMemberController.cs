using Coding.DTOS.WorkspaceMember;
using Coding.Models;
using Coding.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Coding.Controllers
{
    [Route("api/[controller]")]
    public class WorkspaceMemberController : CrudControllerBase<WorkspaceMember, WorkspaceMemberCreateDTO, WorkspaceMemberUpdateDTO, WorkspaceMemberGetDTO>
    {
        public WorkspaceMemberController(ICrudService<WorkspaceMember, WorkspaceMemberCreateDTO, WorkspaceMemberUpdateDTO, WorkspaceMemberGetDTO> service) : base(service) { }
    }
}
