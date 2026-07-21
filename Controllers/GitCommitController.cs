using Coding.DTOS.GitCommit;
using Coding.Models;
using Coding.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Coding.Controllers
{
    [Route("api/[controller]")]
    public class GitCommitController : CrudControllerBase<GitCommit, GitCommitCreateDTO, GitCommitUpdateDTO, GitCommitGetDTO>
    {
        public GitCommitController(ICrudService<GitCommit, GitCommitCreateDTO, GitCommitUpdateDTO, GitCommitGetDTO> service) : base(service) { }
    }
}
