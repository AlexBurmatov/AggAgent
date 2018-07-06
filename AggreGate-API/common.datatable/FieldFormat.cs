namespace com.tibbo.aggregate.common.datatable
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Drawing;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Windows.Documents;

    using com.tibbo.aggregate.common.context;
    using com.tibbo.aggregate.common.data;
    using com.tibbo.aggregate.common.datatable.field;
    using com.tibbo.aggregate.common.datatable.validator;
    using com.tibbo.aggregate.common.util;

    using command;

    using JavaCompatibility;

    public abstract class FieldFormat : ICloneable
    {
        #region Constants

        protected static readonly Dictionary<Char, Type> TYPES = new AgDictionary<Char, Type>();

        protected static readonly Dictionary<Type, Char> CLASS_TO_TYPE = new AgDictionary<Type, Char>();

        protected static readonly Dictionary<Object, string> TYPE_SELECTION_VALUES = new AgDictionary<Object, string>();

        public const char INTEGER_FIELD = 'I';
        public const char STRING_FIELD = 'S';
        public const char BOOLEAN_FIELD = 'B';
        public const char LONG_FIELD = 'L';
        public const char FLOAT_FIELD = 'F';
        public const char DOUBLE_FIELD = 'E';
        public const char DATE_FIELD = 'D';
        public const char DATATABLE_FIELD = 'T';
        public const char OBJECT_FIELD = 'O';
        public const char COLOR_FIELD = 'C';
        public const char DATA_FIELD = 'A';

        protected const string ELEMENT_FLAGS = "F";
        protected const string ELEMENT_DEFAULT_VALUE = "A";
        protected const string ELEMENT_DESCRIPTION = "D";
        protected const string ELEMENT_HELP = "H";
        protected const string ELEMENT_SELECTION_VALUES = "S";
        protected const string ELEMENT_VALIDATORS = "V";
        protected const string ELEMENT_EDITOR = "E";
        protected const string ELEMENT_EDITOR_OPTIONS = "O";
        protected const string ELEMENT_ICON = "I";
        protected const string ELEMENT_GROUP = "G";

        protected const char NULLABLE_FLAG = 'N';
        protected const char OPTIONAL_FLAG = 'O';
        protected const char EXTENDABLE_SELECTION_VALUES_FLAG = 'E';
        protected const char READ_ONLY_FLAG = 'R';
        protected const char NOT_REPLICATED_FLAG = 'C';
        protected const char HIDDEN_FLAG = 'H';
        protected const char KEY_FIELD_FLAG = 'K';
        protected const char INLINE_DATA_FLAG = 'I';
        public const char VALIDATOR_LIMITS = 'L';
        public const char VALIDATOR_REGEX = 'R';
        protected const char DEFAULT_OVERRIDE = 'D';

        #endregion


        #region Class Initialization

        static FieldFormat()
        {
            TYPES.Add(INTEGER_FIELD, typeof(IntFieldFormat));
            TYPES.Add(STRING_FIELD, typeof(StringFieldFormat));
            TYPES.Add(BOOLEAN_FIELD, typeof(BooleanFieldFormat));
            TYPES.Add(LONG_FIELD, typeof(LongFieldFormat));
            TYPES.Add(FLOAT_FIELD, typeof(FloatFieldFormat));
            TYPES.Add(DOUBLE_FIELD, typeof(DoubleFieldFormat));
            TYPES.Add(DATE_FIELD, typeof(DateFieldFormat));
            TYPES.Add(DATATABLE_FIELD, typeof(DataTableFieldFormat));
            TYPES.Add(COLOR_FIELD, typeof(ColorFieldFormat));
            TYPES.Add(DATA_FIELD, typeof(DataFieldFormat));

            CLASS_TO_TYPE.Add(typeof(Int32), INTEGER_FIELD);
            CLASS_TO_TYPE.Add(typeof(string), STRING_FIELD);
            CLASS_TO_TYPE.Add(typeof(Boolean), BOOLEAN_FIELD);
            CLASS_TO_TYPE.Add(typeof(Int64), LONG_FIELD);
            CLASS_TO_TYPE.Add(typeof(float), FLOAT_FIELD);
            CLASS_TO_TYPE.Add(typeof(double), DOUBLE_FIELD);
            CLASS_TO_TYPE.Add(typeof(DateTime), DATE_FIELD);
            CLASS_TO_TYPE.Add(typeof(DataTable), DATATABLE_FIELD);
            CLASS_TO_TYPE.Add(typeof(Object), OBJECT_FIELD);
            CLASS_TO_TYPE.Add(typeof(Color), COLOR_FIELD);
            CLASS_TO_TYPE.Add(typeof(Data), DATA_FIELD);

            TYPE_SELECTION_VALUES.Add(INTEGER_FIELD.ToString(), Cres.get().getString("dtInteger"));
            TYPE_SELECTION_VALUES.Add(STRING_FIELD.ToString(), Cres.get().getString("dtString"));
            TYPE_SELECTION_VALUES.Add(BOOLEAN_FIELD.ToString(), Cres.get().getString("dtBoolean"));
            TYPE_SELECTION_VALUES.Add(LONG_FIELD.ToString(), Cres.get().getString("dtLong"));
            TYPE_SELECTION_VALUES.Add(FLOAT_FIELD.ToString(), Cres.get().getString("dtFloat"));
            TYPE_SELECTION_VALUES.Add(DOUBLE_FIELD.ToString(), Cres.get().getString("dtDouble"));
            TYPE_SELECTION_VALUES.Add(DATE_FIELD.ToString(), Cres.get().getString("date"));
            TYPE_SELECTION_VALUES.Add(DATATABLE_FIELD.ToString(), Cres.get().getString("dtDataTable"));
            TYPE_SELECTION_VALUES.Add(OBJECT_FIELD.ToString(), Cres.get().getString("dtObject"));
            TYPE_SELECTION_VALUES.Add(COLOR_FIELD.ToString(), Cres.get().getString("color"));
            TYPE_SELECTION_VALUES.Add(DATA_FIELD.ToString(), Cres.get().getString("dtDataBlock"));
        }

        #endregion


        #region Instance Variables

        protected bool keyField;
        protected string name;
        protected bool nullable;
        protected bool optional;
        protected bool extendableSelectionValues;
        protected bool readOnly;
        protected bool notReplicated;
        protected bool hidden;
        protected bool inlineData;
        protected bool defaultOverride;
        protected string description;
        protected string help;
        protected string group;
        protected string editor;
        protected string editorOptions;
        protected AgList<FieldValidator> validators = new AgList<FieldValidator>();
        protected string icon;
        protected bool transferEncode;
        protected string cachedDefaultDescription;
        protected object defaultValue;
        protected AgDictionary<NullableObject, string> selectionValues;

        private bool immutable;

        #endregion Instance Variables


        #region Abstract Methods

        public abstract char getType();

        public abstract Type getFieldClass();

        public abstract Type getFieldWrappedClass();

        public abstract object getNotNullDefault();

        public abstract object valueFromString(string value, ClassicEncodingSettings settings, bool validate);

        public abstract string valueToString(object value, ClassicEncodingSettings settings);


        #endregion Abstract Methods


        protected FieldFormat(string name)
        {
            this.name = name;
        }


        #region Field Format Creation

        public static FieldFormat create(string name, Type valueClass)
        {
            char? type = CLASS_TO_TYPE[valueClass];

            if (type == null)
            {
                throw new ArgumentException("Unknown field class: " + valueClass.Name);
            }

            return create(name, (char)type);
        }

        public static FieldFormat create(string name, char type)
        {
            switch (type)
            {
                case INTEGER_FIELD:
                    return new IntFieldFormat(name);

                case STRING_FIELD:
                    return new StringFieldFormat(name);

                case BOOLEAN_FIELD:
                    return new BooleanFieldFormat(name);

                case LONG_FIELD:
                    return new LongFieldFormat(name);

                case FLOAT_FIELD:
                    return new FloatFieldFormat(name);

                case DOUBLE_FIELD:
                    return new DoubleFieldFormat(name);

                case DATE_FIELD:
                    return new DateFieldFormat(name);

                case DATATABLE_FIELD:
                    return new DataTableFieldFormat(name);

                case COLOR_FIELD:
                    return new ColorFieldFormat(name);

                case DATA_FIELD:
                    return new DataFieldFormat(name);

                default:
                    throw new ArgumentException("Unknown field type: " + type);
            }

        }

        public static FieldFormat create(string name, char type, string description)
        {
            var ff = create(name, type);
            ff.setDescription(description);
            return ff;
        }

        public static FieldFormat create(string name, char type, string description, object defaultValue)
        {
            return create(name, type, description, defaultValue, false, null);
        }

        public static FieldFormat create(string name, char type, string description, object defaultValue, bool nullable)
        {
            return create(name, type, description, defaultValue, nullable, null);
        }

        public static FieldFormat create(string name, char type, string description, object defaultValue, bool nullable, string group)
        {
            FieldFormat ff = create(name, type, description);
            ff.setNullable(nullable);
            ff.setDefault(defaultValue);
            //ff.setGroup(group);
            return ff;
        }

        public static FieldFormat create(string format, ClassicEncodingSettings settings)
        {
            return create(format, settings, true);
        }

        public static FieldFormat create(string format, ClassicEncodingSettings settings, bool validate)
        {
            var els = StringUtils.elements(format, settings.isUseVisibleSeparators());

            string name;
            char type;

            try
            {
                name = els[0].getValue();
                type = els[1].getValue()[0];
            }
            catch (IndexOutOfRangeException ex1)
            {
                throw new ArgumentException(ex1.Message + ", format was '" + format + "'", ex1);
            }

            var ff = create(name, type);

            //if (validate)
            //{
            //    ff.validateName();
            //}

            var el = els.getElement(ELEMENT_FLAGS);

            if (el != null)
            {
                var flags = el.getValue();
                ff.setNullable(flags.IndexOf(NULLABLE_FLAG) != -1 ? true : false);
                ff.setOptional(flags.IndexOf(OPTIONAL_FLAG) != -1 ? true : false);
                ff.setExtendableSelectionValues(flags.IndexOf(EXTENDABLE_SELECTION_VALUES_FLAG) != -1 ? true : false);
                ff.setReadonly(flags.IndexOf(READ_ONLY_FLAG) != -1 ? true : false);
                ff.setNotReplicated(flags.IndexOf(NOT_REPLICATED_FLAG) != -1 ? true : false);
                ff.setHidden(flags.IndexOf(HIDDEN_FLAG) != -1 ? true : false);
                ff.setKeyField(flags.IndexOf(KEY_FIELD_FLAG) != -1 ? true : false);
                ff.setDefaultOverride(flags.IndexOf(DEFAULT_OVERRIDE) != -1 ? true : false);
                ff.setInlineData(flags.IndexOf(INLINE_DATA_FLAG) != -1 ? true : false);
            }

            el = els.getElement(ELEMENT_DEFAULT_VALUE);

            if (el != null)
            {
                ff.setDefaultFromString(el.getValue(), settings, validate);
            }

            el = els.getElement(ELEMENT_DESCRIPTION);

            if (el != null)
            {
                ff.setDescription(el.getValue());
            }

            el = els.getElement(ELEMENT_HELP);

            if (el != null)
            {
                ff.setHelp(el.getValue());
            }

            el = els.getElement(ELEMENT_SELECTION_VALUES);

            if (el != null)
            {
                ff.createSelectionValues(el.getValue(), settings);
            }

            el = els.getElement(ELEMENT_VALIDATORS);

            if (el != null)
            {
                ff.createValidators(el.getValue(), settings);
            }

            el = els.getElement(ELEMENT_EDITOR);

            if (el != null)
            {
                ff.setEditor(el.getValue());
            }

            el = els.getElement(ELEMENT_EDITOR_OPTIONS);

            if (el != null)
            {
                ff.setEditorOptions(el.getValue());
            }

            el = els.getElement(ELEMENT_ICON);

            if (el != null)
            {
                ff.setIcon(el.getValue());
            }

            return ff;
        }

        //private void validateName()
        //{
        //    try
        //    {
        //        ValidatorHelper.NAME_SYNTAX_VALIDATOR.validate(getName());
        //    }
        //    catch (ValidationException ve)
        //    {
        //        throw new RuntimeException(MessageFormat.format(Cres.get().getString("dtIllegalFieldValue"), getName(), toDetailedString()) + ve.getMessage(), ve);
        //    }
        //}


        public static FieldFormat create(string format)
        {
            return create(format, new ClassicEncodingSettings(true), true);
        }
        #endregion



        protected void createSelectionValues(string source, ClassicEncodingSettings settings)
        {
            if (source.Length == 0)
            {
                return;
            }

            var values = new AgDictionary<NullableObject, string>();

            var els = StringUtils.elements(source, settings.isUseVisibleSeparators());
            foreach (var el in els)
            {
                var valueSource = el.getValue();

                NullableObject selectionValue = new NullableObject(this.valueFromEncodedString(valueSource, settings, true));
                var desc = el.getName() ?? selectionValue.ToString();
                values[selectionValue] = desc;
            }

            this.setSelectionValues(values.Count > 0 ? values : null);
        }



        public object valueFromEncodedString(string source)
        {
            return this.valueFromEncodedString(source, new ClassicEncodingSettings(false), true);
        }

        public object valueFromEncodedString(string source, ClassicEncodingSettings settings, bool validate)
        {
            return
                source.Equals(settings.isUseVisibleSeparators() ? DataTableUtils.DATA_TABLE_VISIBLE_NULL : DataTableUtils.DATA_TABLE_NULL)
                    ? (object)null
                    : (this.valueFromString(this.isTransferEncode() ? DataTableUtils.transferDecode(source) : source, settings, validate));
        }

        // Converts <code>value</code> string to the object of type suitable for this <code>FieldFormat</code>.
        public object valueFromString(string value)
        {
            return this.valueFromString(value, null, false);
        }

        // Converts value suitable for this field format to string.
        public string valueToString(object value)
        {
            return this.valueToString(value, null);
        }

        public string valueToEncodedString(object value, ClassicEncodingSettings settings)
        {
            return this.valueToEncodedString(value, settings, new StringBuilder(), 1).ToString();
        }

        public StringBuilder valueToEncodedString(object value, ClassicEncodingSettings settings, StringBuilder sb, int? encodeLevel)
        {
            string strVal = this.valueToString(value, settings);

            if (strVal == null)
            {
                return (settings == null || !settings.isUseVisibleSeparators()) ? sb.Append(DataTableUtils.DATA_TABLE_NULL) : sb.Append(DataTableUtils.DATA_TABLE_VISIBLE_NULL);
            }

            if (this.isTransferEncode())
            {
                TransferEncodingHelper.encode(strVal, sb, encodeLevel);
            }
            else
            {
                sb.Append(strVal);
            }

            return sb;
        }

        // Sets default value of field from its string representation.
        public void setDefaultFromString(string value)
        {
            this.setDefaultFromString(value, new ClassicEncodingSettings(false), true);
        }

        public void setDefaultFromString(string value, ClassicEncodingSettings settings, bool validate)
        {
            if (value.Length == 0)
            {
                return;
            }

            // Overriding validate flag here, as default value may contain non-valid table
            this.setDefault(this.valueFromEncodedString(value, settings, false));
        }

        public FieldFormat setDefault(object value)
        {
            //if (immutable)
            //{
            //    throw new IllegalStateException("Immutable");
            //}

            try
            {
                defaultValue = this.checkAndConvertValue(value, true);
            }
            catch (ContextException ex)
            {
                throw new ArgumentException(ex.Message, ex);
            }
            return this;
        }

        private string getEncodedSelectionValues(ClassicEncodingSettings settings)
        {
            if (this.selectionValues == null)
            {
                return null;
            }

            var enc = new StringBuilder();

            foreach (var value in this.selectionValues)
            {
                var valueDesc = value.Value;
                enc.Append(new Element(valueDesc, this.valueToEncodedString(value.Key.Value, settings)).encode(settings));
            }

            return enc.ToString();
        }

        private string getEncodedValidators(ClassicEncodingSettings settings)
        {
            if (this.validators.Count == 0)
            {
                return null;
            }

            var enc = new StringBuilder();

            foreach (var fv in this.validators)
            {
                if (fv.getType() != null)
                {
                    enc.Append(new Element(fv.getType().ToString(), fv.encode()).encode(settings));
                }
            }

            return enc.ToString();
        }

        private string getEncodedFlags()
        {
            var buf = new StringBuilder();
            if (this.isNullable())
            {
                buf.Append(NULLABLE_FLAG);
            }
            if (this.isOptional())
            {
                buf.Append(OPTIONAL_FLAG);
            }
            if (this.isReadonly())
            {
                buf.Append(READ_ONLY_FLAG);
            }
            if (this.isNotReplicated())
            {
                buf.Append(NOT_REPLICATED_FLAG);
            }
            if (this.isExtendableSelectionValues())
            {
                buf.Append(EXTENDABLE_SELECTION_VALUES_FLAG);
            }
            if (this.isHidden())
            {
                buf.Append(HIDDEN_FLAG);
            }
            if (this.isKeyField())
            {
                buf.Append(KEY_FIELD_FLAG);
            }
            if (this.isInlineData())
            {
                buf.Append(INLINE_DATA_FLAG);
            }
            //if (isAdvanced())
            //{
            //    buf.append(ADVANCED_FLAG);
            //}
            if (this.isDefaultOverride())
            {
                buf.Append(DEFAULT_OVERRIDE);
            }
            //if (isShallow())
            //{
            //    buf.append(SHALLOW_FLAG);
            //}
            return buf.ToString();
        }

        private static void encAppend(StringBuilder buffer, string name, string value, ClassicEncodingSettings settings)
        {
            encAppend(buffer, name, value, settings, false);
        }

        private static void encAppend(StringBuilder buffer, string name, string value, ClassicEncodingSettings settings, bool allowEmptyString)
        {
            if (value != null && (allowEmptyString || value.Length > 0))
            {
                new Element(name, value).encode(buffer, settings, false, 0);
            }
        }

        public string encode()
        {
            return this.encode(true);
        }

        public string encode(bool useVisibleSeparators)
        {
            return this.encode(new ClassicEncodingSettings(useVisibleSeparators));
        }

        public string encode(ClassicEncodingSettings settings)
        {
            StringBuilder data = new StringBuilder();

            new Element(null, this.getName()).encode(data, settings);
            new Element(null, this.getType().ToString()).encode(data, settings);

            encAppend(data, ELEMENT_FLAGS, this.getEncodedFlags(), settings);

            if (settings.isEncodeDefaultValues())
            {

                new Element(ELEMENT_DEFAULT_VALUE, this.valueToEncodedString(this.getDefaultValue(), settings)).encode(data, settings);
            }

            encAppend(data, ELEMENT_DESCRIPTION, DataTableUtils.transferEncode(this.description), settings);
            encAppend(data, ELEMENT_HELP, DataTableUtils.transferEncode(this.help), settings);
            encAppend(data, ELEMENT_SELECTION_VALUES, this.getEncodedSelectionValues(settings), settings);
            encAppend(data, ELEMENT_VALIDATORS, this.getEncodedValidators(settings), settings);
            encAppend(data, ELEMENT_EDITOR, DataTableUtils.transferEncode(this.editor), settings);
            encAppend(data, ELEMENT_EDITOR_OPTIONS, DataTableUtils.transferEncode(this.editorOptions), settings, true);
            encAppend(data, ELEMENT_ICON, DataTableUtils.transferEncode(this.icon), settings);
            encAppend(data, ELEMENT_GROUP, DataTableUtils.transferEncode(this.group), settings);

            return data.ToString();
        }

        public bool extend(FieldFormat other)
        {
            return this.extendMessage(other) == null;
        }

        public string extendMessage(FieldFormat other)
        {
            if (!this.getName().Equals(other.getName()))
            {
                return "Wrong name: need " + this.getName() + ", found " + other.getName();
            }
            if (!Util.equals(this.getDescription(), other.getDescription()))
            {
                return "Wrong description: need " + this.getDescription() + ", found " + other.getDescription();
            }
            if (!Util.equals(this.getHelp(), other.getHelp()))
            {
                return "Wrong help: need " + this.getHelp() + ", found " + other.getHelp();
            }
            if (this.getType() != other.getType())
            {
                return "Wrong type: need " + this.getType() + ", found " + other.getType();
            }
            if (!this.isNullable() && other.isNullable())
            {
                return "Different nullable flags: need " + this.isNullable() + ", found " + other.isNullable();
            }
            if (this.isReadonly() != other.isReadonly())
            {
                return "Different readonly flags: need " + this.isReadonly() + ", found " + other.isReadonly();
            }
            if (this.isHidden() != other.isHidden())
            {
                return "Different hidden flags: need " + this.isHidden() + ", found " + other.isHidden();
            }

            if (!isExtendableSelectionValues() || !other.isExtendableSelectionValues())
            {
                bool selectionValuesOk = other.getSelectionValues() == null || Util.equals(getSelectionValues(), other.getSelectionValues());
                if (!selectionValuesOk && getSelectionValues() != null)
                {
                    bool foundMissingValues = false;
                    foreach (var value in getSelectionValues().Keys)
                    {
                        if (!other.getSelectionValues().ContainsKey(value))
                        {
                            foundMissingValues = true;
                        }
                    }
                    if (!foundMissingValues)
                    {
                        selectionValuesOk = true;
                    }
                }

                if (!selectionValuesOk)
                {
                    return "Different selection values: need " + other.getSelectionValues() + ", found " + getSelectionValues();
                }
            }

            if (!Util.equals(this.getEditor(), other.getEditor()))
            {
                return "Different editor: need " + this.getEditor() + ", found " + other.getEditor();
            }
            if (!Util.equals(this.getEditorOptions(), other.getEditorOptions()))
            {
                return "Different editor options: need " + this.getEditorOptions() + ", found " + other.getEditorOptions();
            }
            if (!Util.equals(getIcon(), other.getIcon()))
            {
                return "Wrong icon: need " + getIcon() + ", found " + other.getIcon();
            }
            //if (!Util.equals(getGroup(), other.getGroup()))
            //{
            //    return "Wrong group: need " + getGroup() + ", found " + other.getGroup();
            //}

            List<FieldValidator> otherValidators = other.getValidators();
            foreach (var otherValidator in otherValidators)
            {
                if (!this.getValidators().Contains(otherValidator))
                {
                    return "Different validators: need " + this.getValidators() + ", found " + other.getValidators();
                }
            }

            if (this.getDefaultValue() != null && (this.getDefaultValue() is DataTable) && (other.getDefaultValue() != null) && (other.getDefaultValue() is DataTable))
            {
                DataTable my = this.getDefaultValue() as DataTable;
                DataTable another = other.getDefaultValue() as DataTable;
                string msg = my.getFormat().extendMessage(another.getFormat());
                if (msg != null)
                {
                    return "Field format doesn't match: " + msg;
                }
            }
            return null;
        }

        public void addValidator(FieldValidator validator)
        {
            this.validators.Add(validator);
        }

        public void setValidators(Collection<FieldValidator> newValidators)
        {
            this.validators.Clear();
            foreach (var each in newValidators)
            {
                this.validators.Add(each);
            }
        }

        private void createValidators(string source, ClassicEncodingSettings settings)
        {
            if (string.IsNullOrEmpty(source))
            {
                return;
            }

            var validatorsData = StringUtils.elements(source, settings.isUseVisibleSeparators());

            foreach (var el in validatorsData)
            {
                var validatorType = el.getName()[0];
                var validatorParams = el.getValue();

                switch (validatorType)
                {
                    case VALIDATOR_LIMITS:
                        this.addValidator(new LimitsValidator(this, validatorParams));
                        break;

                    case VALIDATOR_REGEX:
                        this.addValidator(new RegexValidator(validatorParams));
                        break;

                        //case VALIDATOR_NON_NULL:
                        //    addValidator(new NonNullValidator(validatorParams));
                        //    break;
                        //
                        //case VALIDATOR_ID:
                        //    addValidator(new IdValidator());
                        //    break;
                        //
                        //case VALIDATOR_EXPRESSION:
                        //    addValidator(new ExpressionValidator(validatorParams));
                        //    break;
                }
            }
        }

        public object checkAndConvertValue(object value, bool validate)
        {
            return this.checkAndConvertValue(null, null, null, value, validate);
        }

        public object checkAndConvertValue(Context context, ContextManager contextManager, CallerController<CallerData> caller, object value, bool validate)
        {
            if (!this.isNullable() && value == null)
            {
                throw new ValidationException("Nulls are not permitted in " + this);
                //throw new ValidationException(MessageFormat.format(Cres.get().getString("dtNullsNotPermitted"), this.toString()));
            }

            value = this.convertValue(value);

            if (value != null && !this.isExtendableSelectionValues() && this.hasSelectionValue(value))
            {
                if (validate)
                {
                    throw new ValidationException(Cres.get().getString("dtValueNotInSelVals") + value + " (" + value.GetType().Name + ")");
                }
                else
                {
                    value = this.getDefaultValue();
                }
            }

            if (validate)
            {
                foreach (var fv in this.validators)
                {
                    value = fv.validate(context, contextManager, caller, value);
                }
            }

            return value;
        }


        protected object convertValue(object value)
        {
            if (value != null)
            {
                if (this.getFieldClass() == value.GetType() || this.getFieldWrappedClass() == value.GetType() || this.getFieldClass().IsInstanceOfType(value)
                    || this.getFieldWrappedClass().IsInstanceOfType(value))
                {
                    return value;
                }

                throw new ValidationException("Invalid class, need '" + this.getFieldWrappedClass().Name + "', found '" + value.GetType().Name + "'");
            }

            return value; // Always null!!!
        }

        public string getTypeName()
        {
            return TYPE_SELECTION_VALUES[this.getType().ToString()];
        }

        public string getName()
        {
            return this.name;
        }

        public bool isNullable()
        {
            return this.nullable;
        }

        public object getDefaultValue()
        {
            // Don't call this method before format construction is finished, otherwise setting format to nullable will not set default value to null

            if (this.defaultValue == null && !this.isNullable())
            {
                this.defaultValue = this.getNotNullDefault();
            }

            return this.defaultValue;
        }

        public object getDefaultValueCopy()
        {
            var def = this.getDefaultValue();
            return def == null ? null : CloneUtils.deepClone(def);
        }

        public string getDescription()
        {
            if (this.description == null)
            {
                if (cachedDefaultDescription == null)
                {
                    cachedDefaultDescription = createDefaultDescription(this.name);
                }

                return this.cachedDefaultDescription;
            }

            return this.description;
        }

        public bool hasDescription()
        {
            return this.description != null;
        }

        public string getHelp()
        {
            return this.help;
        }

        public bool isOptional()
        {
            return this.optional;
        }

        public bool hasSelectionValues()
        {
            return this.selectionValues != null && this.selectionValues.Count > 0;
        }

        // Returns true if field has specified selection value.
        public bool hasSelectionValue(object value)
        {
            return this.selectionValues != null && !this.selectionValues.ContainsKey(new NullableObject(value));
        }

        public Dictionary<NullableObject, string> getSelectionValues()
        {
            return this.selectionValues;
        }

        public FieldFormat addSelectionValue(object value, string description)
        {

            if (string.IsNullOrEmpty(description))
            {
                throw new ArgumentException("Empty selection value description");
            }

            try
            {
                value = this.convertValue(value);
            }
            catch (ValidationException ex)
            {
                throw new ArgumentException(ex.Message, ex);
            }

            if (this.selectionValues == null)
            {
                this.selectionValues = new AgDictionary<NullableObject, string>();
            }

            this.selectionValues[new NullableObject(value)] = description;

            return this;
        }

        // Adds new selection value to the field.Value description is obtained by calling value.toString().
        public FieldFormat addSelectionValue(object value)
        {
            return this.addSelectionValue(value, value.ToString());
        }

        public bool isExtendableSelectionValues()
        {
            return this.extendableSelectionValues;
        }

        public List<FieldValidator> getValidators()
        {
            return this.validators;
        }

        public bool isReadonly()
        {
            return this.readOnly;
        }

        public bool isNotReplicated()
        {
            return this.notReplicated;
        }

        protected bool isTransferEncode()
        {
            return this.transferEncode;
        }

        public bool isHidden()
        {
            return this.hidden;
        }

        public string getEditor()
        {
            return this.editor;
        }

        public static Dictionary<object, string> getTypeSelectionValues()
        {
            return TYPE_SELECTION_VALUES;
        }

        public static Dictionary<Type, char> getClassToTypeMap()
        {
            return CLASS_TO_TYPE;
        }

        public bool isKeyField()
        {
            return this.keyField;
        }

        public string getEditorOptions()
        {
            return this.editorOptions;
        }

        public bool isInlineData()
        {
            return this.inlineData;
        }


        public void setDescription(string descriptionString)
        {
            if (immutable)
            {
                throw new ArgumentException("Immutable");
            }

            this.description = descriptionString;
        }

        public FieldFormat setHelp(string helpString)
        {
            if (immutable)
            {
                throw new ArgumentException("Immutable");
            }

            this.help = helpString;
            return this;
        }

        //Sets field selection values.
        public FieldFormat setSelectionValues(AgDictionary<NullableObject, string> selectionValuesDictionary)
        {
            if (immutable)
            {
                throw new ArgumentException("Immutable");
            }

            if (selectionValues != null && selectionValues.Count == 0)
            {
                this.selectionValues = null;
                return this;
            }

            this.selectionValues = (selectionValuesDictionary is ICloneable) ? selectionValuesDictionary : new AgDictionary<NullableObject, string>(selectionValuesDictionary);

            // If current default value doesn't match to new selection values, we change it to the first selection value from the list
            // ReSharper disable once PossibleNullReferenceException
            if (!this.selectionValues.ContainsKey(new NullableObject(this.getDefaultValue())) && !this.extendableSelectionValues)
            {
                this.setDefault(this.selectionValues.Keys.GetEnumerator().Current);
            }
            return this;
        }

        public FieldFormat setExtendableSelectionValues(bool extendableSelectionValuesBool)
        {
            if (immutable)
            {
                throw new ArgumentException("Immutable");
            }

            this.extendableSelectionValues = extendableSelectionValuesBool;
            return this;
        }

        public FieldFormat setNullable(bool nullableBool)
        {
            if (immutable)
            {
                throw new ArgumentException("Immutable");
            }

            this.nullable = nullableBool;
            return this;
        }


        public FieldFormat setOptional(bool optionalBool)
        {
            if (immutable)
            {
                throw new ArgumentException("Immutable");
            }

            this.optional = optionalBool;
            return this;
        }

        public FieldFormat setReadonly(bool readOnlyBool)
        {
            if (immutable)
            {
                throw new ArgumentException("Immutable");
            }

            this.readOnly = readOnlyBool;
            return this;
        }

        public FieldFormat setNotReplicated(bool notReplicated)
        {
            if (immutable)
            {
                throw new ArgumentException("Immutable");
            }

            this.notReplicated = notReplicated;
            return this;
        }

        protected FieldFormat setTransferEncode(bool transferEncodeBool)
        {
            this.transferEncode = transferEncodeBool;
            return this;
        }

        public FieldFormat setHidden(bool hidden)
        {
            if (immutable)
            {
                throw new ArgumentException("Immutable");
            }

            this.hidden = hidden;
            return this;
        }

        public FieldFormat setEditor(string editorString)
        {
            if (immutable)
            {
                throw new ArgumentException("Immutable");
            }

            if (editorString != null && !this.getSuitableEditors().Contains(editorString))
            {
                Logger.getLogger(Log.DATATABLE).warn(Cres.get().getString("dtEditorNotSuitable") + editorString + " (" + this.ToString() + ")");
            }

            this.editor = editorString;

            return this;
        }

        public FieldFormat setKeyField(bool keyFieldBool)
        {
            if (immutable)
            {
                throw new ArgumentException("Immutable");
            }

            this.keyField = keyField;
            return this;
        }

        public FieldFormat setName(string name)
        {
            if (immutable)
            {
                throw new ArgumentException("Immutable");
            }

            this.name = name;
            return this;
        }

        public FieldFormat setEditorOptions(string editorOptions)
        {
            if (immutable)
            {
                throw new ArgumentException("Immutable");
            }

            this.editorOptions = editorOptions;
            return this;
        }

        public FieldFormat setInlineData(bool inlineData)
        {
            if (immutable)
            {
                throw new ArgumentException("Immutable");
            }

            this.inlineData = inlineData;
            return this;
        }

        public void setSelectionValues(string source)
        {
            createSelectionValues(source, new ClassicEncodingSettings(false));
        }

        public FieldFormat setIcon(string icon)
        {
            if (immutable)
            {
                throw new ArgumentException("Immutable");
            }

            this.icon = icon;
            return this;
        }

        public string getIcon()
        {
            return this.icon;
        }

        public bool isDefaultOverride()
        {
            return this.defaultOverride;
        }

        public FieldFormat setDefaultOverride(bool defaultOverrideBool)
        {
            this.defaultOverride = defaultOverrideBool;
            return this;
        }

        public override string ToString()
        {
            return description ?? name;
        }

        public string toDetailedString()
        {
            return (this.description != null ? this.description + " (" + this.name + ")" : this.name) + ", " + this.getTypeName();
        }

        protected virtual List<string> getSuitableEditors()
        {
            return new List<string>();
        }

        public TableFormat wrap()
        {
            return new TableFormat(this);
        }

        public TableFormat wrapSimple()
        {
            return new TableFormat(this).setMinRecords(1).setMaxRecords(1);
        }

        public override int GetHashCode()
        {
            const int prime = 31;
            int result = 1;
            // FIXME: Depends on CloneUtil.
            var def = this.getDefaultValue();
            result = prime * result + ((def == null) ? 0 : def.GetHashCode());
            result = prime * result + getType();
            result = prime * result + ((description == null) ? 0 : description.GetHashCode());
            result = prime * result + ((editor == null) ? 0 : editor.GetHashCode());
            result = prime * result + ((editorOptions == null) ? 0 : editorOptions.GetHashCode());
            result = prime * result + ((icon == null) ? 0 : icon.GetHashCode());
            result = prime * result + ((group == null) ? 0 : group.GetHashCode());
            result = prime * result + (extendableSelectionValues ? 1231 : 1237);
            result = prime * result + ((help == null) ? 0 : help.GetHashCode());
            result = prime * result + (hidden ? 1231 : 1237);
            result = prime * result + (inlineData ? 1231 : 1237);
            result = prime * result + (keyField ? 1231 : 1237);
            result = prime * result + ((name == null) ? 0 : name.GetHashCode());
            result = prime * result + (notReplicated ? 1231 : 1237);
            result = prime * result + (nullable ? 1231 : 1237);
            result = prime * result + (optional ? 1231 : 1237);
//            result = prime * result + (readonly ? 1231 : 1237);
//            result = prime * result + (advanced ? 1231 : 1237);
            result = prime * result + ((selectionValues == null) ? 0 : selectionValues.GetHashCode());
            result = prime * result + (transferEncode ? 1231 : 1237);
            result = prime * result + ((validators == null) ? 0 : validators.GetHashCode());
            return result;
        }

        // TODO: Resync with Java, may (surely?) missing some (new) fields 
        public override bool Equals(object obj)
        {
            if (this == obj)
            {
                return true;
            }
            if (obj == null)
            {
                return false;
            }
            if (GetType() != obj.GetType())
            {
                return false;
            }
            var other = (FieldFormat)obj;
            if (name == null)
            {
                if (other.name != null)
                {
                    return false;
                }
            }
            else if (!name.Equals(other.name))
            {
                return false;
            }
            if (description == null)
            {
                if (other.description != null)
                {
                    return false;
                }
            }
            else if (!description.Equals(other.description))
            {
                return false;
            }
            var def = getDefaultValue();
            var odef = other.getDefaultValue();
            if (def == null)
            {
                if (odef != null)
                {
                    return false;
                }
            }
            else if (!def.Equals(odef))
            {
                return false;
            }
            if (help == null)
            {
                if (other.help != null)
                {
                    return false;
                }
            }
            else if (!help.Equals(other.help))
            {
                return false;
            }
            if (editor == null)
            {
                if (other.editor != null)
                {
                    return false;
                }
            }
            else if (!editor.Equals(other.editor))
            {
                return false;
            }
            if (editorOptions == null)
            {
                if (other.editorOptions != null)
                {
                    return false;
                }
            }
            else if (!editorOptions.Equals(other.editorOptions))
            {
                return false;
            }
            if (icon == null)
            {
                if (other.icon != null)
                {
                    return false;
                }
            }
            else if (!icon.Equals(other.icon))
            {
                return false;
            }
            if (extendableSelectionValues != other.extendableSelectionValues)
            {
                return false;
            }
            if (hidden != other.hidden)
            {
                return false;
            }
            if (inlineData != other.inlineData)
            {
                return false;
            }
            if (keyField != other.keyField)
            {
                return false;
            }
            if (notReplicated != other.notReplicated)
            {
                return false;
            }
            if (nullable != other.nullable)
            {
                return false;
            }
            if (optional != other.optional)
            {
                return false;
            }
            if (readOnly != other.readOnly)
            {
                return false;
            }
            if (selectionValues == null)
            {
                if (other.selectionValues != null)
                {
                    return false;
                }
            }
            else if (!selectionValues.Equals(other.selectionValues))
            {
                return false;
            }
            if (transferEncode != other.transferEncode)
            {
                return false;
            }
            if (this.validators == null)
            {
                if (other.validators != null)
                {
                    return false;
                }
            }
            else if (!this.validators.Equals(other.validators))
            {
                return false;
            }
            return true;
        }

        // TODO: Resync with Java, may (surely?) missing some (new) fields 
        public object Clone()
        {
            FieldFormat cl;
            try
            {
                cl = (FieldFormat)this.MemberwiseClone();
            }
            catch (CloneNotSupportedException ex)
            {
                throw new InvalidOperationException(ex.Message, ex);
            }

            cl.defaultValue = CloneUtils.genericClone(this.getDefaultValue());
            cl.selectionValues = (AgDictionary<NullableObject, string>)CloneUtils.deepClone(this.selectionValues);
            cl.validators = (AgList<FieldValidator>)CloneUtils.deepClone(this.validators);

            cl.immutable = false;

            return cl;
        }

        public void makeImmutable()
        {
            immutable = true;
        }

        private static string createDefaultDescription(string name)
        {
            var sb = new StringBuilder();

            var prevWasUpper = false;
            var nextToUpper = false;

            for (var i = 0; i < name.Length; i++)
            {
                var c = name[i];

                if (char.IsUpper(c))
                {
                    if (!prevWasUpper && i != 0)
                    {
                        sb.Append(" ");
                    }
                    prevWasUpper = true;
                }
                else
                {
                    prevWasUpper = false;
                }

                if (i == 0 || nextToUpper)
                {
                    c = char.ToUpper(c);
                    nextToUpper = false;
                }

                if (c == '_')
                {
                    sb.Append(" ");
                    nextToUpper = true;
                }
                else
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }

    }
}