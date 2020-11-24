using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace ACG.Vendors.ADAPT.api.Controllers
{
    [ApiController]
    [Route("/machines")]
    public class MachinesController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public MachinesController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetMachines(string userId)
        {

            return NotFound();
        }

        [HttpGet("{machineId}")]
        public async Task<IActionResult> GetMachine(string machineId)
        {
           
            return BadRequest();
        }

    }
}

