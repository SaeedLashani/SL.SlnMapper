using System;
using System.ComponentModel.DataAnnotations;

namespace SL.Domain.Validation
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class GreaterThanZeroAttribute : ValidationAttribute
    {
        public GreaterThanZeroAttribute() : base("The {0} field must be greater than zero.")
        {
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // Check if value is null or not an integer
            if (value == null || !(value is int intValue))
            {
                return new ValidationResult("A valid integer value is required.");
            }

            // Check if the integer is greater than zero
            if (intValue <= 0)
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }

            return ValidationResult.Success;
        }
    }
}
