using System;
using System.Collections.Generic;
using com.tibbo.aggregate.common.datatable;

namespace com.tibbo.aggregate.datatable.converter
{
    public class DefaultFormatConverter : AbstractFormatConverter
    {
        private readonly List<string> constructorArguments = new List<string>();

        public DefaultFormatConverter(Type valueClass, TableFormat format) : base(valueClass, format)
        {
        }

        public override FieldFormat createFieldFormat(string name)
        {
            return null;
        }

        public void addConstructorField(string field)
        {
            this.constructorArguments.Add(field);
        }
    }
}