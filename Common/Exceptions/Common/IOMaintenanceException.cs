using System;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Exceptions.Base;

namespace IOBootstrap.NET.Common.Exceptions.Common
{
    public class IOMaintenanceException : IOServiceException
    {
        public IOMaintenanceException() : base(IOExceptionMessages.MaintenanceCode, IOExceptionMessages.MaintenanceMessage, "")
        {
        }

        public IOMaintenanceException(string detailedMessage) : base(IOExceptionMessages.MaintenanceCode, IOExceptionMessages.MaintenanceMessage, detailedMessage)
        {
        }
    }
}
