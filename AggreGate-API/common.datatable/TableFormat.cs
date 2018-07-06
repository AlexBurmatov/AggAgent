using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Documents;
using com.tibbo.aggregate.common.binding;
using com.tibbo.aggregate.common.datatable.validator;
using com.tibbo.aggregate.common.expression;
using com.tibbo.aggregate.common.reference;
using com.tibbo.aggregate.common.util;
using JavaCompatibility;

namespace com.tibbo.aggregate.common.datatable
{
    using System.Linq;

    using com.tibbo.aggregate.common.context;

    public class TableFormat : IEnumerable<FieldFormat>, ICloneable, StringEncodable
    {
        #region constants

        public static readonly TableFormat EMPTY_FORMAT = new TableFormat(0, 0);

        public const int DEFAULT_MIN_RECORDS = 0;
        public const int DEFAULT_MAX_RECORDS = Int32.MaxValue;

        private const string ELEMENT_FLAGS = "F";
        private const string ELEMENT_TABLE_VALIDATORS = "V";
        private const string ELEMENT_RECORD_VALIDATORS = "R";
        private const string ELEMENT_BINDINGS = "B";
        private const string ELEMENT_MIN_RECORDS = "M";
        private const string ELEMENT_MAX_RECORDS = "X";
        private const string ELEMENT_NAMING = "N";

        public const char TABLE_VALIDATOR_KEY_FIELDS = 'K';

        public const char RECORD_VALIDATOR_KEY_FIELDS = 'K';

        private const char REORDERABLE_FLAG = 'R';
        private const char BINDINGS_EDITABLE_FLAG = 'B';

        #endregion

        private List<FieldFormat> fields = new AgList<FieldFormat>();

        private int minRecords = DEFAULT_MIN_RECORDS;
        private int maxRecords = DEFAULT_MAX_RECORDS;

        private bool reorderable;
        private bool bindingsEditable;

        private List<RecordValidator> recordValidators = new AgList<RecordValidator>();
        private List<TableValidator> tableValidators = new AgList<TableValidator>();
        private List<Binding> bindings = new AgList<Binding>();

        private Expression namingExpression;

        private bool immutable;
        private DataTable immutabilizer; // Data Table that made this format one immutable

        private Int32? id = null;

        public TableFormat()
        {
        }

        public TableFormat(object records)
        {
        }

        public TableFormat(bool reorderable)
        {
            setReorderable(reorderable);
        }

        public TableFormat(int minRecords, int maxRecords)
        {
            setMinRecords(minRecords);
            setMaxRecords(maxRecords);
        }

        public TableFormat(FieldFormat ff)
        {
            addField(ff);
        }

        public TableFormat(string format, ClassicEncodingSettings settings)
            : this(format, settings, true)
        {
        }

        public TableFormat(string format, ClassicEncodingSettings settings, bool validate)
        {
            if (format == null)
            {
                return;
            }

            var els = StringUtils.elements(format, settings.isUseVisibleSeparators());

            foreach (var el in els)
            {
                if (el.getName() == null)
                {
                    fields.Add(FieldFormat.create(el.getValue(), settings, validate));
                    continue;
                }
                if (el.getName().Equals(ELEMENT_FLAGS))
                {
                    var flags = el.getValue();
                    setReorderable(flags.IndexOf(REORDERABLE_FLAG) != -1 ? true : false);
                    setBindingsEditable(flags.IndexOf(BINDINGS_EDITABLE_FLAG) != -1 ? true : false);
                    continue;
                }
                if (el.getName().Equals(ELEMENT_MIN_RECORDS))
                {
                    minRecords = Int32.Parse(el.getValue());
                    continue;
                }
                if (el.getName().Equals(ELEMENT_MAX_RECORDS))
                {
                    maxRecords = Int32.Parse(el.getValue());
                    continue;
                }
                if (el.getName().Equals(ELEMENT_TABLE_VALIDATORS))
                {
                    createTableValidators(el.getValue(), settings);
                    continue;
                }
                if (el.getName().Equals(ELEMENT_RECORD_VALIDATORS))
                {
                    createRecordValidators(el.getValue(), settings);
                    continue;
                }
                if (el.getName().Equals(ELEMENT_BINDINGS))
                {
                    continue;
                }
                if (el.getName().Equals(ELEMENT_NAMING))
                {
                    createNaming(el.getValue());
                    continue;
                }
            }
        }

