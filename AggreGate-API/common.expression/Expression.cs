using System;
using JavaCompatibility;

namespace com.tibbo.aggregate.common.expression
{
    public class Expression : ICloneable
    {
        public const String REFERENCE_START = "{";

        public const String REFERENCE_END = "}";

        private readonly String text;

        public Expression(String text)
        {
            this.text = text;
        }

        public String getText()
        {
            return text;
        }

        public override String ToString()
        {
            return text;
        }

        public override int GetHashCode()
        {
            const int prime = 31;
            var result = 1;
            result = prime*result + ((text == null) ? 0 : text.GetHashCode());
            return result;
        }

        public override bool Equals(Object obj)
        {
            if (this == obj)
            {
                return true;
            }
            if (obj == null)
            {
                return false;
            }
            if (GetType() != obj.GetType())
            {
                return false;
            }
            var other = (Expression) obj;
            if (text == null)
            {
                if (other.text != null)
                {
                    return false;
                }
            }
            else if (!text.Equals(other.text))
            {
                return false;
            }
            return true;
        }

        public object Clone()
        {
            try
            {
                return MemberwiseClone();
            }
            catch (CloneNotSupportedException ex)
            {
                throw new InvalidOperationException(ex.Message, ex);
            }
        }
    }
}