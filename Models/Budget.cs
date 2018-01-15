using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mblyakher_budgeter.Models
{
    public class Budget
    {
        public Budget()
        {
            BudgetItems = new HashSet<BudgetItem>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Amount { get; set; }

        public int HouseholdId { get; set; }

        public virtual Household Household { get; set; }
        public virtual ICollection<BudgetItem> BudgetItems { get; set; }
    }
}