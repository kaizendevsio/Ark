using Ark.ExternalUtilities.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ark.Entities.BO
{
    public class PaynamicsResponseBO : ApiResponseBO
    {
        public PaynamicsRequestForm ApiResponse { get; set; }

        public ShopOrderItemBO PaymentResponse { get; set; }
    }
}
