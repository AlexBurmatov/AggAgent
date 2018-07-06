namespace com.tibbo.aggregate.common.datatable
{
    public class NullableObject
    {

        public NullableObject(object value)
        {
            if (value == null)
            {
                return;
            }
            this.Value = value;
            this.isNull = false;
        }

        public object Value
        {
            get;
            private set;
        }

        public bool IsNull()
        {
            return this.isNull;
        }

        public override string ToString()
        {
            return (! this.isNull) ? this.Value.ToString() : null;
        }


        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return this.IsNull();
            }

            if (!(obj is NullableObject))
            {
                return false;
            }

            var no = (NullableObject)obj;

            if (this.IsNull())
            {
                return no.IsNull();
            }

            if (no.IsNull())
            {
                return false;
            }

            return this.Value.Equals(no.Value);
        }

        public override int GetHashCode()
        {
            if (this.isNull)
            {
                return 0;
            }

            var result = this.Value.GetHashCode();

            if (result >= 0)
            {
                result++;
            }

            return result;
        }

        //public static implicit operator object (NullableObject nullObject)        
        //{
        //    return nullObject.Value;
        //}
        //
        //public static implicit operator NullableObject(object item)
        //{
        //    return new NullableObject(item);
        //}

        private readonly bool isNull = true;

    }
}