using Coding.DTOS.Folder;
using Coding.Models;
using Coding.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Coding.Controllers
{
    [Route("api/[controller]")]
    public class FolderController : CrudControllerBase<Folder, FolderCreateDTO, FolderUpdateDTO, FolderGetDTO>
    {
        public FolderController(ICrudService<Folder, FolderCreateDTO, FolderUpdateDTO, FolderGetDTO> service) : base(service) { }
    }
}