        public TableFormat(int minRecords, int maxRecords, string fieldFormatString)
            : this(minRecords, maxRecords)
        {
            addField(fieldFormatString);
        }
         
        public TableFormat(int minRecords, int maxRecords, FieldFormat fieldFormat) : this(minRecords, maxRecords)
        {
            addField(fieldFormat);
        }

        public TableFormat addField(FieldFormat ff)
        {
            return addField(ff, fields.Count);
        }

        public TableFormat addField(string encodedFormat)
        {
            return addField(FieldFormat.create(encodedFormat));
        }

        // Note, that modifying record format of an existing table by inserting fields anywhere except appending them to the end may cause <code>DataTable</code> to become invalid.
        public TableFormat addField(FieldFormat ff, int index)
        {
            if (immutable)
            {
                throw new ArgumentException("Immutable");
            }

            FieldFormat existing = this.getFieldFormat(ff.getName()) as FieldFormat;

            if (existing != null)
            {
                if (!ff.extend(existing))
                {
                    throw new ArgumentException("Field '" + ff.getName() + "' already exist in TableFormat");
                }
                return this;
            }

            fields.Insert(index, ff as FieldFormat);
            return this;
        }

        public void addField(char type, string name) 
        {
            addField(type, name, fields.Count);
        }

        // Note, that modifying record format of an existing table by inserting fields anywhere except appending them to the end may cause <code>DataTable</code> to become invalid.
        public void addField(char type, string name, int index)
        {
            if (immutable)
            {
                throw new ArgumentException("Immutable");
            }

            fields.Insert(index, FieldFormat.create(name, type));
        }

        public char getFieldType(int index)
        {
            return fields[index].getType();
        }

        public string getFieldName(int index)
        {
            return fields[index].getName();
        }

        public int getFieldIndex(string name)
        {
            foreach (var ff in fields)
            {
                if (ff.getName().Equals(name))
                {
                    return fields.IndexOf(ff);
                }
            }
            return -1;
        }

        public int getFieldCount()
        {
            return fields.Count;
        }

        public List<FieldFormat> getFields()
        {
            return this.fields;
        }

        public List<RecordValidator> getRecordValidators()
        {
            return recordValidators;
        }

        public List<TableValidator> getTableValidators()
        {
            return tableValidators;
        }

        public int getMaxRecords()
        {
            return maxRecords;
        }

        public int getMinRecords()
        {
            return minRecords;
        }

        public bool isReorderable()
        {
            return reorderable;
        }

        public Expression getNamingExpression()
        {
            return namingExpression;
        }

        public string encode(bool useVisibleSeparators)
        {
            return encode(new ClassicEncodingSettings(useVisibleSeparators));
        }

        public string encode(ClassicEncodingSettings settings)
        {
            return encode(settings, false, 0);
        }

        public string encode(ClassicEncodingSettings settings, Boolean isTransferEncode, int encodeLevel)
        {
            StringBuilder formatString = new StringBuilder(getFieldCount() * 7);

            encode(formatString, settings, isTransferEncode, encodeLevel);

            return formatString.ToString();
        }

