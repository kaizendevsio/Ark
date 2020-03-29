using System;
using System.Collections.Generic;
using System.Text;
using Ark.ExternalUtilities.Enums;

namespace Ark.Entities.DTO
{
   public partial class TblPaynamicsResponse
    {
        public long Id { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public string Description { get; set; }
        public PaynamicsResponseStatus Status { get; set; }
    }
}
