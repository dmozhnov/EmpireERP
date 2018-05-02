using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ERP.Utils.Mvc.Validators
{
    /// <summary>
    /// Указывает, что значение поля должно соответствовать регулярному выражению, если другое поле имеет необходимое значение
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class RegularExpressionByRadioButtonAttribute : ValidationAttribute, IClientValidatable
    {
        private readonly string fieldName;
        private string valueToCompare;
        private string pattern;
        RegularExpressionAttribute regularExpressionAttribute;

        /// <summary>
        /// Указывает, что значение поля должно соответствовать регулярному выражению, если другое поле имеет необходимое значение
        /// </summary>
        /// <param name="fieldName">Название "контроллирующего" поля модели и группы радиобаттонов на странице</param>
        /// <param name="valueToCompare">Значение, которое должно иметь "контроллирующее" поле</param>
        /// <param name="pattern">Регулярное выражение</param>        
        public RegularExpressionByRadioButtonAttribute(string fieldName, int valueToCompare, string pattern)           
        {   
            this.fieldName = fieldName;
            this.valueToCompare = valueToCompare.ToString();
            this.pattern = pattern;
            regularExpressionAttribute = new RegularExpressionAttribute(pattern);
        }      

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule()
            {
                ErrorMessage = this.FormatErrorMessage(valueToCompare.ToString()),
                ValidationType = "regexbyradiobutton"
            };

            rule.ValidationParameters.Add("radiobuttongroupname", fieldName);
            rule.ValidationParameters.Add("valuetocompare", valueToCompare);
            rule.ValidationParameters.Add("regex", pattern);           

            yield return rule;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var finalValue = validationContext.ObjectType.GetProperty(fieldName).GetValue(validationContext.ObjectInstance, null).ToString();
            if ( finalValue != valueToCompare || (regularExpressionAttribute.GetValidationResult(value, validationContext) == ValidationResult.Success))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(
               this.FormatErrorMessage(validationContext.DisplayName));
        }
    }
}
