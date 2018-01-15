using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mblyakher_budgeter.Models
{
    public class Account
    {
        public Account() // hashset initialized by default constructor
        {
            Transactions = new HashSet<Transaction>();
        }
        // properties
        public int Id { get; set; }
        public string Name { get; set; }
        public int Balance { get; set; }
        // foreign keys
        public int HouseholdId { get; set; }
        // virtual properties
        public virtual Household Household { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}