using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IOBootstrap.NET.Common.Models.Shared;
using Newtonsoft.Json;

namespace IOBootstrap.NET.Common.Entities.Configuration
{
    public class IOConfigurationEntity
    {

        #region Properties

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [StringLength(128)]
        public string ConfigKey { get; set; }

        public int? ConfigIntValue { get; set; }
        public string ConfigStringValue { get; set; }

        #endregion

        #region Helper Methods

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
                
            return JsonConvert.DeserializeObject<TModel>(this.ConfigStringValue);
        }

        #endregion

    }
}