        public StringBuilder encode(StringBuilder builder, ClassicEncodingSettings settings, Boolean isTransferEncode, int encodeLevel)
        {
            for (int i = 0; i < fields.Count; i++)
            {
                new Element(null, getField(i).encode(settings)).encode(builder, settings, isTransferEncode, encodeLevel);
            }

            if (minRecords != DEFAULT_MIN_RECORDS)
            {
                new Element(ELEMENT_MIN_RECORDS, minRecords.ToString()).encode(builder, settings, isTransferEncode, encodeLevel);
            }

            if (maxRecords != DEFAULT_MAX_RECORDS)
            {
                new Element(ELEMENT_MAX_RECORDS, maxRecords.ToString()).encode(builder, settings, isTransferEncode, encodeLevel);
            }

            if (tableValidators.Count > 0)
            {
                new Element(ELEMENT_TABLE_VALIDATORS, getEncodedTableValidators(settings)).encode(builder, settings, isTransferEncode, encodeLevel);
            }

            if (recordValidators.Count > 0)
            {
                new Element(ELEMENT_RECORD_VALIDATORS, getEncodedRecordValidators(settings)).encode(builder, settings, isTransferEncode, encodeLevel);
            }

            if (bindings.Count > 0)
            {
                new Element(ELEMENT_BINDINGS, getEncodedBindings(settings)).encode(builder, settings, isTransferEncode, encodeLevel);
            }

            if (namingExpression != null)
            {
                new Element(ELEMENT_NAMING, namingExpression == null ? "" : namingExpression.getText()).encode(builder, settings, isTransferEncode, encodeLevel);
            }

            encAppend(builder, ELEMENT_FLAGS, getEncodedFlags(), settings);

            return builder;
        }

        public FieldFormat getField(int index)
        {
            return fields.ToArray()[index];
        }

        public bool isBindingsEditable()
        {
            return bindingsEditable;
        }

        public void setBindingsEditable(bool bindingsEditable)
        {
            // Bindings Editable flag only affects visual editing of the table, so there's no need to condider immutability here

            this.bindingsEditable = bindingsEditable;
        }

        public List<Binding> getBindings()
        {
            return bindings;
        }

        public void addBinding(Binding binding)
        {
            bindings.Add(binding);
        }

        public void addBinding(Reference target, Expression expression)
        {
            addBinding(new Binding(target, expression));
        }

        public void addBinding(string target, string expression)
        {
            addBinding(new Binding(new Reference(target), new Expression(expression)));
        }

        public void removeBinding(Binding binding)
        {
            bindings.Remove(binding);
        }

        public void setBindings(List<Binding> in_bindings)
        {
            bindings = in_bindings;
        }
  
        private static void encAppend(StringBuilder buffer, string name, string value, ClassicEncodingSettings settings)
        {
            if (!string.IsNullOrEmpty(value))
            {
                buffer.Append(new Element(name, value).encode(settings));
            }
        }

        private string getEncodedFlags()
        {
            var buf = new StringBuilder();
            if (isReorderable())
            {
                buf.Append(REORDERABLE_FLAG);
            }
            if (isBindingsEditable())
            {
                buf.Append(BINDINGS_EDITABLE_FLAG);
            }
            return buf.ToString();
        }

        private string getEncodedTableValidators(ClassicEncodingSettings settings)
        {
            var enc = new StringBuilder();

            foreach (var tv in tableValidators)
            {
                if (tv.getType() != null)
                {
                    enc.Append(
                        new Element(tv.getType().ToString(), tv.encode()).encode(settings));
                }
            }

            return enc.ToString();
        }

        private string getEncodedRecordValidators(ClassicEncodingSettings settings)
        {
            var enc = new StringBuilder();

            foreach (var rv in recordValidators)
            {
                if (rv.getType() != null)
                {
                    enc.Append(
                        new Element(rv.getType().ToString(), rv.encode()).encode(settings));
                }
            }

            return enc.ToString();
        }

        public override string ToString()
        {
            return encode(new ClassicEncodingSettings(true));
        }

        private int findIndex(string name)
        {
            var index = 0;
            foreach (var ff in this)
            {
                if (ff.getName().Equals(name))
                {
                    return index;
                }
                index++;
            }

            return -1;
        }

        public FieldFormat getFieldFormat(int index)
        {
            return fields[index];
        }

        public FieldFormat getFieldFormat(string fieldName)
        {
            var index = findIndex(fieldName);
            return index != -1 ? getFieldFormat(index) : null;
        }

        public bool hasField(string name)
        {
            return findIndex(name) != -1;
        }

        public bool hasFields(char type)
        {
            foreach (var ff in this)
            {
                if (ff.getType() == type)
                {
                    return true;
                }
            }

            return false;
        }

