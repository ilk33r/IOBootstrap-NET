using System;
using System.ComponentModel.DataAnnotations;
using IOBootstrap.NET.Common.Extensions;

namespace IOBootstrap.NET.Common.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
        AllowMultiple = false)]
public class IOBackofficeRequestAttribute : RegularExpressionAttribute
{

    public IOBackofficeRequestAttribute() : base(@"^([a-zA-Z0-9-_@./\\\#\+\ \(\)\?\n\=]+)$")
    {
        ErrorMessage = "Invalid characters.";
    }

    public override bool IsValid(object? value)
    {
        bool valid = base.IsValid(value);
        string? stringValue = (string?)value;

        if (String.IsNullOrEmpty(stringValue))
        {
            return valid;
        }

        string sanitizedString = stringValue.SanitizeHtml();
        if (stringValue == sanitizedString)
        {
            return valid;
        }

        return false;
    }
}
