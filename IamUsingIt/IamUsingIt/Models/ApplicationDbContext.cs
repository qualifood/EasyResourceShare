using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;

namespace IamUsingIt.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {

        public ApplicationDbContext()
            : base("Sequelizer", throwIfV1Schema: false)
        {
        }

        public ApplicationDbContext(string connectionstring)
            : base(connectionstring, throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            var result = new ApplicationDbContext();
            return result;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            if (modelBuilder == null)
            {
                throw new ArgumentNullException(nameof(modelBuilder));
            }

            base.OnModelCreating(modelBuilder);

            //Defining the keys and relations

            //Identity stuff
            modelBuilder.Entity<ApplicationUser>().ToTable("AspNetUsers");
            modelBuilder.Entity<ApplicationRole>().HasKey<string>(r => r.Id).ToTable("AspNetRoles");
            modelBuilder.Entity<ApplicationUser>().HasMany(u => u.UserRoles);
            modelBuilder.Entity<ApplicationUserRole>().HasKey(r => new {r.UserId, r.RoleId }).ToTable("AspNetUserRoles");
            
            //Entities
            modelBuilder.Entity<Reservation>().HasKey(r => r.ReservationId).ToTable("Reservations");
            modelBuilder.Entity<Reservation>()
                .HasRequired(r => r.Resource)
                .WithMany(r => r.Reservations)
                .HasForeignKey(r => r.ResourceId);
            modelBuilder.Entity<Reservation>()
                .HasRequired(r => r.User)
                .WithMany(u => u.Reservations)
                .HasForeignKey(r => r.UserId);
        }

        public bool Seed(ApplicationDbContext context)
        {
#if DEBUG
            ApplicationRoleManager roleManager = new ApplicationRoleManager(new RoleStore<ApplicationRole>(context));

            var success = CreateRole(roleManager, "Admin", "Global Access");
            if (!success) return false;

            success = CreateRole(roleManager, "User", "Restricted to business domain activity");
            if (!success) return false;

            ApplicationUserManager userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));

            //admin
            ApplicationUser adminUser = new ApplicationUser
            {
                UserName = "admin",
                Email = "none@none.com"
            };
            userManager.Create(adminUser, "Demo123!");
            success=AddUserToRole(userManager, adminUser.Id, "Admin");

            //demoUser1
            var user = new ApplicationUser
            {
                UserName = "demoUser1",
                Email = "none@none.com"
            };
            userManager.Create(user, "Demo123!");
            success = success && AddUserToRole(userManager, user.Id, "User");

            //demoUser2
            var user2 = new ApplicationUser
            {
                UserName = "demoUser2",
                Email = "none@none.com"
            };
            userManager.Create(user2, "Demo123!");
            success = success && AddUserToRole(userManager, user2.Id, "User");

            //DemoResource1
            var resource = new Resource();
            resource.Name = "DemoResource1";
            context.Resources.Add(resource);

            //DemoResource2
            var resource2 = new Resource();
            resource2.Name = "DemoResource2";
            context.Resources.Add(resource2);

            //DemoReservation
            var reservation = new Reservation()
            {
                Resource = resource,
                Begin = DateTime.Now,
                End = DateTime.Now.AddHours(3),
                User = user
            };
            context.Reservations.Add(reservation);
            context.SaveChanges();

            return success;
#endif
#pragma warning disable 162
            // ReSharper disable once HeuristicUnreachableCode
            return true;
#pragma warning restore 162
        }

        public bool RoleExists(ApplicationRoleManager roleManager, string name)
        {
            return roleManager.RoleExists(name);
        }

        public bool CreateRole(ApplicationRoleManager roleManager, string name, string description = "")
        {
            var idResult = roleManager.Create(new ApplicationRole(name, description));
            return idResult.Succeeded;
        }

        public bool AddUserToRole(ApplicationUserManager userManager, string userId, string roleName)
        {
            var idResult = userManager.AddToRole(userId, roleName);
            return idResult.Succeeded;
        }

        public void ClearUserRoles(ApplicationUserManager userManager, string userId)
        {
            var user = userManager.FindById(userId);
            var currentRoles = new List<IdentityUserRole>();

            currentRoles.AddRange(user.UserRoles);
            foreach (var identityUserRole in currentRoles)
            {
                var role = (ApplicationUserRole) identityUserRole;
                userManager.RemoveFromRole(userId, role.Role.Name);
            }
        }

        public void RemoveFromRole(ApplicationUserManager userManager, string userId, string roleName)
        {
            userManager.RemoveFromRole(userId, roleName);
        }

        public void DeleteRole(ApplicationDbContext context, ApplicationUserManager userManager, string roleId)
        {
            var roleUsers = context.Users.Where(u => u.UserRoles.Any(r => r.RoleId == roleId));
            var role = context.Roles.Find(roleId);

            foreach (var user in roleUsers)
            {
                RemoveFromRole(userManager, user.Id, role.Name);
            }
            context.Roles.Remove(role);
            context.SaveChanges();
        }

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public DbSet<Resource> Resources { get; set; }

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public DbSet<Reservation> Reservations { get; set; }
    }


    /// <summary>
    /// Context Initializer
    /// </summary>
    public class DropCreateAlwaysInitializer : DropCreateDatabaseAlways<ApplicationDbContext>
    {
        public override void InitializeDatabase(ApplicationDbContext context)
        {
#if DEBUG
            context.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction
                , $"ALTER DATABASE [{context.Database.Connection.Database}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE");
#endif

            base.InitializeDatabase(context);
        }

        protected override void Seed(ApplicationDbContext context)
        {
            context.Seed(context);

            base.Seed(context);
        }
    }

    public class ApplicationRoleManager : RoleManager<ApplicationRole>
    {
        public ApplicationRoleManager(IRoleStore<ApplicationRole, string> roleStore)
            : base(roleStore)
        {
        }

        public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
        {
            return new ApplicationRoleManager(new RoleStore<ApplicationRole>(context.Get<ApplicationDbContext>()));
        }
    }
}