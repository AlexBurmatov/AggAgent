using System;

namespace com.tibbo.aggregate.common.datatable
{
    public class QueryCondition
    {
        //Operators
        public const int EQ = 1; // =
        public const int GT = 2; // >
        public const int LT = 4; // <
        public const int NE = 8; // !=
        public const int GE = GT | EQ; // >=
        public const int LE = LT | EQ; // <=

        private readonly String field;
        private readonly Object value;
        private readonly int oper = EQ;

        public QueryCondition(String field, Object value, int operInteger)
        {
            this.field = field;
            this.value = value;
            oper = operInteger;
        }

        public QueryCondition(String field, Object value) : this(field, value, EQ)
        {
        }

        public String getField()
        {
            return field;
        }

        public Object getValue()
        {
            return value;
        }

        public int getOperator()
        {
            return oper;
        }
    }
}