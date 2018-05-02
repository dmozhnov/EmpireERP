using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ERP.Utils.Mvc.Validators
{
    /// <summary>
    /// Указывает, что значение поля должно быть >= некоторого другого поля модели
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class GreaterOrEqualByPropertyAttribute : ValidationAttribute, IClientValidatable
    {
        private readonly string _propertyToCompare;
        private decimal _valueToCompare;

        /// <summary>
        ///
        /// </summary>
        /// <param name="propertyToCompare">Название поля модели, с которым сравниваем</param>
        public GreaterOrEqualByPropertyAttribute(string propertyToCompare)
            : base("The {0} must be greater than or equal to {1}.")
        {
            this._propertyToCompare = propertyToCompare;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var model = ((ViewContext)context).ViewData.Model;
            string propertyValue = model.GetType().GetProperty(_propertyToCompare).GetValue(model, null).ToString();
            _valueToCompare = ValidationUtils.TryGetDecimal(propertyValue);

            var rule = new ModelClientValidationRule()
            {
                ErrorMessage = this.FormatErrorMessage(this._valueToCompare.ToString()),
                ValidationType = "greaterorequalbyproperty"
            };

            rule.ValidationParameters.Add("valuetocompare", this._valueToCompare);

            yield return rule;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (ValidationUtils.TryGetDecimal(value) >= this._valueToCompare)
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(
               this.FormatErrorMessage(validationContext.DisplayName));
        }
    }
}