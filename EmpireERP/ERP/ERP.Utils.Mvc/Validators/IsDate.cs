using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ERP.Utils.Mvc.Validators
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class IsDate : ValidationAttribute, IClientValidatable
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="regExp">Регулярное выражение для проверки даты</param>
        public IsDate()
            : base("")
        {
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var model = ((ViewContext)context).ViewData.Model;

            var rule = new ModelClientValidationRule()
            {
                ErrorMessage = this.FormatErrorMessage(""),
                ValidationType = "isdate"
            };

            yield return rule;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            string str = Convert.ToString(value);
            try
            {
                var dt = DateTime.Parse(str);
                if (dt != null)
                    return ValidationResult.Success;

                return new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName));
            }
            catch (Exception)
            {
                return new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName));
            }
        }
    }
}