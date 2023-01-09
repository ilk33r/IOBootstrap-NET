using System;
using System.ComponentModel.DataAnnotations;
using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.NET.Common.Messages.FN
{
    public class IOFNPushNotificationDevicesRequestModel : IORequestModel
    {
        [Required]
        public DeviceTypes DeviceType { get; set; }

        [Required]
        public int MessageId { get; set; }
        
        public int? ClientId { get; set; }
    }
}
