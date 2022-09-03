using System;
using Microsoft.AspNetCore.Mvc;

namespace IOBootstrap.NET.WebApi.Index.Controllers
{

    // Deprecated
    // [Obsolete("This Method is Deprecated", false)]
    [Produces("application/json")]
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
