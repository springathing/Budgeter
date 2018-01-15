using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mblyakher_budgeter.Models
{
    public class HouseholdViewModel
    {
        public Household household { get; set; }
        public List<Transaction> MyTransactions { get; set; }
        public List<Account> MyAccounts { get; set; }
        public List<BudgetItem> MyBudgetItems { get; set; }
    }
}