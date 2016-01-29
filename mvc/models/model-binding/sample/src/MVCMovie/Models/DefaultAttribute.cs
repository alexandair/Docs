using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MVCMovie.Models
{
    public class DefaultAttribute : ValidationAttribute
    {
        private DataType _type;
        private object _val;
        public DefaultAttribute(string defaultValue)
        {
            _val = defaultValue;
        
            //switch (type.ToString())
            //{
            //    case "Currency":
            //        break;
            //    case "":
            //        break;
            //    default:
            //        break;
            //}
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (validationContext.MemberName == "Price") { }
            if (validationContext.MemberName == "ReleaseDate") {
                if (value == null) { value = 5.99;
                    return ValidationResult.Success;
                }
            }
            return ValidationResult.Success;
        }
    }
}

