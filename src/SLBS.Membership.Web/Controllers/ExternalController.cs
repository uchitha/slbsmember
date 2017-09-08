using SLBS.Membership.Web.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Mvc;

namespace SLBS.Membership.Web.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [System.Web.Http.Route("External")]
    public class ExternalController : ApiController
    {
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("SendMail")]
        public int SendMail()
        {
            ////Send emails
            var sender = new EmailSender(EnumMode.External);
            //await sender.SendExternalMail(message, subject, ConfigurationManager.AppSettings["ExternalMailTo"]);

            return 20;
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("SendMail")]
        public string SendMailGet()
        {
            ////Send emails
            var sender = new EmailSender(EnumMode.External);
            //await sender.SendExternalMail(message, subject, ConfigurationManager.AppSettings["ExternalMailTo"]);

            return "Send";
        }


        public string Get() { return "GET"; }
    }
}
