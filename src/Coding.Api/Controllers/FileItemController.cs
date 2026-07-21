using Coding.DTOS.FileItem;
using Coding.Models;
using Coding.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Coding.Controllers
{
    [Route("api/[controller]")]
    public class FileItemController : CrudControllerBase<FileItem, FileItemCreateDTO, FileItemUpdateDTO, FileItemGetDTO>
    {
        public FileItemController(ICrudService<FileItem, FileItemCreateDTO, FileItemUpdateDTO, FileItemGetDTO> service) : base(service) { }
    }
}
