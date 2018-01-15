using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;

namespace mblyakher_budgeter.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string TimeZone { get; set; }

        public ApplicationUser()
        {
            Budgets = new HashSet<Budget>();
            Accounts = new HashSet<Account>();
            BudgetItems = new HashSet<BudgetItem>();
            Transactions = new HashSet<Transaction>();
            Invites = new HashSet<Invite>();
        }

        public virtual ICollection<Budget> Budgets { get; set; }
        public virtual ICollection<Account> Accounts { get; set; }
        public virtual ICollection<BudgetItem> BudgetItems { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
        public virtual ICollection<Invite> Invites { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false) // looking up in the web.config connection string, using default connection
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
        // Entity Framework is the ORM b/w Database and application. Local host knows where sql is
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<BudgetItem> BudgetItems { get; set; }
        public DbSet<BudgetItemType> BudgetItemTypes { get; set; }
        public DbSet<Household> Households { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<TransactionType> TransactionTypes { get; set; }
        public DbSet<Invite> Invites { get; set; }
    }
}