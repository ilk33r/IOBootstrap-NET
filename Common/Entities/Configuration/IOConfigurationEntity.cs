using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using IOBootstrap.NET.Common.Models.Shared;
using IOBootstrap.NET.Core.Database;
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

        public static IOConfigurationEntity ConfigForKey<TDBContext>(string configKey, TDBContext inContext)
            where TDBContext : IODatabaseContext<TDBContext>
        {
            var configurations = inContext.Configurations.Where((arg) => arg.ConfigKey.Equals(configKey));

            if (configurations.Count() > 0)
            {
                return configurations.First();
            }

            return null;
        }

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
