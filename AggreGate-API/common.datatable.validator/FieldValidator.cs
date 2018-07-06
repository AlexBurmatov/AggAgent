using System;

namespace com.tibbo.aggregate.common.datatable.validator
{
    using com.tibbo.aggregate.common.context;

    public interface FieldValidator
    {
        bool shouldEncode();

        char? getType();

        string encode();

        object validate(object value);

        object validate(Context context, ContextManager contextManager, CallerController<CallerData> caller, object value);
    }
}