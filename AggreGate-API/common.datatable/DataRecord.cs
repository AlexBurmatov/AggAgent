using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using com.tibbo.aggregate.common.data;
using com.tibbo.aggregate.common.util;

namespace com.tibbo.aggregate.common.datatable
{
    using com.tibbo.aggregate.common.datatable.field;

    public class DataRecord : ICloneable, StringEncodable
    {
        private const string ELEMENT_ID = "I";

        private Dictionary<string, Object> data = new Dictionary<string, Object>();
        private TableFormat format = new TableFormat();
        private string id;

        private DataTable table;

        public DataRecord()
        {
        }

        public DataRecord(TableFormat tableFormat)
        {
            if (tableFormat != null)
            {
                tableFormat.makeImmutable(null);
                format = tableFormat;
            }
        }

        public DataRecord(TableFormat tableFormat, params Object[] data) : this(tableFormat)
        {
            foreach (var param in data)
            {
                this.addValue(param);
            }
        }

        public DataRecord(TableFormat tableFormat, string dataString, ClassicEncodingSettings settings, bool validate,
                          IList<string> fieldNamesInData) : this(tableFormat)
        {
            setData(dataString, settings, validate, fieldNamesInData);
        }

        public DataRecord(TableFormat tableFormat, string dataString)
            : this(tableFormat, dataString, new ClassicEncodingSettings(false), true, null)
        {
        }

        private void setData(string dataString, ClassicEncodingSettings settings, Boolean validate,
                             IList<string> fieldNamesInData)
        {
            var recs = StringUtils.elements(dataString, settings.isUseVisibleSeparators());

            var i = 0;

            foreach (var el in recs)
            {
                if (el.getName() != null)
                {
                    if (el.getName().Equals(ELEMENT_ID))
                    {
                        setId(el.getValue());
                    }
                    else
                    {
                        // This code exists for compatibility reason only
                        var ff = format.getFieldFormat(el.getName());
                        if (ff != null)
                        {
                            setValue(el.getName(), ff.valueFromEncodedString(el.getValue(), settings, validate),
                                     validate);
                        }
                    }
                }
                else
                {
                    if (fieldNamesInData != null && fieldNamesInData.Count > i)
                    {
                        var fieldName = fieldNamesInData[i];
                        if (getFormat().hasField(fieldName))
                        {
                            var value = format.getFieldFormat(fieldName).valueFromEncodedString(el.getValue(), settings, validate);
                            setValue(fieldName, value, validate);
                        }
                    }
                    else if (i < format.getFieldCount())
                    {
                        var value = format.getFieldFormat(i).valueFromEncodedString(el.getValue(), settings, validate);
                        setValue(i, value, validate);
                    }
                    i++;
                }
            }
        }

        public int getFieldCount()
        {
            return format == null ? 0 : format.getFieldCount();
        }

        public TableFormat getFormat()
        {
            return format;
        }

        public FieldFormat getFormat(int index)
        {
            return format.getFieldFormat(index);
        }

        public FieldFormat getFormat(string name)
        {
            return format.getFieldFormat(name);
        }

        public string getId()
        {
            return id;
        }

        public DataTable getTable()
        {
            return table;
        }

        public string encode(bool useVisibleSeparators)
        {
            return encode(new ClassicEncodingSettings(useVisibleSeparators));
        }

        public string encode(ClassicEncodingSettings settings)
        {
            return encode(new StringBuilder(), settings, false, 0).ToString();
        }

        public StringBuilder encode(StringBuilder sb, ClassicEncodingSettings settings, Boolean isTransferEncode, int encodeLevel)
        {
            if (getId() != null)
            {
                new Element(ELEMENT_ID, getId()).encode(sb, settings, isTransferEncode, encodeLevel);
            }

            for (int i = 0; i < format.getFieldCount(); i++)
            {
                FieldFormat ff = format.getField(i);

                Object value = getValue(ff);

                new Element(null, ff, value).encode(sb, settings, isTransferEncode, encodeLevel);
            }

            return sb;
        }

        private void checkNumberOfDataFieldsSet()
        {
            if (data.Count >= format.getFieldCount())
            {
                throw new InvalidOperationException(
                    "Can't add data to data record: all data fields defined by format are already set");
            }
        }

        public DataRecord addInt(Int32 val)
        {
            checkNumberOfDataFieldsSet();
            return setValue(data.Count, val);
        }

        public DataRecord addString(string val)
        {
            checkNumberOfDataFieldsSet();
            return setValue(data.Count, val);
        }

        public DataRecord addBoolean(Boolean val)
        {
            checkNumberOfDataFieldsSet();
            return setValue(data.Count, val);
        }

        public DataRecord addLong(Int64 val)
        {
            checkNumberOfDataFieldsSet();
            return setValue(data.Count, val);
        }

        public DataRecord addFloat(float val)
        {
            checkNumberOfDataFieldsSet();
            return setValue(data.Count, val);
        }

        public DataRecord addDate(DateTime val)
        {
            checkNumberOfDataFieldsSet();
            return setValue(data.Count, val);
        }

