using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Interfaces;
using Entities;
using System.Xml.Serialization;
using System.Xml;
using Microsoft.EntityFrameworkCore.Storage;

using Microsoft.EntityFrameworkCore;
using System.IO;

using Microsoft.Extensions.Options;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net.Http;
using System.Dynamic;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace ARIT_Hackathon.Controllers
{
    //[BasicAuthorize]
    [Route("api/[controller]")]
    public class CommonController : Controller
    {
        private ILoggerManager _logger;
        private IRepositoryWrapper _repoWrapper;        
        private RepositoryContext _context;
        public CommonController(ILoggerManager logger, IRepositoryWrapper repoWrapper, RepositoryContext context)
        {
            _logger = logger;
            _repoWrapper = repoWrapper;
            _context = context;            
        }
        [Route("GetUserLoginData")]
        [HttpGet]
        public IActionResult GetUserData()
        {
            string Pan = "AXGPT8163J";
            _logger.LogInfo($"Fetch all process start");
            var userData = _context.UserLogins.AsNoTracking().Where(x => Pan == Pan.ToUpper() && (x.ExpireyDt==null || x.ExpireyDt>DateTime.Now)).FirstOrDefault();
            return Ok(userData);
        }
    }
}
