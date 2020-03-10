using System;
using System.Collections.Generic;
using System.Text;

namespace Ark.ExternalUtilities.Models
{
   public class PaynamicsEnvironment
    {
        public PaynamicsSettings Production { get; set; }
        public PaynamicsSettings Staging { get; set; }
        public PaynamicsSettings Development { get; set; }
    }
}
