using Simple_Stocks.Models;
using Simple_Stocks.Services;
using System.ComponentModel.DataAnnotations;

namespace Simple_Stocks.Utils
{
    public class ValidateUserExistenceAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
           var userRepo = (IUserRepo)validationContext.GetService(typeof(IUserRepo));

           var userToCheck = userRepo.GetUserByUsername(value.ToString());

            if (userToCheck != null)
            {
                return ValidationResult.Success;
            }
            return new ValidationResult(ErrorMessage);
        }
    }
}
