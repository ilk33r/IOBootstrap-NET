using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Exceptions.Base;

namespace IOBootstrap.NET.Common.Exceptions.Common;

[Serializable]
public class IOEncryptionRequiredException : IOServiceException
{
    public IOEncryptionRequiredException() : base(IOExceptionMessages.EncryptionRequiredCode, IOExceptionMessages.EncryptionRequiredMessage, "")
    {
    }

    public IOEncryptionRequiredException(string detailedMessage) : base(IOExceptionMessages.EncryptionRequiredCode, IOExceptionMessages.EncryptionRequiredMessage, detailedMessage)
    {
    }
}
