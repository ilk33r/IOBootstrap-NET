using IOBootstrap.NET.Common.Constants;
using System;

namespace IOBootstrap.NET.Common.Models.Shared
{
    public class IOResponseStatusModel
    {

        #region Properties

        public int Code { get; }
        public String DetailedMessage { get; }
        public String Message { get; }
        public bool Success { get; }

        #endregion

        #region Initialization Methods

        public IOResponseStatusModel(int code, String detailedMessage = "")
        {
            // Setup properties
            this.Code = code;
            this.DetailedMessage = detailedMessage;
            this.Message = IOResponseStatusMessages.Messages[code];

            // Check code is equal to the zero
            if (code == 0)
            {
                // Then set success true
                this.Success = true;
            }
            else
            {
                // Set success false
                this.Success = false;
            }
        }

        public IOResponseStatusModel(int code, String message, bool success, String detailedMessage = "")
        {
            // Setup properties
            this.Code = code;
            this.Message = message;
            this.Success = success;
            this.DetailedMessage = detailedMessage;
        }

        #endregion
    }
}
