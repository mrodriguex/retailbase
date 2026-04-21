using Asp.Versioning;
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
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ProductController : BaseController
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetByIdAsync([FromQuery, Required] int id)
        {
            var webResult = await _productService.GetByIdAsync(id);
            return webResult.Success ? Ok(webResult) : BadRequest(webResult);
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] bool? enabled = null, int pageIndex = 1, int pageSize = int.MaxValue)
        {
            BaseFilter baseFilter = new BaseFilter()
            {
                Enabled = enabled,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            var webResult = await _productService.GetAllAsync(baseFilter);
            return webResult.Success ? Ok(webResult) : BadRequest(webResult);
        }

        [HttpPost("Add")]
        public async Task<IActionResult> Add([FromBody] Product product)
        {
            var webResult = await _productService.AddAsync(product, IdUserAutenticado);
            return webResult.Success ? Ok(webResult) : BadRequest(webResult);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] Product product)
        {
            var webResult = await _productService.UpdateAsync(product, IdUserAutenticado);
            return webResult.Success ? Ok(webResult) : BadRequest(webResult);
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete([FromQuery, Required] int id)
        {
            var webResult = await _productService.DeleteAsync(id, IdUserAutenticado);
            return webResult.Success ? Ok(webResult) : BadRequest(webResult);
        }
    }
}
