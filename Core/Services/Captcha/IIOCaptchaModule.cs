using System;

namespace IOBootstrap.NET.Core.Services.Captcha;

public interface IIOCaptchaModule
{
    byte[] Generate(string stringText, string projectDir);
}
