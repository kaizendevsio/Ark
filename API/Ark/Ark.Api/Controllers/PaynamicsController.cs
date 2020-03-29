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
            UserDepositRequestRepository userDepositRequestRepository = new UserDepositRequestRepository();
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

                PaynamicsRequestForm paynamicsRequestForm = paynamics.CreateRequest(paynamicsRequest, Env);

                if (paynamicsRequest.DepositId != 0)
                {
                    TblUserDepositRequest tblUserDepositRequest = userDepositRequestRepository.Get( new TblUserDepositRequest { Id = paynamicsRequest.DepositId }, new ArkContext());

                    tblUserDepositRequest.ReferenceNo = paynamicsRequest.Request_id;
                    tblUserDepositRequest.RawData = paynamicsRequestForm.JsonData;
                    tblUserDepositRequest.Remarks = "PAYNAMICS PAYMENT";

                    userDepositRequestRepository.Update(tblUserDepositRequest, new ArkContext());
                }
                else
                {
                    userDepositRequestRepository.Create(new TblUserDepositRequest
                    {
                        UserAuthId = userAuth.Id,
                        Amount = decimal.Parse(paynamicsRequest.Amount),
                        DepositStatus = (short)DepositStatus.PendingPayment,
                        SourceCurrencyId = 3,
                        TargetWalletTypeId = 20,
                        Remarks = "PAYNAMICS PAYMENT",
                        RawData = paynamicsRequestForm.JsonData,
                        ReferenceNo = paynamicsRequest.Request_id
                    }, new ArkContext());
                }

               




                _apiResponse.ApiResponse = new PaynamicsRequestForm { paymentrequest = paynamicsRequestForm.paymentrequest, RequestUrl = paynamicsRequestForm.RequestUrl };
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

        [HttpPost("ProcessCallbackRequest")]
        public ActionResult ProcessCallbackRequest([FromBody] ShopOrderItemBO shopOrderItem)
        {
            ShopAppService shopAppService = new ShopAppService();
            MailAppService mailAppService = new MailAppService();
            PaynamicsResponseBO _apiResponse = new PaynamicsResponseBO();

            try
            {
                _apiResponse.PaymentResponse = shopAppService.UpdateOrderPayment(shopOrderItem, Env);
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

        [HttpPost("ProcessCallbackRequestCancel")]
        public ActionResult ProcessCallbackRequestCancel([FromBody] ShopOrderItemBO shopOrderItem)
        {
            ShopAppService shopAppService = new ShopAppService();
            MailAppService mailAppService = new MailAppService();
            PaynamicsResponseBO _apiResponse = new PaynamicsResponseBO();

            try
            {
                shopAppService.CancelOrderPayment(shopOrderItem.RawBase64, Env);

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
