using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace IamUsingIt.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public ICollection<ApplicationUserRole> UserRoles { get; set; }
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public ICollection<Reservation> Reservations { get; set; } 
    }

    // ReSharper disable once ClassNeverInstantiated.Global
    public class ApplicationUserRole : IdentityUserRole
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public ApplicationRole Role { get; set; }
    }

    public sealed class ApplicationRole : IdentityRole
    {
        public ApplicationRole()
        { }

        // ReSharper disable once UnusedParameter.Local
        public ApplicationRole(string name, string description)
            : base(name)
        {
        }
    }
}