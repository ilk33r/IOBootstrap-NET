using System;
using System.Text.Json;
using IOBootstrap.NET.Common.Models.Base;

namespace IOBootstrap.NET.Common.Models.Configuration
{
    public class IOConfigurationModel : IOModel
    {
        public int ID { get; set; }
        public string ConfigKey { get; set; }
        public int? ConfigIntValue { get; set; }
        public string ConfigStringValue { get; set; }

        public int IntValue()
        {
            return this.ConfigIntValue ?? 0;
        }

        public string StringValue()
        {
            return this.ConfigStringValue ?? "";
        }

        public TModel ObjectValue<TModel>() where TModel : IOModel, new()
        {
            if (String.IsNullOrEmpty(this.ConfigStringValue))
            {
                return new TModel();
            }
                
            return JsonSerializer.Deserialize<TModel>(this.ConfigStringValue);
        }
    }
}
