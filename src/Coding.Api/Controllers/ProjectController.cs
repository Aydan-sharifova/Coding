using Coding.DTOS.Project;
using Coding.Models;
using Coding.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Coding.Controllers
{
    [Route("api/[controller]")]
    public class ProjectController : CrudControllerBase<Project, ProjectCreateDTO, ProjectUpdateDTO, ProjectGetDTO>
    {
        public ProjectController(ICrudService<Project, ProjectCreateDTO, ProjectUpdateDTO, ProjectGetDTO> service) : base(service) { }
    }
}
