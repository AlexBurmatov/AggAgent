using System;
using System.Collections.Generic;

namespace com.tibbo.aggregate.common.datatable.field
{
    public class StringFieldFormat : FieldFormat
    {
        public const String EDITOR_CONTEXT_MASK = "contextmask";
        public const String EDITOR_CONTEXT = "context";
        public const String EDITOR_EXPRESSION = "expression";
        public const String EDITOR_TEXT = "text";
        public const String EDITOR_TEXT_AREA = "textarea";
        public const String EDITOR_REFERENCE = "reference";
        public const String EDITOR_PASSWORD = "password";
        public const String EDITOR_FONT = "font";
        public const String EDITOR_IP = "ip";

        public const String FIELD_EXPRESSION_EDITOR_OPTIONS_REFERENCE = "reference";
        public const String FIELD_EXPRESSION_EDITOR_OPTIONS_DESCRIPTION = "description";

        public static readonly TableFormat ADDITIONAL_REFERENCES_FORMAT = new TableFormat();

        static StringFieldFormat()
        {
            ADDITIONAL_REFERENCES_FORMAT.addField("<reference><S>");
            ADDITIONAL_REFERENCES_FORMAT.addField("<description><S>");
        }

        public const String EDITOR_MODE_JAVA = "java";
        public const String EDITOR_MODE_XML = "xml";
        public const String EDITOR_MODE_SQL = "sql";
        public const String EDITOR_MODE_SHELLSCRIPT = "shellscript";
        public const String EDITOR_MODE_SMI_MIB = "smi-mib";

        public StringFieldFormat(String name) : base(name)
        {
            setTransferEncode(true);
        }

        public override char getType()
        {
            return STRING_FIELD;
        }

        public override Type getFieldClass()
        {
            return typeof (String);
        }

        public override Type getFieldWrappedClass()
        {
            return typeof (string);
        }

        public override object getNotNullDefault()
        {
            return "";
        }

        public override object valueFromString(string value, ClassicEncodingSettings settings, bool validate)
        {
            return value;
        }

        public override string valueToString(object value, ClassicEncodingSettings settings)
        {
            return (string)value;
        }

        protected override List<String> getSuitableEditors()
        {
            return
                new List<String>(new[]
                                     {
                                         EDITOR_CONTEXT_MASK, EDITOR_CONTEXT, EDITOR_TEXT_AREA, EDITOR_TEXT,
                                         EDITOR_REFERENCE, EDITOR_EXPRESSION, EDITOR_PASSWORD, EDITOR_FONT, EDITOR_IP
                                     });
        }

        public static String encodeExpressionEditorOptions(Dictionary<String, String> additionalReferences)
        {
            var op = new DataTable(ADDITIONAL_REFERENCES_FORMAT);

            foreach (var entry in additionalReferences)
            {
                op.addRecord().addString(entry.Key).addString(entry.Value ?? entry.Key);
            }

            return op.encode(false);
        }
    }
}