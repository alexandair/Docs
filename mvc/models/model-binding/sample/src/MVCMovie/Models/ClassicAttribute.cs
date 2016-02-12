using Microsoft.AspNet.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MVCMovie.Models
{
    public class ClassicAttribute : ValidationAttribute, IClientModelValidator
    {
        private int _year;
        public ClassicAttribute(int Year)
        {
            _year = Year;                
        }

       protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            Movie movie = (Movie)validationContext.ObjectInstance;

            if (movie.Genre == Genre.Vintage) {
                if (movie.ReleaseDate.Year < this._year) {
                    return ValidationResult.Success;
                }
                else {
                    return new ValidationResult("Vintage movies must have a release year earlier than " + this._year);
                }                
            }
            return ValidationResult.Success;
        }

        IEnumerable<ModelClientValidationRule> IClientModelValidator.GetClientValidationRules(ClientModelValidationContext context)
        {
            var rule = new ModelClientValidationRule("required", "This field is required");
            //var r = new ModelClientValidationRequiredRule("Required!");
            //var r = new ModelClientValidationRangeRule("min max message", 1, 10);
            yield return rule;
        }

        
    }


}

    

