using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MVCMovie.Models
{
    public class VintageAttribute : ValidationAttribute
    {
        private int _year;
        public VintageAttribute(int Year)
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
    }

    

}
