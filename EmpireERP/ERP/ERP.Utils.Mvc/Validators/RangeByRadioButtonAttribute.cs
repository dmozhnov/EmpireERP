using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ERP.Utils.Mvc.Validators
{
    /// <summary>
    /// Указывает, что значение поля должно входить в указанный интервал, если другое поле имеет необходимое значение
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class RangeByRadioButtonAttribute : ValidationAttribute, IClientValidatable
    {
        private readonly string fieldName;
        private string valueToCompare;
        private object minimum;
        private object maximum;
        RangeAttribute rangeAttribute;

        private RangeByRadioButtonAttribute(string fieldName, int valueToCompare, object minimum, object maximum)
        {
            this.fieldName = fieldName;
            this.valueToCompare = valueToCompare.ToString();
            this.minimum = minimum;
            this.maximum = maximum;
        }


        /// <summary>
        /// Указывает, что значение поля должно входить в указанный интервал, если другое поле имеет необходимое значение
        /// </summary>
        /// <param name="fieldName">Название "контроллирующего" поля модели и группы радиобаттонов на странице</param>
        /// <param name="valueToCompare">Значение, которое должно иметь "контроллирующее" поле</param>
        /// <param name="minimum">Нижний предел интервала</param>
        /// <param name="maximum">Верхний предел интервала</param>
        public RangeByRadioButtonAttribute(string fieldName, int valueToCompare, int minimum, int maximum)
            : this(fieldName, valueToCompare, (object)minimum, (object)maximum)            
        {
            rangeAttribute = new RangeAttribute(minimum, maximum);            
        }

        /// <summary>
        /// Указывает, что значение поля должно входить в указанный интервал, если другое поле имеет необходимое значение
        /// </summary>
        /// <param name="fieldName">Название "контроллирующего" поля модели и группы радиобаттонов на странице</param>
        /// <param name="valueToCompare">Значение, которое должно иметь "контроллирующее" поле</param>
        /// <param name="minimum">Нижний предел интервала</param>
        /// <param name="maximum">Верхний предел интервала</param>
        public RangeByRadioButtonAttribute(string fieldName, int valueToCompare, double minimum, double maximum)
            : this(fieldName, valueToCompare, (object)minimum, (object)maximum)   
        {
            rangeAttribute = new RangeAttribute(minimum, maximum);
        }

        /// <summary>
        /// Указывает, что значение поля должно входить в указанный интервал, если другое поле имеет необходимое значение
        /// </summary>
        /// <param name="fieldName">Название "контроллирующего" поля модели и группы радиобаттонов на странице</param>
        /// <param name="valueToCompare">Значение, которое должно иметь "контроллирующее" поле</param>
        /// <param name="Type">Тип поля</param>
        /// <param name="minimum">Нижний предел интервала</param>
        /// <param name="maximum">Верхний предел интервала</param>
        public RangeByRadioButtonAttribute(string fieldName, int valueToCompare, Type type, string minimum, string maximum)
            : this(fieldName, valueToCompare, (object)minimum, (object)maximum)     
        {
            rangeAttribute = new RangeAttribute(type, minimum, maximum);
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule()
            {
                ErrorMessage = this.FormatErrorMessage(valueToCompare.ToString()),
                ValidationType = "rangebyradiobutton"
            };

            rule.ValidationParameters.Add("radiobuttongroupname", fieldName);
            rule.ValidationParameters.Add("valuetocompare", valueToCompare);
            rule.ValidationParameters.Add("min", minimum);
            rule.ValidationParameters.Add("max", maximum);

            yield return rule;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            
            var finalValue = validationContext.ObjectType.GetProperty(fieldName).GetValue(validationContext.ObjectInstance, null).ToString();
            if ( finalValue != valueToCompare || (rangeAttribute.GetValidationResult((value as String).Replace('.', ','), validationContext) == ValidationResult.Success))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(
               this.FormatErrorMessage(validationContext.DisplayName));
        }
    }
}
