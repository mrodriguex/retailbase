using Asp.Viewsioning;
using RETAIL.BASE.API.Controllers.Base;
using RETAIL.BASE.NEG.Interfaces;
using RETAIL.BASE.OBJ;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using System.ComponentModel.DataAnnotations;

namespace RETAIL.BASE.API.Controllers.V1
{
    [Authorize]
    [ApiController]
    [ApiViewsion("1.0")]
    [Route("api/v{version:apiViewsion}/[controller]")] // Viewsion in the URL path
    public class CustomerController : BaseController
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] bool? enabled = null, int pageIndex = 1, int pageSize = int.MaxValue)
        {
            BaseFilter filter = new BaseFilter
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                Enabled = enabled
            };
            var webResult = await _customerService.GetAllAsync(filter);
            return webResult.Success ? Ok(webResult) : BadRequest(webResult);
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetByIdAsync([FromQuery, Required] int idCustomer)
        {
            var webResult = await _customerService.GetByIdAsync(idCustomer);
            return webResult.Success ? Ok(webResult) : BadRequest(webResult);
        }

        [HttpPost("Add")]
        public async Task<IActionResult> Add([FromBody] Customer customer)
        {
            var webResult = await _customerService.AddAsync(customer, IdUserAutenticado);
            return webResult.Success ? Ok(webResult) : BadRequest(webResult);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] Customer customer)
        {
            var webResult = await _customerService.UpdateAsync(customer, IdUserAutenticado);
            return webResult.Success ? Ok(webResult) : BadRequest(webResult);
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete([FromQuery, Required] int idCustomer)
        {
            var webResult = await _customerService.DeleteAsync(idCustomer, IdUserAutenticado);
            return webResult.Success ? Ok(webResult) : BadRequest(webResult);
        }
    }
}
