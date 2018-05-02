using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using ERP.Utils;

namespace System.Web.Mvc
{
    /// <summary>
    /// Сравнение с другим полем модели
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class CompareWithPropertyAttribute : ValidationAttribute, IClientValidatable
    {
        private readonly string _propertyToCompare;
        private decimal _valueToCompare;
        private CompareOperationType _compareType;        

        /// <summary>
        ///
        /// </summary>
        /// <param name="propertyToCompare">Название поля модели, с которым сравниваем</param>
        public CompareWithPropertyAttribute(string propertyToCompare, CompareOperationType compareType)
            : base("")
        {
            this._propertyToCompare = propertyToCompare;
            this._compareType = compareType;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var model = ((ViewContext)context).ViewData.Model;

            var property = model.GetType().GetProperty(_propertyToCompare);

            if (property != null)
            {
                var propertyValue = property.GetValue(model, null);

                if (propertyValue != null)
                {
                    _valueToCompare = ValidationUtils.TryGetDecimal(propertyValue.ToString());

                    var rule = new ModelClientValidationRule()
                    {
                        ErrorMessage = this.FormatErrorMessage(this._valueToCompare.ToString()),
                        ValidationType = "comparewithproperty"
                    };

                    rule.ValidationParameters.Add("valuetocompare", this._valueToCompare);
                    rule.ValidationParameters.Add("comparetype", (byte)this._compareType);

                    yield return rule;
                }
            }

            yield return new ModelClientValidationRule() { ValidationType = "empty" }; //хак, чтобы в этом случае не валидировалось
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            bool success = false;
            var left = ValidationUtils.TryGetDecimal(value);
            var right = this._valueToCompare; 

            switch (this._compareType)
            {
                case CompareOperationType.Eq:
                    success = left == right; break;

                case CompareOperationType.Lt:
                    success = left < right; break;

                case CompareOperationType.Le:
                    success = left <= right; break;

                case CompareOperationType.Ge:
                    success = left >= right; break;

                case CompareOperationType.Gt:
                    success = left > right; break;
            }

            return success ? ValidationResult.Success : new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName));
        }
    }
}