        public bool hasReadOnlyFields()
        {
            foreach (var ff in this)
            {
                if (ff.isReadonly())
                {
                    return true;
                }
            }

            return false;
        }

        public List<string> getKeyFields()
        {
            var keyFields = new List<string>();

            foreach (var ff in this)
            {
                if (ff.isKeyField())
                {
                    keyFields.Add(ff.getName());
                }
            }

            return keyFields;
        }

        public bool extend(TableFormat other)
        {
            return extendMessage(other) == null;
        }

        public string extendMessage(TableFormat other)
        {
            if (!Util.equals(getNamingExpression(), other.getNamingExpression()))
            {
                return "Different naming expressions: need " + getNamingExpression() + ", found " +
                       other.getNamingExpression();
            }

            foreach (Binding otherBinding in other.getBindings())
            {
                if (!getBindings().Contains(otherBinding))
                {
                    return "Different bindings: need " + getBindings() + ", found " + other.getBindings();
                }
            }

            for (var i = 0; i < other.getFieldCount(); i++)
            {
                var otherFormat = other.getFieldFormat(i);
                var fieldName = other.getFieldName(i);

                var ownFormat = getFieldFormat(fieldName);

                if (ownFormat == null)
                {
                    if (otherFormat.isOptional())
                    {
                        continue;
                    }
                    return "Required field doesn't exist: " + fieldName;
                }

                var fieldExtendMessage = ownFormat.extendMessage(otherFormat);

                if (fieldExtendMessage != null)
                {
                    return "Incorrect format of field '" + fieldName + "': " + fieldExtendMessage;
                }
            }

            return null;
        }

        public void addTableValidator(TableValidator tv)
        {
            tableValidators.Add(tv);
        }

        public void addRecordValidator(RecordValidator rv)
        {
            recordValidators.Add(rv);
        }

        private string getEncodedBindings(ClassicEncodingSettings settings)
        {
            StringBuilder enc = new StringBuilder();

            foreach (Binding bin in bindings)
            {
                enc.Append(new Element(bin.getTarget().getImage(), bin.getExpression().getText()).encode(settings));
            }
    
            return enc.ToString();
        }

        private void createTableValidators(string source, ClassicEncodingSettings settings)
        {
            if (string.IsNullOrEmpty(source))
            {
                return;
            }

            var validatorsData = StringUtils.elements(source, settings.isUseVisibleSeparators());

            foreach (var el in validatorsData)
            {
                var validatorType = el.getName()[0];

                switch (validatorType)
                {
                    case TABLE_VALIDATOR_KEY_FIELDS:
                        addTableValidator(new TableKeyFieldsValidator());
                        break;
                }
            }
        }

        private void createRecordValidators(string source, ClassicEncodingSettings settings)
        {
            if (string.IsNullOrEmpty(source))
            {
                return;
            }

            var validatorsData = StringUtils.elements(source, settings.isUseVisibleSeparators());

            foreach (var el in validatorsData)
            {
                var validatorType = el.getName()[0];

                switch (validatorType)
                {
                    case RECORD_VALIDATOR_KEY_FIELDS:
                        addRecordValidator(new KeyFieldsValidator());
                        break;
                }
            }
        }

        private void createNaming(string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return;
            }

            namingExpression = new Expression(source);
        }

        public bool isReplicated()
        {
            var isReplicated = false;

            foreach (var ff in this)
            {
                if (ff.isNotReplicated()) continue;
                isReplicated = true;
                break;
            }

            return isReplicated;
        }

        public void resetAllowedRecords()
        {
            minRecords = DEFAULT_MIN_RECORDS;
            maxRecords = DEFAULT_MAX_RECORDS;
        }

        public TableFormat setMaxRecords(int maxRecordsInteger)
        {
            //if (immutable)
            //{
            //    throw new IllegalStateException("Immutable");
            //}

            this.maxRecords = maxRecordsInteger;

            return this;
        }

        public TableFormat setMinRecords(int minRecordsInteger)
        {
            //if (immutable)
            //{
            //    throw new IllegalStateException("Immutable");
            //}

            this.minRecords = minRecordsInteger;

            return this;
        }

