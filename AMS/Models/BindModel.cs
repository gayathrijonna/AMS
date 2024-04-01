using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMS.Models
{
    public class BindModel
    {
        public tblHanger Hanger { get; set; }
        public tblManager Manager { get; set; }

        public tblAddress Address { get; set; }
    }
}