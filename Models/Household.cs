using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mblyakher_budgeter.Models
{
    public class Household
    {
        public Household()
        {
            Budgets = new HashSet<Budget>();
            Accounts = new HashSet<Account>();
            Users = new HashSet<ApplicationUser>();
            Invites = new HashSet<Invite>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string OwnerId { get; set; }

        public virtual ApplicationUser Owner { get; set; }
        public virtual ICollection<Budget> Budgets { get; set; }
        public virtual ICollection<Account> Accounts { get; set; }
        public virtual ICollection<ApplicationUser> Users { get; set; }
        public virtual ICollection<Invite> Invites { get; set; }
    }
}