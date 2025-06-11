using System;
using System.ComponentModel.DataAnnotations;

namespace IOBootstrap.NET.Common.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
        AllowMultiple = false)]
public class IOBackofficeRequestAttribute : RegularExpressionAttribute
{

    public IOBackofficeRequestAttribute() : base(@"^([a-zA-Z0-9-_@./\\\#\+\ \(\)\?]+)$")
    {
        ErrorMessage = "Invalid characters.";
    }

    public override bool IsValid(object? value)
    {
        bool valid = base.IsValid(value);
        return valid;
    }
}
