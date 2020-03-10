using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ark.API.Controllers;
using Ark.AppService;
using Ark.DataAccessLayer;
using Ark.Entities.BO;
using Ark.Entities.DTO;
using Ark.Entities.Enums;
using Ark.ExternalUtilities;
using Ark.ExternalUtilities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace Ark.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaynamicsController : ControllerBase
    {
        public readonly string Env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        [HttpPost]
        public ActionResult NewTransaction([FromBody] PaynamicsRequest paynamicsRequest)
        {
            Paynamics paynamics = new Paynamics();
            UserInfoRepository userInfoRepository = new UserInfoRepository();
            PaynamicsResponseBO _apiResponse = new PaynamicsResponseBO();

            try
            {
                SessionController sessionController = new SessionController();
                TblUserAuth userAuth = sessionController.GetSession(HttpContext.Session);
                TblUserInfo userInfo = userInfoRepository.Get(userAuth, new ArkContext());

                paynamicsRequest.Fname = userInfo.FirstName;
                paynamicsRequest.Lname = userInfo.LastName;
                paynamicsRequest.Email = userInfo.Email;
                paynamicsRequest.Phone = userInfo.PhoneNumber;
                paynamicsRequest.Mobile = userInfo.PhoneNumber;


                _apiResponse.ApiResponse = paynamics.CreateRequest(paynamicsRequest, Env);
                _apiResponse.HttpStatusCode = "200";
                _apiResponse.Message = "Api Call Successful";
                _apiResponse.Status = "Success";

                return Ok(_apiResponse);
            }
            catch (Exception ex)
            {
                _apiResponse.HttpStatusCode = "500";
                _apiResponse.Message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _apiResponse.Status = "Error";
                return Ok(_apiResponse);
            }
        }
    }
}
