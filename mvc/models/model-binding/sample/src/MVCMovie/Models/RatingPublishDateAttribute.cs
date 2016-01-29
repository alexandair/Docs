using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MVCMovie.Models
{
    public class RateWhenPublishedAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // None, G, PG, PG-13, and R are the options
            Movie movie = (Movie)validationContext.ObjectInstance;

            // Movies may not have ratings until they are published, so if the release date is in the future
            // we must verify the rating is set to "None" or nothing ("").

            if (movie.ReleaseDate.Date > DateTime.Now)
            {
                if (movie.Rating == null)
                {
                    return ValidationResult.Success;
                }

                if (movie.Rating.ToUpper() == "NONE")
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult("Movies that are not yet published may not have ratings.");
                }

            }
            else
            {
                // if it's a published movie, it must have a rating
                if (movie.Rating != null)
                {
                    switch (movie.Rating.ToUpper())
                    {
                        case "G":
                        case "PG":
                        case "PG-13":
                        case "R":
                            return ValidationResult.Success;
                        default:
                            return new ValidationResult("Published movies must have a rating of 'G', 'PG', 'PG-13', or 'R'.");
                    }
                }
                else
                {
                    return new ValidationResult("Published movies must have a rating of 'G', 'PG', 'PG-13', or 'R'.");
                }

            }
        }

    }
}
