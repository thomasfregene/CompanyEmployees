using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Microsoft.AspNetCore.Mvc;

namespace CompanyEmployees.Presentation.Controllers
{
    //[ApiVersion("2.0", Deprecated = true)]
    [Route("api/companies")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "v2")]
    public  class CompaniesV2Controller : ControllerBase
    {
        private readonly IServiceManager _services;

        public CompaniesV2Controller(IServiceManager services) =>
            _services = services;

        [HttpGet]
        public async Task<IActionResult> GetCompanies()
        {
            var companies = await _services.CompanyService
                .GetAllCompaniesAsync(trackChanges: false);
            var companiesV2 = companies.Select(x => $"{x.Name} V2");

            return Ok(companiesV2);
        }
    }
}
