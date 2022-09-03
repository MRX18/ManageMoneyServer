using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace ManageMoneyServer.Validations
{
    public class CollectionLengthAttribute : ValidationAttribute
    {
        public readonly int MinLength = 0;
        public object MaxLength { get; set; }
        public bool IsRange { get; set; } = true;
        public CollectionLengthAttribute(int minLength = 0)
        {
            MinLength = minLength;
        }
        public override string FormatErrorMessage(string name)
        {
            int? maxLength = MaxLength as int?;
            string message = string.Empty;

            if(!string.IsNullOrEmpty(ErrorMessageString)) {
                if(IsRange)
                {
                    if(maxLength.HasValue)
                    {
                        message = string.Format(ErrorMessageString, name, maxLength.Value + 1, MinLength - 1);
                    } else
                    {
                        message = string.Format(ErrorMessageString, name, MinLength - 1);
                    }
                } else
                {
                    message = string.Format(ErrorMessageString, name, MinLength);
                }
            }

            return message;
        }
        public override bool IsValid(object? value)
        {
            int? maxLength = MaxLength as int?;
            if (value != null)
            {
                IList collection = value as IList;

                if(IsRange)
                {
                    bool result = true;

                    if (maxLength.HasValue)
                        result = collection.Count <= maxLength.Value;

                    return result && collection.Count >= MinLength;
                } else
                {
                    if (collection.Count == MinLength)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
