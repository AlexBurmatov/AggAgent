using com.tibbo.aggregate.common.datatable;

namespace com.tibbo.aggregate.common.context
{
    public interface VariableSetter
    {
        bool set(Context con, VariableDefinition def, CallerController<CallerData> caller, RequestController<RequestData> request, DataTable value);
    }

    public class DelegatedVariableSetter : VariableSetter
    {
        private readonly SetterDelegate setter;

        public delegate void SetterDelegate(Context con, VariableDefinition def, CallerController<CallerData> caller, RequestController<RequestData> request, DataTable value);

        public DelegatedVariableSetter(SetterDelegate aDelegate)
        {
            this.setter = aDelegate;
        }

        public virtual bool set(Context con, VariableDefinition def, CallerController<CallerData> caller, RequestController<RequestData> request, DataTable value)
        {
            this.setter.Invoke(con, def, caller, request, value);
            return true;
        }
    }
}