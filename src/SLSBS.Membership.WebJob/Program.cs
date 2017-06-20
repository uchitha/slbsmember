using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using Microsoft.WindowsAzure.Storage.Queue;

namespace SLSBS.Membership.WebJobEmail
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new JobHostConfiguration();
            config.UseDevelopmentSettings();

            //if (config.IsDevelopment)
            //{
            //    config.UseDevelopmentSettings();
            //}

            JobHost host = new JobHost();
            host.RunAndBlock();
        }

       


    }
}
