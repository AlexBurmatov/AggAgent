using System;
using com.tibbo.aggregate.common.util;

namespace com.tibbo.aggregate.common.datatable.validator
{
    using context;

    public abstract class AbstractFieldValidator : FieldValidator
    {
        public virtual bool shouldEncode()
        {
            return false;
        }

        public virtual char? getType()
        {
            return null;
        }

        public virtual String encode()
        {
            return "";
        }

        public object validate(object value)
        {
            return this.validate(null, null, null, value);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (!(obj is FieldValidator))
            {
                return false;
            }

            return Util.equals(this.getType(), ((FieldValidator) obj).getType());
        }

        public object clone()
        {
            try
            {
                return MemberwiseClone();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("", ex);
            }
        }

        public override int GetHashCode()
        {
            const int prime = 31;
            int result = 1;

            if (getType() != null)
                result = prime * result + (char)getType();

            result = prime * result + encode().GetHashCode();
            result = prime * result + (shouldEncode() ? 1231 : 1237);

            return result;
        }

        public abstract object validate(Context context, ContextManager contextManager, CallerController<CallerData> caller, object value);
    }
}