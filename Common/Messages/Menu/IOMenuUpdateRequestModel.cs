using System;

namespace IOBootstrap.NET.Common.Messages.Menu
{
    public class IOMenuUpdateRequestModel : IOMenuAddRequestModel
    {
        public int ID { get; set; }

        public IOMenuUpdateRequestModel() : base()
        {
        }
    }
}
