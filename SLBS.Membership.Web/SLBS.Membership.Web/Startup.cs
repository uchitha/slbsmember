﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using SLBS.Membership.Domain;
using SLBS.Membership.Domain.Identity;
using SLBS.Membership.Web.App_Start;

[assembly: OwinStartup(typeof(SLBS.Membership.Web.Startup))]

namespace SLBS.Membership.Web
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
            app.CreatePerOwinContext<SlsbsContext>(SlsbsContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            app.CreatePerOwinContext<RoleManager<AppRole>>((options, context) =>
              new RoleManager<AppRole>(
                  new RoleStore<AppRole>(context.Get<SlsbsContext>())));



            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login")
            });

            CreateDefaultRolesAndUsers();
        }

        private void CreateDefaultRolesAndUsers()
        {
            var context = new SlsbsContext();
            var roleManager = new RoleManager<AppRole>(new RoleStore<AppRole>(context));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            var defaultRoles = new List<string> {"Admin", "Viewer", "BSEditor", "DSEditor", "Sender"};

            defaultRoles.ForEach(r =>
            {
                if (!roleManager.RoleExists(r))
                {
                    CreateRole(roleManager, r);
                }
            });

            if (!userManager.Users.Any(u => u.UserName == "admin"))
            {
               CreateUser(userManager,"admin","!Admin123","admin@slsbsmembership.net", new List<string>{ "Admin" });
            }

            if (!userManager.Users.Any(u => u.UserName == "chandana"))
            {
                CreateUser(userManager, "chandana", "!Chandana123", "treasurer@slsbsmembership.net",new List<string> {"BSEditor", "DSEditor", "Sender"});
            }

            if (!userManager.Users.Any(u => u.UserName == "user"))
            {
                CreateUser(userManager, "user", "!User123", "user@slsbsmembership.net", new List<string> { "Viewer"});
            }

        }

        private void CreateRole(RoleManager<AppRole> roleManager, string roleName)
        {
            var role = new AppRole();
            role.Name = roleName;
            roleManager.Create(role);   

        }

        private void CreateUser(UserManager<ApplicationUser> userManager, string userName, string password, string email, List<string> roleNames)
        {
            var user = new ApplicationUser();
            user.UserName = userName;
            user.Email = email;
            var chkUser = userManager.Create(user, password);

            if (chkUser.Succeeded)
            {
                roleNames.ForEach(r => userManager.AddToRole(user.Id, r));
            }
        }

        
    }
}