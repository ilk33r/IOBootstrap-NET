﻿using System;
using IOBootstrap.NET.Common.Constants;

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
            Code = code;
            DetailedMessage = detailedMessage;
            if (IOResponseStatusMessages.Messages.ContainsKey(code))
            {
                Message = IOResponseStatusMessages.Messages[code];
            }
            else 
            {
                Message = "";
            }

            // Check code is equal to the zero
            if (code == IOResponseStatusMessages.OK)
            {
                // Then set success true
                Success = true;
            }
            else
            {
                // Set success false
                Success = false;
            }
        }

        public IOResponseStatusModel(int code, String message, bool success, String detailedMessage = "")
        {
            // Setup properties
            Code = code;
            Message = message;
            Success = success;
            DetailedMessage = detailedMessage;
        }

        #endregion
    }
}
