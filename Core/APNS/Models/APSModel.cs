using System;

namespace IOBootstrap.NET.Core.APNS.Utils.Models
{
    public class APSModel
    {

        public APSAlertModel alert { get; set; }
        public string sound { get; set; }
        public int badge { get; set; }

    }
}
