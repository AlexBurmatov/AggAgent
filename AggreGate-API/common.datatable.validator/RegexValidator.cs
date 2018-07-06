using System;
using System.Text.RegularExpressions;
using JavaCompatibility;

namespace com.tibbo.aggregate.common.datatable.validator
{
    using com.tibbo.aggregate.common.context;

    public class RegexValidator : AbstractFieldValidator
    {
        private const String SEPARATOR = "^^";

        private readonly String regex;
        private readonly String message;

        public RegexValidator(String source)
        {
            var parts = source.Split(new String[] { SEPARATOR }, StringSplitOptions.None);

            regex = parts[0];

            if (parts.Length > 1)
            {
                message = parts[1];
            }
        }

        public RegexValidator(String regex, String message)
        {
            this.regex = regex;
            this.message = message;
        }

        public override bool shouldEncode()
        {
            return true;
        }

        public override String encode()
        {
            return regex + (message != null ? SEPARATOR + message : "");
        }

        public Char? getType()
        {
            return FieldFormat.VALIDATOR_REGEX;
        }

        public override object validate(Context context, ContextManager contextManager, CallerController<CallerData> caller, object value)
        {
            Regex reg;
            try
            {
                reg = new Regex(regex);
            }
            catch (ArgumentException ex)
            {
                throw new ValidationException(ex.Message, ex);
            }

            if (value != null && !reg.IsMatch(value.ToString()))
            {
                throw new ValidationException(this.message ?? string.Format(Cres.get().getString("dtValueDoesNotMatchPattern"), value, this.regex));
            }

            return value;
        }

        public override bool Equals(Object obj)
        {
            if (this == obj)
            {
                return true;
            }
            if (!base.Equals(obj))
            {
                return false;
            }
            if (GetType() != obj.GetType())
            {
                return false;
            }
            var other = (RegexValidator) obj;
            if (message == null)
            {
                if (other.message != null)
                {
                    return false;
                }
            }
            else if (!message.Equals(other.message))
            {
                return false;
            }
            if (regex == null)
            {
                if (other.regex != null)
                {
                    return false;
                }
            }
            else if (!regex.Equals(other.regex))
            {
                return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            const int prime = 31;
            var result = 1;
            result = prime*result + ((message == null) ? 0 : message.GetHashCode());
            result = prime*result + ((regex == null) ? 0 : regex.GetHashCode());
            return result;
        }
    }
}