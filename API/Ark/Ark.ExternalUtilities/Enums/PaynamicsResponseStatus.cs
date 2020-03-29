using System;
using System.Collections.Generic;
using System.Text;

namespace Ark.ExternalUtilities.Enums
{
   public enum PaynamicsResponseStatus : int
    {
        Pending = 0,
        Error = 1,
        Cancelled = 2,
        Success = 3,
    }
}
