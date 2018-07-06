using com.tibbo.aggregate.common.datatable;

namespace com.tibbo.aggregate.common.context
{
    public interface VariableGetter
    {
        DataTable get(Context con, VariableDefinition def, CallerController<CallerData> caller,
                      RequestController<RequestData> request);
    }

    public class DelegatedVariableGetter : VariableGetter
    {
        private readonly GetterDelegate getter;

        public delegate DataTable GetterDelegate( Context con, VariableDefinition def, CallerController<CallerData> caller, RequestController<RequestData> request);

        public DelegatedVariableGetter(GetterDelegate aDelegate)
        {
            getter = aDelegate;
        }


        public DataTable get(Context con, VariableDefinition def, CallerController<CallerData> caller,
                             RequestController<RequestData> request)
        {
            return getter.Invoke(con, def, caller, request);
        }
    }
}