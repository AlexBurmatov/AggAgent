using System;
using System.Collections.Generic;
using System.Text;
using com.tibbo.aggregate.common.data;
using com.tibbo.aggregate.common.util;

namespace com.tibbo.aggregate.common.datatable.field
{
    public class DataFieldFormat : FieldFormat
    {
        public const string EDITOR_TEXT = "dtext";
        public const string EDITOR_IMAGE = "image";
        public const string EDITOR_SOUND = "sound";

        private const char SEPARATOR = '/';

        public const string EXTENSIONS_DESCR_FIELD = "extensionsDescr";
        public const string MODE_FIELD = "mode";
        public const string EXTENSIONS_FIELD = "extensions";
        public const string EXTENSION_FIELD = "extension";

        public const string FOLDER_FIELD = "folder";

        public static readonly TableFormat EXTENSIONS_FORMAT = new TableFormat();

        public static readonly TableFormat DATA_EDITOR_OPTIONS_FORMAT = new TableFormat(1, 1);

        static DataFieldFormat()
        {
            var modeF = create(MODE_FIELD, STRING_FIELD);
            modeF.setNullable(true);

            var edF = create(EXTENSIONS_DESCR_FIELD, STRING_FIELD);
            edF.setNullable(true);

            // Default value for 'extensions' field
            var extF = create(EXTENSION_FIELD, STRING_FIELD);
            EXTENSIONS_FORMAT.addField(extF);
            var dt = new DataTable(EXTENSIONS_FORMAT);

            FieldFormat extsF = create(EXTENSIONS_FIELD, DATATABLE_FIELD);
            extsF.setDefault(dt);
            extsF.setNullable(true);

            var folderF = create(FOLDER_FIELD, STRING_FIELD);
            folderF.setNullable(true);

            DATA_EDITOR_OPTIONS_FORMAT.addField(modeF);
            DATA_EDITOR_OPTIONS_FORMAT.addField(edF);
            DATA_EDITOR_OPTIONS_FORMAT.addField(extsF);
            DATA_EDITOR_OPTIONS_FORMAT.addField(folderF);
        }

        public DataFieldFormat(string name) : base(name)
        {
            setTransferEncode(true);
        }

        public override char getType()
        {
            return DATA_FIELD;
        }

        public override Type getFieldClass()
        {
            return typeof (Data);
        }

        public override Type getFieldWrappedClass()
        {
            return typeof (Data);
        }

        public override object getNotNullDefault()
        {
            return new Data();
        }

        public override object valueFromString(string value, ClassicEncodingSettings settings, Boolean validate)
        {
            try
            {
                var data = new Data();

                var parts = StringUtils.split(value, SEPARATOR, 5);

                if (!parts[1].Equals(DataTableUtils.DATA_TABLE_NULL))
                {
                    data.setId(Int64.Parse(parts[1]));
                }

                if (!parts[2].Equals(DataTableUtils.DATA_TABLE_NULL))
                {
                    data.setName(parts[2]);
                }

                var previewLen = Int32.Parse(parts[3]);

                if (previewLen != -1)
                {
                    data.setPreview(StringUtils.ASCII_CHARSET.GetBytes(parts[5].Substring(0, previewLen)));
                }

                var dataLen = Int32.Parse(parts[4]);

                if (dataLen != -1)
                {
                    data.setData(StringUtils.ASCII_CHARSET.GetBytes(parts[5].Substring(previewLen <= 0 ? 0 : previewLen)));
                }

                return data;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Invalid data block: " + ex.Message, ex);
            }
        }

        public override string valueToString(object value, ClassicEncodingSettings settings)
        {
            Data correctValue = (Data) value;
            return correctValue.encode();
        }

        protected override List<string> getSuitableEditors()
        {
            return new List<string>(new[] {EDITOR_TEXT, EDITOR_IMAGE, EDITOR_SOUND});
        }

        public static string encodeEditorOptions(string mode)
        {
            return encodeEditorOptions(mode, null, null, null);
        }

        public static string encodeEditorOptions(string extensionsDescription, string folder, List<string> extensions)
        {
            return encodeEditorOptions(null, extensionsDescription, folder, extensions);
        }

        private static string encodeEditorOptions(string mode, string extensionsDescription, string folder,
                                                  IEnumerable<string> extensions)
        {
            DataTable esdt = null;
            if (extensions != null)
            {
                esdt = new DataTable(EXTENSIONS_FORMAT);
                foreach (var ext in extensions)
                {
                    esdt.addRecord().setValue(EXTENSION_FIELD, ext);
                }
            }
            var eodt = new DataTable(DATA_EDITOR_OPTIONS_FORMAT);
            var dr = eodt.addRecord();
            dr.setValue(MODE_FIELD, mode);
            dr.setValue(FOLDER_FIELD, folder);
            dr.setValue(EXTENSIONS_DESCR_FIELD, extensionsDescription);
            dr.setValue(EXTENSIONS_FIELD, esdt);

            return eodt.encode();
        }
    }
}