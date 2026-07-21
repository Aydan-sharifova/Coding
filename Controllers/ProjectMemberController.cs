using Coding.DTOS.ProjectMember;
using Coding.Models;
using Coding.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Coding.Controllers
{
    [Route("api/[controller]")]
    public class ProjectMemberController : CrudControllerBase<ProjectMember, ProjectMemberCreateDTO, ProjectMemberUpdateDTO, ProjectMemberGetDTO>
    {
        public ProjectMemberController(ICrudService<ProjectMember, ProjectMemberCreateDTO, ProjectMemberUpdateDTO, ProjectMemberGetDTO> service) : base(service) { }
    }
}
