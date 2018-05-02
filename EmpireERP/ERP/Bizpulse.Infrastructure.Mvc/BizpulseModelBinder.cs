using System.ComponentModel;
using System.Web.Mvc;

namespace Bizpulse.Infrastructure.Mvc
{
    /// <summary>
    /// Обрезает пробелы в начале и конце строки
    /// </summary>
    public class BizpulseModelBinder : DefaultModelBinder
    {
        protected override void SetProperty(ControllerContext controllerContext, ModelBindingContext bindingContext, 
            PropertyDescriptor propertyDescriptor, object value)
        {
            if (propertyDescriptor.PropertyType == typeof(string))
            {
                var stringValue = value as string;
                if (!string.IsNullOrEmpty(stringValue))
                    stringValue = stringValue.Trim();

                value = stringValue;
            }

            base.SetProperty(controllerContext, bindingContext, propertyDescriptor, value);
        }
    }
}