        public DataRecord addDataTable(DataTable val)
        {
            checkNumberOfDataFieldsSet();
            return setValue(data.Count, val);
        }

        public DataRecord addColor(Color val)
        {
            checkNumberOfDataFieldsSet();
            return setValue(data.Count, val);
        }

        public DataRecord addData(Data val)
        {
            checkNumberOfDataFieldsSet();
            return setValue(data.Count, val);
        }

        public DataRecord setValue(int index, Object value)
        {
            return setValue(index, value, true);
        }

        private DataRecord setValue(int index, Object value, Boolean validate)
        {
            FieldFormat ff = getFormat().getFieldFormat(index);

            try
            {
                ff.checkAndConvertValue(value, validate);
            }
            catch (ValidationException ex)
            {
                throw new ArgumentException(
                    string.Format(Cres.get().getString("dtIllegalFieldValue"), value, ff) + ex.Message, ex);
            }

            Object oldValue = null;
            if (data.ContainsKey(ff.getName()))
                oldValue = data[ff.getName()];

            try
            {
                data[ff.getName()] = value;
                if (table != null)
                {
                    table.validateRecord(this);
                }
            }
            catch (ValidationException ex1)
            {
                data.Add(ff.getName(), oldValue);
                throw new ArgumentException(ex1.Message, ex1);
            }

            return this;
        }


        public DataRecord setValue(string name, Object value)
        {
            return setValue(findIndex(name), value, true);
        }

        public DataRecord setValue(string name, Object value, Boolean validate)
        {
            return setValue(findIndex(name), value, validate);
        }

        public DataRecord setValueSmart(string name, Object value)
        {
            var ff = getFormat().getFieldFormat(name);

            if (value == null || ff.getFieldClass().Equals(value.GetType()))
            {
                return setValue(ff.getName(), value);
            }
            var stringValue = value.ToString();
            try
            {
                return setValue(ff.getName(), ff.valueFromString(stringValue));
            }
            catch (Exception ex)
            {
                if (ff.getSelectionValues() != null)
                {
                    foreach (var sv in ff.getSelectionValues())
                    {
                        var svdesc = sv.Value;
                        if (stringValue.Equals(svdesc))
                        {
                            return setValue(ff.getName(), sv);
                        }
                    }
                }
                throw new ArgumentException(ex.Message, ex);
            }
        }

        public DataRecord setValueSmart(int index, Object value)
        {
            var ff = getFormat(index);
            return setValueSmart(ff.getName(), value);
        }

        public DataRecord addValue(Object value)
        {
            checkNumberOfDataFieldsSet();
            return setValue(data.Count, value);
        }

        private int findIndex(string name)
        {
            int index = 0;
            for (var iter = format.getFields().GetEnumerator(); iter.MoveNext();)
            {
                var ff = iter.Current;
                if (ff.getName().Equals(name))
                {
                    return index;
                }
                index++;
            }
            throw new ArgumentException(string.Format(Cres.get().getString("dtFieldNotFound"), name));
        }

        public string getString(string name)
        {
            return getString(findIndex(name));
        }

        public string getString(int index)
        {
            return (string)getValue(index);
        }

        public Int32 getInt(string name)
        {
            return getInt(findIndex(name));
        }

        public Int32 getInt(int index)
        {
            return (Int32)getValue(index);
        }

        public bool? getBoolean(string name)
        {
            return getBoolean(findIndex(name));
        }

        public Boolean getBoolean(int index)
        {
            return (Boolean)getValue(index);
        }

        public Int64 getLong(string name)
        {
            return getLong(findIndex(name));
        }

        public Int64 getLong(int index)
        {
            return (Int64)getValue(index);
        }

        public float getFloat(string name)
        {
            return getFloat(findIndex(name));
        }

        public float getFloat(int index)
        {
            return (float)getValue(index);
        }

        public DateTime getDate(string name)
        {
            return getDate(findIndex(name));
        }

        public DateTime getDate(int index)
        {
            return (DateTime)getValue(index);
        }

        public DataTable getDataTable(string name)
        {
            return getDataTable(findIndex(name));
        }

        public DataTable getDataTable(int index)
        {
            return (DataTable)getValue(index);
        }

        public Color getColor(string name)
        {
            return getColor(findIndex(name));
        }

        public Color getColor(int index)
        {
            return (Color)getValue(index);
        }

        public Data getData(string name)
        {
            return getData(findIndex(name));
        }

        public Data getData(int index)
        {
            return (Data)getValue(index);
        }

        public Object getValue(int index)
        {
            FieldFormat ff = format.getFieldFormat(index);

            if (data.ContainsKey(ff.getName()))
            {
                return data[ff.getName()];
            }

            return ff.getDefaultValueCopy();
        }

        public Object getValue(string name)
        {
            return getValue(findIndex(name));
        }

        public Object getValue(FieldFormat ff)
        {
            if (data.ContainsKey(ff.getName()))
            {
                return data[ff.getName()];
            }

            return ff.isDefaultOverride() ? null : ff.getDefaultValueCopy();
        }



