using Microsoft.AspNet.Mvc.ModelBinding;
using Microsoft.AspNet.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MVCMovie.Models
{
    public class FamilyMovieAttribute : ValidationAttribute, IClientModelValidator
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            Movie movie = (Movie)validationContext.ObjectInstance;

            if (movie.Audience == "G")
            {
                if (movie.Genre != Genre.Family)
                {
                    return new ValidationResult("Movies for the G [General] audience must be Family movies");
                }
            }
            return ValidationResult.Success;
        }
        IEnumerable<ModelClientValidationRule> IClientModelValidator.GetClientValidationRules(ClientModelValidationContext context)
        {          
            var rule = new ModelClientValidationRule("familymovie", "Movies for the G [General] audience must be Family movies");
            rule.ValidationParameters.Add("genre", "7");
            rule.ValidationParameters.Add("audience", "G");
            yield return rule;            
        }
    }
}
