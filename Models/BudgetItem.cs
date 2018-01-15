using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mblyakher_budgeter.Models
{
    public class BudgetItem
    {
        public int Id { get; set; }
        public int Amount { get; set; }

        public int BudgetItemTypeId { get; set; }
        public int BudgetId { get; set; }

        public virtual BudgetItemType BudgetItemType { get; set; }
        public virtual Budget Budget { get; set; }
    }
}