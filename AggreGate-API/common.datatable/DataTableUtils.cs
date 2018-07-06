using System;
using System.Collections.Generic;
using com.tibbo.aggregate.common.command;
using com.tibbo.aggregate.common.context;
using com.tibbo.aggregate.common.util;
using com.tibbo.aggregate.datatable.converter;
using JavaCompatibility;

namespace com.tibbo.aggregate.common.datatable
{
    using System.Text;

    using com.tibbo.aggregate.common.datatable.field;

    public class DataTableUtils
    {
        public const int PREVIEW_ICON_HEIGHT = 80;
        public const int PREVIEW_ICON_WIDTH = 120;
        public const String IMAGE_JPEG = "jpeg";

        public const String NAMING_ENVIRONMENT_SHORT_DATA = "short";
        public const String NAMING_ENVIRONMENT_FULL_DATA = "full";

        public const char ELEMENT_START = '\u001c';
        public const char ELEMENT_END = '\u001d';
        public const char ELEMENT_NAME_VALUE_SEPARATOR = '\u001e';

        public const char ELEMENT_VISIBLE_START = '<';
        public const char ELEMENT_VISIBLE_END = '>';
        public const char ELEMENT_VISIBLE_NAME_VALUE_SEPARATOR = '=';

        public static readonly string DATA_TABLE_NULL = '\u001a'.ToString();
        public const String DATA_TABLE_VISIBLE_NULL = "<NULL>";

        private static readonly List<FormatConverter<Object>> FORMAT_CONVERTERS = new List<FormatConverter<Object>>();

        public static String transferDecode(String value)
        {
            try
            {
                var s = TransferEncodingHelper.decode(value);

                return s;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Error decoding string value '" + value + "'", ex);
            }
        }

        public static string transferEncode(string value)
        {
           return TransferEncodingHelper.encode(value, 1);
        }

        public static FieldFormat createTableField(string name, TableFormat format)
        {
            var ff = (DataTableFieldFormat) FieldFormat.create(name, FieldFormat.DATATABLE_FIELD);
            ff.setDefault(new DataTable(format, true));
            return ff;
        }

        public static void registerFormatConverter(FormatConverter<object> converter)
        {
            FORMAT_CONVERTERS.Add(converter);
        }


        public static void inlineData(DataTable table, ContextManager cm, CallerController<CallerData> cc)
        {
            foreach (var ff in table.getFormat())
            {
                if (ff.getType() == FieldFormat.DATA_FIELD)
                {
                    foreach (var rec in table)
                    {
                        var data = rec.getData(ff.getName());
                        data.fetchData(cm, cc);
                        data.setId(null);
                    }
                }

                if (ff.getType() != FieldFormat.DATATABLE_FIELD) continue;
                foreach (var rec in table)
                {
                    var dt = rec.getDataTable(ff.getName());
                    inlineData(dt, cm, cc);
                }
            }
        }
    }
}