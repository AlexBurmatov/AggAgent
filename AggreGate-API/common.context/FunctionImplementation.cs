using com.tibbo.aggregate.common.datatable;

namespace com.tibbo.aggregate.common.context
{
    public interface FunctionImplementation
    {
        DataTable execute(Context con, FunctionDefinition def, CallerController<CallerData> caller,
                          RequestController<RequestData> request, DataTable parameters);
    }

    public class DelegatedFunctionImplementation : FunctionImplementation
    {
        private readonly ExecutionDelegate executor;

        public delegate DataTable ExecutionDelegate(Context con, FunctionDefinition def, CallerController<CallerData> caller,
                          RequestController<RequestData> request, DataTable parameters);
        public DelegatedFunctionImplementation(ExecutionDelegate anExecutionDelegate)
        {
            executor = anExecutionDelegate;
        }

        public DataTable execute(Context con, FunctionDefinition def, CallerController<CallerData> caller,
                          RequestController<RequestData> request, DataTable parameters)
        {
            return executor.Invoke(con, def, caller, request, parameters);
        }

    }
}