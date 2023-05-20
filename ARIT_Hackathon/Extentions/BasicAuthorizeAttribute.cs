using System;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ARIT_Hackathon.Extensions
{
    public class BasicAuthorizeAttribute : TypeFilterAttribute
    {
        public BasicAuthorizeAttribute()
            : base(typeof(AuthorizeActionFilter))
        {
            Arguments = new object[] { };
        }
    }
}