        public void setReorderable(bool reorderableBool)
        {
            reorderable = reorderableBool;
        }

        public void setNamingExpression(Expression namingExpressionString)
        {
            namingExpression = namingExpressionString;
        }

        public void setNamingExpression(string namingExpressionString)
        {
            setNamingExpression(new Expression(namingExpressionString));
        }

        public override int GetHashCode()
        {
            const int prime = 31;
            int result = 1;

            result = prime * result + maxRecords;
            result = prime * result + minRecords;
            result = prime * result + ((fields == null) ? 0 : this.fields.GetHashCode());
            result = prime * result + ((namingExpression == null) ? 0 : namingExpression.GetHashCode());
            result = prime * result + ((recordValidators == null) ? 0 : recordValidators.GetHashCode());
            result = prime * result + ((tableValidators == null) ? 0 : tableValidators.GetHashCode());
            result = prime * result + (reorderable ? 1231 : 1237);
//            result = prime * result + (unresizable ? 1231 : 1237);
            result = prime * result + ((bindings == null) ? 0 : bindings.GetHashCode());

            if (result < 0)
            {
                result = int.MaxValue + result;
            }
            return result;
        }

        public override bool Equals(Object obj)
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
            var other = (TableFormat) obj;
            if (fields == null)
            {
                if (other.fields != null)
                {
                    return false;
                }
            }
            else if (!fields.SequenceEqual<FieldFormat>(other.fields))
            {
                return false;
            }
            if (maxRecords != other.maxRecords)
            {
                return false;
            }
            if (minRecords != other.minRecords)
            {
                return false;
            }
            if (namingExpression == null)
            {
                if (other.namingExpression != null)
                {
                    return false;
                }
            }
            else if (!namingExpression.Equals(other.namingExpression))
            {
                return false;
            }
            if (recordValidators == null)
            {
                if (other.recordValidators != null)
                {
                    return false;
                }
            }
            else if (!recordValidators.SequenceEqual(other.recordValidators))
            {
                return false;
            }
            if (reorderable != other.reorderable)
            {
                return false;
            }
            if (tableValidators == null)
            {
                if (other.tableValidators != null)
                {
                    return false;
                }
            }
            else if (!tableValidators.SequenceEqual(other.tableValidators))
            {
                return false;
            }
            if (bindingsEditable != other.bindingsEditable)
            {
                return false;
            }
            if (bindings == null)
            {
                if (other.bindings != null)
                {
                    return false;
                }
            }
            else if (!bindings.SequenceEqual(other.bindings))
            {
                return false;
            }
            return true;
        }


        public object Clone()
        {
            TableFormat cl;

            try
            {
                cl = (TableFormat)this.MemberwiseClone();
            }
            catch (CloneNotSupportedException ex)
            {
                throw new InvalidOperationException(ex.Message, ex);
            }

            cl.fields = (List<FieldFormat>) CloneUtils.deepClone(fields);
            cl.recordValidators = (List<RecordValidator>) CloneUtils.deepClone(recordValidators);
            cl.tableValidators = (List<TableValidator>) CloneUtils.deepClone(tableValidators);

            cl.id = null; // Need to clear ID to avoid conflicts in format cache
            cl.immutable = false;

            return cl;
        }

        public void makeImmutable(DataTable immutabilizer)
        {
            if (immutable)
            {
                return;
            }

            immutable = true;

            this.immutabilizer = immutabilizer;

            foreach (FieldFormat ff in fields)
            {
                ff.makeImmutable();
            }
        }

        public IEnumerator<FieldFormat> GetEnumerator()
        {
            return this.fields.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Int32? getId()
        {
            return id;
        }

        public bool isImmutable()
        {
            return immutable;
        }

        public void setId(Int32 id)
        {
            // We consider all formats as immutable now. This can cause some bugs if we cache mutable formats
            if (!immutable)
            {
                throw new ContextRuntimeException("Cannot set ID of non-immutable format");
            }

            if (this.id != null && !this.id.Equals(id))
            {
                throw new ContextRuntimeException("Format already has ID " + this.id + ", new ID " + id);
            }

            this.id = id;
        }


    }
}
