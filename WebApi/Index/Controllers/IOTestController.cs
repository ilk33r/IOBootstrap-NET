using System;
using Microsoft.AspNetCore.Mvc;

namespace IOBootstrap.NET.WebApi.Index.Controllers
{

    // Deprecated
    // [Obsolete("This Method is Deprecated", false)]
    [ApiController]
    [Route("[controller]")]
    public class IOTestController : Controller
    {
        [HttpGet("[action]")]
        public string Check() 
        {
            return "OK";
        }
    }
}
