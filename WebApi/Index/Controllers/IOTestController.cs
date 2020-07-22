using System;
using Microsoft.AspNetCore.Mvc;

namespace IOBootstrap.NET.WebApi.Index.Controllers
{
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
