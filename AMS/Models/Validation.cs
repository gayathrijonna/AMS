using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AMS.Models
{
    public class Validation
    {
        public tblPlane Plane { get; set; }
        public tblPilot Pilot { get; set; }
        public tblAddress Address { get; set; }
    }
}