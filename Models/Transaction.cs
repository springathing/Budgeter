using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace mblyakher_budgeter.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public string Description { get; set; }
        [DisplayFormat(DataFormatString = "{0:M/d/yyyy h:mm tt}")]
        public DateTimeOffset CreatedDate { get; set; }
        public decimal Amount { get; set; }

        public int TransactionTypeId { get; set; }
        public int AccountId { get; set; }
        public int MadeById { get; set; }

        public virtual ApplicationUser MadeBy { get; set; }
        public virtual TransactionType TransactionType { get; set; }
        public virtual Account Account { get; set; }
    }
}