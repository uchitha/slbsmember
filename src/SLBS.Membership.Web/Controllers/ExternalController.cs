using SLBS.Membership.Web.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;

namespace SLBS.Membership.Web.Controllers
{
    public class ExternalController : ApiController
    {
        [System.Web.Http.HttpPost]
        public async Task<JsonResult> SendMail(string message,string subject)
        {
            //Send emails
            var sender = new EmailSender(EnumMode.External);

            await sender.SendExternalMail(message, subject, ConfigurationManager.AppSettings["ExternalMailTo"]);

            return new JsonResult();
        }

    }
}
