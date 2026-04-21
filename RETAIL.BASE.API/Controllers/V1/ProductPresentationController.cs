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
    [Route("api/v{version:apiViewsion}/[controller]")]
    public class ProductPresentationController : BaseController
    {
        private readonly IProductPresentationService _presentationService;

        public ProductPresentationController(IProductPresentationService presentationService)
        {
            _presentationService = presentationService;
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetByIdAsync([FromQuery, Required] int id)
        {
            var webResult = await _presentationService.GetByIdAsync(id);
            return webResult.Success ? Ok(webResult) : BadRequest(webResult);
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] bool? enabled = null, [FromQuery] int? productId = null, int pageIndex = 1, int pageSize = int.MaxValue)
        {
            BaseFilter baseFilter = new BaseFilter()
            {
                Enabled = enabled,
                IdMaster = productId,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            var webResult = await _presentationService.GetAllAsync(baseFilter);
            return webResult.Success ? Ok(webResult) : BadRequest(webResult);
        }

        [HttpPost("Add")]
        public async Task<IActionResult> Add([FromBody] ProductPresentation presentation)
        {
            var webResult = await _presentationService.AddAsync(presentation, IdUserAutenticado);
            return webResult.Success ? Ok(webResult) : BadRequest(webResult);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] ProductPresentation presentation)
        {
            var webResult = await _presentationService.UpdateAsync(presentation, IdUserAutenticado);
            return webResult.Success ? Ok(webResult) : BadRequest(webResult);
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete([FromQuery, Required] int id)
        {
            var webResult = await _presentationService.DeleteAsync(id, IdUserAutenticado);
            return webResult.Success ? Ok(webResult) : BadRequest(webResult);
        }
    }
}
