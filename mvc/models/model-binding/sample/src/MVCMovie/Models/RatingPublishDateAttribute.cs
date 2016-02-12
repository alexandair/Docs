using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MVCMovie.Models
{
    public class RateOnPublicationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // None, G, PG, PG-13, and R are the options
            Movie movie = (Movie)validationContext.ObjectInstance;

            // Movies may not have Audiences until they are published, so if the release date is in the future
            // we must verify the Audience is set to "None" or nothing ("").

            if (movie.ReleaseDate.Date > DateTime.Now)
            {
                if (movie.Audience == null)
                {
                    return ValidationResult.Success;
                }

                if (movie.Audience.ToUpper() == "NONE")
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult("Movies that are not yet published may not have Audiences.");
                }

            }
            else
            {
                // if it's a published movie, it must have a Audience
                if (movie.ReleaseDate != null && movie.ReleaseDate <= DateTime.Today)
                {
                    switch (movie.Audience.ToUpper())
                    {
                        case "G":
                        case "PG":
                        case "PG-13":
                        case "R":
                            return ValidationResult.Success;
                        default:
                            return new ValidationResult("Published movies must have a target audience of 'G', 'PG', 'PG-13', or 'R'.");
                    }
                }
                else
                {
                    return new ValidationResult("Published movies must have a target audience of 'G', 'PG', 'PG-13', or 'R'.");
                }

            }
        }

    }
}