        public DataRecord setId(string idString)
        {
            id = idString;
            return this;
        }

        protected internal void setTable(DataTable aDataTable)
        {
            table = aDataTable;
        }

        public void setFormat(TableFormat aTableFormat)
        {
            format.makeImmutable(null);
            format = aTableFormat;
        }

        public override Boolean Equals(Object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (!(obj is DataRecord))
            {
                return false;
            }

            var rec = (DataRecord)obj;

            if (!Util.equals(getId(), rec.getId()))
            {
                return false;
            }

            // Formats are compared only if record does not belong to a table
            if (table == null)
            {
                if (!format.Equals(rec.getFormat()))
                {
                    return false;
                }
            }

            for (var i = 0; i < getFieldCount(); i++)
            {
                var field = getValue(i);
                var value = rec.getValue(i);
                if (field != null ? !field.Equals(value) : value != null)
                {
                    return false;
                }
            }

            return true;
        }

        public Boolean hasField(string name)
        {
            return getFormat().hasField(name);
        }

        public Boolean meetToCondition(QueryCondition cond)
        {
            if (hasField(cond.getField()))
            {
                var val = getValue(cond.getField());
                if (val == null)
                {
                    if (cond.getOperator() != QueryCondition.EQ)
                    {
                        throw new ArgumentException("Can't compare value to NULL");
                    }

                    return cond.getValue() == null;
                }

                if ((cond.getOperator() & QueryCondition.EQ) > 0)
                {
                    return val.Equals(cond.getValue());
                }
                if (!(val is IComparable))
                {
                    throw new ArgumentException("value isn't Comparable: " + val);
                }

                var comp = (IComparable)val;

                if ((cond.getOperator() & QueryCondition.GT) > 0)
                {
                    return comp.CompareTo(cond.getValue()) > 0;
                }
                if ((cond.getOperator() & QueryCondition.LT) > 0)
                {
                    return comp.CompareTo(cond.getValue()) < 0;
                }

                throw new ArgumentException("Illegal operator " + cond.getOperator());
            }

            return false;
        }

        public void cloneFormatFromTable()
        {
            if (table != null)
            {
                format = table.getFormat().Clone() as TableFormat;
            }
            else
            {
                throw new InvalidOperationException("Table not defined");
            }
        }


        public override string ToString()
        {
            return this.dataAsString(true, true);
        }

        public string dataAsString(bool showFieldNames, bool showHiddenFields)
        {
            return this.dataAsString(showFieldNames, showHiddenFields, true);
        }

        public string dataAsString(bool showFieldNames, bool showHiddenFields, bool showPasswords)
        {
            var res = new StringBuilder();

            var needSeparator = false;

            for (var j = 0; j < this.getFieldCount(); j++)
            {
                var ff = getFormat().getFieldFormat(j);

                if (ff.isHidden() && !showHiddenFields)
                {
                    continue;
                }

                if (needSeparator)
                {
                    res.Append(", ");
                }
                else
                {
                    needSeparator = true;
                }

                var value = this.valueAsString(ff.getName(), showFieldNames, showHiddenFields, showPasswords);




                if (StringFieldFormat.EDITOR_PASSWORD.Equals(ff.getEditor()) && !showPasswords)
                {
                    StringBuilder buf = new StringBuilder();

                    for (int i = 0; i < value.Length; ++i)
                    {
                        buf.Append('\u2022');
                    }

                    value = buf.ToString();
                }



                res.Append((showFieldNames ? ff + "=" : "") + value);
            }

            return res.ToString();
        }

        public string valueAsString(string name)
        {
            return valueAsString(name, true, false, true);
        }

        public string valueAsString(string name, bool showFieldNames, bool showHiddenFields, bool showPasswords)
        {
            var ff = this.getFormat(name);

            var val = this.getValue(name);

            var value = val != null ? (FieldFormat.DATATABLE_FIELD == ff.getType()) 
                ? ((DataTable)val).dataAsString(showFieldNames, showHiddenFields, showPasswords) 
                : val.ToString() : "NULL";

            if (ff.hasSelectionValues())
            {
                object sv = ff.getSelectionValues().ContainsKey(new NullableObject(val)) ? ff.getSelectionValues()[new NullableObject(val)] : null;
                value = sv != null ? sv.ToString() : value;
            }

            return value;
        }


        public DataTable wrap()
        {
            return new DataTable(this);
        }

        public object Clone()
        {
            var cl = (DataRecord)MemberwiseClone();
            cl.data = (Dictionary<string, object>)CloneUtils.deepClone(data);
            return cl;
        }

        public override int GetHashCode()
        {
            var result = (data != null ? data.GetHashCode() : 0);
            result = (result * 397) ^ (format != null ? format.GetHashCode() : 0);
            result = (result * 397) ^ (id != null ? id.GetHashCode() : 0);
            result = (result * 397) ^ (table != null ? table.GetHashCode() : 0);
            return result;
        }
    }
}