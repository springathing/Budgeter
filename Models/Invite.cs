using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace mblyakher_budgeter.Models
{
    public class Invite
    {
        public int Id { get; set; }
        public string EmailTo { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTimeOffset Created { get; set; }
        public string Code { get; set; }

        public string EmailFromId { get; set; }
        public int HouseholdId { get; set; }

        public virtual ApplicationUser EmailFrom { get; set; }
        public virtual Household Household { get; set; }
    }
}