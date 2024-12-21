using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Product.Api.Models;
using Product.Business.Models;
using Product.Business.Services;
using SharedKernel.Enums;
using SharedKernel.Notifications;
using System.Net;

namespace Product.Api.Controllers.V1
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly INotification _notification;
        const string AuthorizeRoles = UserRoles.Admin + "," + UserRoles.User;

        public ProductsController(IProductService productService, INotification notification)
        {
            _productService = productService;
            _notification = notification;
        }


        [HttpGet]
        [Authorize(Roles = AuthorizeRoles)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetAllAsync();
            if (products is { })
            {
                return Ok(await _productService.GetAllAsync());
            }

            if (_notification.NotificationType is NotificationType.Unexpected)
            {
                return StatusCode(500, new ResponseError(_notification.GetSummary()));
            }

            return NotFound();
        }

        [HttpGet("{id}")]
        [Authorize(Roles = AuthorizeRoles)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GeById(Guid id)
        {
            var product = await _productService.GetByIdAsync(id);

            if (_notification.NotificationType is NotificationType.Unexpected)
            {
                return StatusCode(500, new ResponseError(_notification.GetSummary()));
            }

            if (_notification.NotificationType is NotificationType.NotFound)
            {
                return NotFound(new ResponseError(_notification.GetSummary()));
            }

            return Ok(product);
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Admin)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> Create(ProductModel product)
        {
            await _productService.CreateAsync(product);

            if (_notification.NotificationType is NotificationType.Unexpected)
            {
                return StatusCode(500, new ResponseError(_notification.GetSummary()));
            }

            if (_notification.Any())
            {
                return BadRequest(new ResponseError(_notification.GetSummary()));
            }

            return CreatedAtAction(nameof(GeById), new { id = product.Id }, product);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = UserRoles.Admin)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            await _productService.DeleteAsync(id);

            if (_notification.NotificationType is NotificationType.Unexpected)
            {
                return StatusCode(500, new ResponseError(_notification.GetSummary()));
            }

            if (_notification.NotificationType is NotificationType.NotFound)
            {
                return NotFound(new ResponseError(_notification.GetSummary()));
            }

            return Ok(new { message = $"Product {id} deleted successfully." });
        }

        [HttpPut]
        [Authorize(Roles = UserRoles.Admin)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> UpdateProduct([FromBody] ProductModel product)
        {
            await _productService.UpdateAsync(product);

            if (_notification.NotificationType is NotificationType.Unexpected)
            {
                return StatusCode(500, new ResponseError(_notification.GetSummary()));
            }

            if (_notification.NotificationType is NotificationType.NotFound)
            {
                return NotFound(new ResponseError(_notification.GetSummary()));
            }

            if (_notification.Any())
            {
                return BadRequest(new ResponseError(_notification.GetSummary()));
            }

            return Ok(new { message = $"Product {product.Name} updated successfully." });
        }
    }

}
