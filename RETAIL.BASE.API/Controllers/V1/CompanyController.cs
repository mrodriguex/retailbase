using Asp.Versioning;
using RETAIL.BASE.API.Controllers.Base;
using RETAIL.BASE.NEG.Interfaces;
using RETAIL.BASE.OBJ;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using System.ComponentModel.DataAnnotations;

namespace RETAIL.BASE.API.Controllers.V1
{
    /// <summary>
    /// Controller for managing user companies.
    /// </summary>
    /// <remarks>
    /// This controller provides endpoints for managing user companies.
    /// </remarks>
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")] // Version in the URL path
    [ApiController]
    public class CompanyController : BaseController
    {

        private readonly ICompanyService _companyService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompanyController"/> class.
        /// </summary>
        /// <param name="companyService">
        /// The company service.
        /// </param>
        public CompanyController(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        /// <summary>
        /// Obtains a company by its unique key.
        /// </summary>
        /// <param name="idCompany">The unique key identifying the company.</param>
        /// <returns>The company associated with the provided key.</returns>
        [HttpGet("GetById")]
        public async Task<IActionResult> GetByIdAsync([FromQuery, Required] int idCompany)
        {
            var webResult = await _companyService.GetByIdAsync(idCompany);
            return webResult.Success ? Ok(webResult) : BadRequest(webResult);
        }

        /// <summary>
        /// Obtains all companies.
        /// </summary>
        /// <param name="idRole">
        /// The unique key identifying the profile.
        /// </param>
        /// <param name="userName">
        /// The unique key identifying the user.
        /// </param>
        /// <returns>
        /// A list of companies matching the specified criteria.
        /// </returns>
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] bool? enabled = null, [FromQuery] int? idRole = null, [FromQuery] int? idUser = null, int pageIndex = 1, int pageSize = int.MaxValue)
        {
            BaseFilter filter = new BaseFilter
            {
                Enabled = enabled,
                IdMaster = idRole,
                IdDetail = idUser,
                PageIndex = pageIndex,
                PageSize = pageSize
            };

            var webResult = await _companyService.GetAllAsync(filter);
            return webResult.Success ? Ok(webResult) : BadRequest(webResult);
        }

        [HttpPost("Add")]
        public async Task<IActionResult> Add([FromBody] Company company)
        {
            var webResult = await _companyService.AddAsync(company, IdUserAutenticado);
            return webResult.Success ? Ok(webResult) : BadRequest(webResult);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] Company company)
        {
            var webResult = await _companyService.UpdateAsync(company, IdUserAutenticado);
            return webResult.Success ? Ok(webResult) : BadRequest(webResult);
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete([FromQuery, Required] int idCompany)
        {
            var webResult = await _companyService.DeleteAsync(idCompany, IdUserAutenticado);
            return webResult.Success ? Ok(webResult) : BadRequest(webResult);
        }

        /// <summary>
        /// Obtains all companies assigned to a user.
        /// </summary>
        /// <param name="idUser">
        /// The unique key identifying the user.
        /// </param>
        /// <returns>
        /// A list of companies assigned to the user.
        /// </returns>
        [HttpGet("GetCompaniesByUser")]
        public async Task<IActionResult> GetCompaniesByUser([FromQuery, Required] int idUser, int pageIndex = 1, int pageSize = int.MaxValue)
        {
            var webResult = await _companyService.GetCompaniesByUserAsync(idUser, pageIndex, pageSize);
            return webResult.Success ? Ok(webResult) : BadRequest(webResult);
        }

        /// <summary>
        /// Assigns a company to a user.
        /// </summary>
        /// <param name="idUser">The unique key identifying the user.</param>
        /// <param name="idCompany">The unique key identifying the company.</param>
        /// <returns>A result indicating the success or failure of the operation.</returns>
        [HttpPost("AssignCompanyToUser")]
        public async Task<IActionResult> AssignCompanyToUser([FromQuery, Required] int idUser, [FromQuery, Required] int idCompany)
        {
            var webResult = await _companyService.AssignCompanyToUserAsync(idUser, idCompany);
            return webResult.Success ? Ok(webResult) : BadRequest(webResult);
        }

        /// <summary> Removes a company from a user.
        /// </summary> <param name="idUser">The unique key identifying the user.</param>
        /// <param name="idCompany">The unique key identifying the company.</param>
        /// <returns>A result indicating the success or failure of the operation.</returns>
        [HttpPost("RemoveCompanyFromUser")]
        public async Task<IActionResult> RemoveCompanyFromUser([FromQuery, Required] int idUser, [FromQuery, Required] int idCompany)
        {
            var webResult = await _companyService.RemoveCompanyFromUserAsync(idUser, idCompany);
            return webResult.Success ? Ok(webResult) : BadRequest(webResult);
        }
    }
}