using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AMS.Models
{
    public class Dates
    {
        DateTime startdate;
        DateTime enddate;
        [Required(ErrorMessage = "Please select start date")]
        [DataType(DataType.Date)]
        public DateTime Startdate { get => startdate; set => startdate = value; }
        [Required(ErrorMessage = "Please select end date")]
        [DataType(DataType.Date)]

        public DateTime Enddate { get => enddate; set => enddate = value; }
    }
}
