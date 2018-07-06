using JavaCompatibility;

namespace com.tibbo.aggregate.common.datatable.validator
{
    using System;

    using com.tibbo.aggregate.common.data;
    using com.tibbo.aggregate.common.util;

    using context;

    public class LimitsValidator : AbstractFieldValidator
    {
        private const char MIN_MAX_SEPARATOR = ' ';

        private IComparable min;
        private IComparable max;

        public LimitsValidator(FieldFormat fieldFormat, string source)
        {
            var minMax = StringUtils.split(source, MIN_MAX_SEPARATOR);

            if (fieldFormat.getType() == FieldFormat.DATA_FIELD ||
                fieldFormat.getType() == FieldFormat.STRING_FIELD)
            {
                if (minMax.Count > 1)
                {
                    this.min = int.Parse(minMax[0]);
                    this.max = int.Parse(minMax[1]);
                }
                else
                {
                    this.max = int.Parse(minMax[0]);
                }
            }
            else
            {
                if (minMax.Count > 1)
                {
                    this.min = (IComparable) fieldFormat.valueFromString(minMax[0]);
                    this.max = (IComparable) fieldFormat.valueFromString(minMax[1]);
                }
                else
                {
                    this.max = (IComparable) fieldFormat.valueFromString(minMax[0]);
                }
            }
        }

        public LimitsValidator(IComparable min, IComparable max)
        {
            setLimits(min, max);
        }

        protected void setLimits(IComparable min, IComparable max)
        {
            if (min != null && max != null && !min.GetType().Equals((max.GetType())))
                Logger.getLogger(Log.DATATABLE).error("'min' and 'max' Limits Validator parameters should be the same type");

            this.min = min;
            this.max = max;
        }

        public override bool shouldEncode()
        {
            return true;
        }

        public override char? getType()
        {
            return FieldFormat.VALIDATOR_LIMITS;
        }

        public override string encode()
        {
            return this.min != null ? this.min.ToString() + MIN_MAX_SEPARATOR + this.max : this.max.ToString();
        }

        public override object validate(Context context, ContextManager contextManager, CallerController<CallerData> caller, object value)
        {
            if (value == null)
            {
                return value;
            }

            if (value is Data)
            {
                var data = value as Data;

                if (data.getData() != null)
                {
                    IComparable size = data.getData().Length;
                    this.compare(size, null, null);
                }
            }
            else if (value is string)
            {
                this.compare(value.ToString().Length, Cres.get().getString("dtValueTooShort"), Cres.get().getString("dtValueTooLong"));
            }
            else
            {
                if (!(value is IComparable))
                {
                    throw new ValidationException(Cres.get().getString("dtValueNotComparable"));
                }

                var cv = (IComparable)value;

                this.compare(cv, null, null);
            }

            return value;
        }

        private void compare(IComparable cv, string smallMessage, string bigMessage)
        {
            if (this.min != null)
            {
                if (cv.CompareTo(this.min) < 0)
                {
                    throw new ValidationException(string.Format(smallMessage ?? Cres.get().getString("dtValueTooSmall"), cv, min));
                }
            }

            if (cv.CompareTo(max) > 0)
            {
                throw new ValidationException(string.Format(bigMessage ?? Cres.get().getString("dtValueTooBig"), cv, max));
            }
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
            {
                return true;
            }

            if (!base.Equals(obj))
            {
                return false;
            }

            if (obj == null)
            {
                return false;
            }

            if (this.GetType() != obj.GetType())
            {
                return false;
            }

            var other = (LimitsValidator)obj;

            if (this.max == null)
            {
                if (other.max != null)
                {
                    return false;
                }
            }
            else if (!this.max.Equals(other.max))
            {
                return false;
            }
            if (this.min == null)
            {
                if (other.min != null)
                {
                    return false;
                }
            }
            else if (!this.min.Equals(other.min))
            {
                return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            const int Prime = 31;
            var result = 1;
            result = (Prime * result) + ((this.max == null) ? 0 : this.max.GetHashCode());
            result = (Prime * result) + ((this.min == null) ? 0 : this.min.GetHashCode());
            return result;
        }
    }
}