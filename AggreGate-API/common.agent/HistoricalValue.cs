using System;
using com.tibbo.aggregate.common.datatable;

namespace com.tibbo.aggregate.common.agent
{
    public class HistoricalValue
    {
        private String variable;
        private DateTime timestamp;
        private DataTable value;

        public HistoricalValue(String variable, DateTime timestamp, DataTable value)
        {
            this.variable = variable;
            this.timestamp = timestamp;
            this.value = value;
        }

        public String getVariable()
        {
            return variable;
        }

        public void setVariable(String variable)
        {
            this.variable = variable;
        }

        public DateTime getTimestamp()
        {
            return timestamp;
        }

        public void setTimestamp(DateTime timestamp)
        {
            this.timestamp = timestamp;
        }

        public DataTable getValue()
        {
            return value;
        }

        public void setValue(DataTable value)
        {
            this.value = value;
        }
    }
}
