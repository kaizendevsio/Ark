using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ark.API.Controllers;
using Ark.AppService;
using Ark.DataAccessLayer;
using Ark.Entities.BO;
using Ark.Entities.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ark.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AffiliateController : ControllerBase
    {
        [HttpPost("InvitationLink")]
        public ActionResult InvitationLink([FromBody] AffiliateMapBO affiliateMapBO)
        {
            AffiliateAppService affiliateAppService = new AffiliateAppService();
            AffiliateLinkResponseBO _apiResponse = new AffiliateLinkResponseBO();

            try
            {
                _apiResponse.AffiliateMapBO = affiliateAppService.GetAffiliateLink(affiliateMapBO);

                _apiResponse.HttpStatusCode = "200";
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

        [HttpPost("Commission")]
        public ActionResult Commission([FromBody] AffiliateCountersBO affiliateCounters)
        {
            AffiliateAppService affiliateAppService = new AffiliateAppService();
            AffiliateCommissionResponseBO _apiResponse = new AffiliateCommissionResponseBO();

            try
            {
                // GET SESSIONS
                UserAuthRepository userAuthRepository = new UserAuthRepository();
                TblUserAuth userAuth = userAuthRepository.GetByShopID(long.Parse(affiliateCounters.ShopUserId), new ArkContext());

                _apiResponse.Commission = affiliateAppService.ComputeCommissions(userAuth, affiliateCounters.amountPaid);
                _apiResponse.HttpStatusCode = "200";
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
