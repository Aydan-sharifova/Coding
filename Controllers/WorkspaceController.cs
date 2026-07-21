using Coding.DTOS.Workspace;
using Coding.Models;
using Coding.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Coding.Controllers
{
    [Route("api/[controller]")]
    public class WorkspaceController : CrudControllerBase<Workspace, WorkspaceCreateDTO, WorkspaceUpdateDTO, WorkspaceGetDTO>
    {
        public WorkspaceController(ICrudService<Workspace, WorkspaceCreateDTO, WorkspaceUpdateDTO, WorkspaceGetDTO> service) : base(service) { }
    }
}
