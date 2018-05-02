using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ERP.Utils.Mvc.Validators
{
    /// <summary>
    /// Указывает, что значение поля должно быть больше константы
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class GreaterByConstAttribute : ValidationAttribute, IClientValidatable
    {
        private readonly int _constToCompare;
      
        /// <summary>
        ///
        /// </summary>
        /// <param name="constToCompare">Константа, с которой сравниваем</param>
        public GreaterByConstAttribute(int constToCompare)
            : base("The {0} must be greater than {1}.")
        {
            this._constToCompare = constToCompare;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule()
            {
                ErrorMessage = this.FormatErrorMessage(this._constToCompare.ToString()),
                ValidationType = "greaterbyconst"
            };

            rule.ValidationParameters.Add("consttocompare", this._constToCompare);

            yield return rule;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (ValidationUtils.TryGetDecimal(value) > this._constToCompare)
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(
               this.FormatErrorMessage(validationContext.DisplayName));
        }
    }
}