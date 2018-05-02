using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ERP.Utils.Mvc.Validators
{
    /// <summary>
    /// Указывает, что поле требуемо к заполнению в случае, если другое поле имеет необходимое значение
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class RequiredByRadioButtonAttribute : ValidationAttribute, IClientValidatable
    {
        private readonly string fieldName;
        private string valueToCompare;

        /// <summary>
        /// Указывает, что поле требуемо к заполнению в случае, если другое поле имеет необходимое значение
        /// </summary>
        /// <param name="fieldName">Название "контроллирующего" поля модели и группы радиобаттонов на странице</param>
        /// <param name="valueToCompare">Значение, которое должно иметь "контроллирующее" поле</param>
        public RequiredByRadioButtonAttribute(string fieldName, int valueToCompare) : base("The {0} field is required.")
        {
            this.fieldName = fieldName;
            this.valueToCompare = valueToCompare.ToString();
        }


        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule()
            {
                ErrorMessage = this.FormatErrorMessage(valueToCompare.ToString()),
                ValidationType = "requiredbyradiobutton"
            };

            rule.ValidationParameters.Add("radiobuttongroupname", fieldName);
            rule.ValidationParameters.Add("valuetocompare", valueToCompare);

            yield return rule;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var finalValue = validationContext.ObjectType.GetProperty(fieldName).GetValue(validationContext.ObjectInstance, null).ToString();
            if ( finalValue != valueToCompare || value != null)
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(
               this.FormatErrorMessage(validationContext.DisplayName));
        }
    }
}
