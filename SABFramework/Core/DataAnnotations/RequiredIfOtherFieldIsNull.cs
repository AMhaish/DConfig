using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace System.ComponentModel.DataAnnotations
{
    public class RequiredIfOtherFieldIsNull : ValidationAttribute, IClientValidatable
    {
        private string _otherProperty;
        public RequiredIfOtherFieldIsNull(string otherProperty)
        {
            _otherProperty = otherProperty;
        }

        protected override ValidationResult IsValid(Object value, ValidationContext validationContext)
        {
            var Myproperty = validationContext.ObjectType.GetProperty(_otherProperty);
            if (Myproperty == null || !Myproperty.PropertyType.IsClass)
            {
                return new ValidationResult(String.Format(System.Globalization.CultureInfo.CurrentCulture, "'Unknown/Invalid type' property {0}", new { _otherProperty }));
            }
            object otherPropertyValue = Myproperty.GetValue(validationContext.ObjectInstance, null);
            if (otherPropertyValue == null)
            {
                if (value == null)
                {
                    return new ValidationResult(String.Format(System.Globalization.CultureInfo.CurrentCulture, FormatErrorMessage(validationContext.DisplayName), new { _otherProperty }));
                }
            }
            return null;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            List<ModelClientValidationRule> rr = new List<ModelClientValidationRule>();
            ModelClientValidationRule rule = new ModelClientValidationRule()
            {
                ErrorMessage = FormatErrorMessage(metadata.GetDisplayName()),
                ValidationType = "requiredif"
            };
            rule.ValidationParameters.Add("other", _otherProperty);
            rr.Add(rule);
            return rr;
        }
    }
}
