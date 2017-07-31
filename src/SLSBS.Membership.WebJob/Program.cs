using Microsoft.Azure.WebJobs;

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

        }

       


    }
}
