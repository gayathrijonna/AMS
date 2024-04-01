using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AMS.Models
{
    //CustomDateValidation Class
    public class CustomDateValidation:ValidationAttribute
    {
        //Validates wheather the entered date greater than the current date
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime dt = Convert.ToDateTime(value);
            DateTime t = DateTime.Today;           
            if (dt < t)
                return new ValidationResult("Date must be greater today's date");          
            else
                return ValidationResult.Success;
        }
    }
}