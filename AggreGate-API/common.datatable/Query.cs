using System.Collections;
using System.Collections.Generic;

namespace com.tibbo.aggregate.common.datatable
{
    public class Query : IEnumerable<QueryCondition>
    {
        private readonly List<QueryCondition> conditions = new List<QueryCondition>();

        public Query(params QueryCondition[] conditions)
        {
            this.conditions.AddRange(conditions);
        }

        public List<QueryCondition> getConditions()
        {
            return conditions;
        }

        public void addCondition(QueryCondition condition)
        {
            conditions.Add(condition);
        }


        public IEnumerator<QueryCondition> GetEnumerator()
        {
            return conditions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}