using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ark.AppService;
using Ark.Entities.BO;
using Ark.Entities.DTO;
using Ark.Entities.Enums;
using Ark.ExternalUtilities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ark.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShopController : ControllerBase
    {
        public readonly string Env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        [HttpPost("UpdateOrderPayment")]
        public ActionResult UpdateOrderPayment([FromBody] ShopOrderItemBO shopOrderItem)
        {
            ShopAppService shopAppService = new ShopAppService();
            MailAppService mailAppService = new MailAppService();
            ApiResponseBO _apiResponse = new ApiResponseBO();

            try
            {
                shopAppService.UpdateOrderPayment(shopOrderItem, Env);

                _apiResponse.HttpStatusCode = "200";
                _apiResponse.Message = "Package successfully purchased";
                _apiResponse.RedirectUrl = "/admin/customers";
                _apiResponse.Status = "Success";

                return Ok(_apiResponse);
            }
            catch (Exception ex)
            {
                _apiResponse.HttpStatusCode = "500";
                _apiResponse.Message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _apiResponse.Status = "Error";
                return BadRequest(_apiResponse);
            }

        }
    }
}
