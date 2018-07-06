namespace com.tibbo.aggregate.common.datatable
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Text;

    using com.tibbo.aggregate.common.datatable.field;

    using context;
    using validator;

    using util;

    public class DataTable : IEnumerable<DataRecord>, ICloneable
    {
        private const string ELEMENT_FIELD_NAME = "N";

        private const string ELEMENT_FORMAT = "F";

        private const string ELEMENT_FORMAT_ID = "D";

        private const string ELEMENT_INVALIDATOR = "I";

        private const string ELEMENT_QUALITY = "Q";

        private const string ELEMENT_RECORD = "R";

        private const string ELEMENT_TIMESTAMP = "T";

        private TableFormat format = new TableFormat();

        private long? id;

        private string invalidationMessage;

        private int? quality = null;

        private List<DataRecord> records = new AgList<DataRecord>();

        private DateTime? timestamp = null;

        public DataTable()
        {
        }

        public DataTable(TableFormat format)
        {
            this.setFormat(format);
        }

        public DataTable(TableFormat format, int emptyRecords)
            : this(format)
        {
            for (var i = 0; i < emptyRecords; i++) this.addRecord();
        }

        public DataTable(TableFormat format, bool createEmptyRecords)
            : this(format, createEmptyRecords ? format != null ? format.getMinRecords() : 0 : 0)
        {
        }

        public DataTable(DataRecord record)
        {
            this.addRecord(record);
        }

        public DataTable(TableFormat format, string dataString, ClassicEncodingSettings settings)
        {
            if (dataString == null)
            {
                throw new NullReferenceException("Data string is null");
            }

            this.setFormat(format);

            List<string> fieldNames = null;
            bool flag = false;
            if (settings != null) flag = settings.isUseVisibleSeparators();

            var recs = StringUtils.elements(dataString, flag);

            foreach (var el in recs)
                if (el.getName().Equals(ELEMENT_FIELD_NAME))
                {
                    if (fieldNames == null)
                    {
                        fieldNames = new List<string>();
                    }
                    fieldNames.Add(el.getValue());
                }
                else if (el.getName() != null && el.getName().Equals(ELEMENT_RECORD))
                {
                    this.addRecord(new DataRecord(this.getFormat(), el.getValue(), settings, true, fieldNames));
                }
                else
                {
                    this.decodeAdvancedElement(el);
                }
        }



        public DataTable(string format, string dataString)
            : this(new TableFormat(format, new ClassicEncodingSettings(true)), dataString)
        {
        }

        public DataTable(TableFormat format, params object[] firstRowData)
            : this(format)
        {
            this.addRecord(new DataRecord(format, firstRowData));
        }

        public DataTable(string data)
            : this(data, true)
        {
        }

        public DataTable(string data, bool validate)
            : this(data, new ClassicEncodingSettings(false), validate)
        {
        }

        public DataTable(string data, ClassicEncodingSettings settings, bool validate)
        {
            if (data == null)
            {
                return;
            }

            var found = false;
            string encodedFormat = null;
            List<string> fieldNames = null;

            bool flag = false;
            if (settings != null) flag = settings.isUseVisibleSeparators();

            var recs = StringUtils.elements(data, flag);

            foreach (var el in recs)
            {
                if (el.getName() == null)
                {
                    continue;
                }

                if (el.getName().Equals(ELEMENT_FORMAT_ID))
                {
                    var formatId = int.Parse(el.getValue());

                    if (settings == null || settings.getFormatCache() == null) throw new InvalidOperationException("Can't use format ID - format cache not found");

                    if (encodedFormat != null)
                    {
                        // If format was already found in the encoded data
                        var newFormat1 = new TableFormat(encodedFormat, settings, validate);
                        settings.getFormatCache().put(formatId, newFormat1);
                        continue;
                    }

                    var newFormat = settings.getFormatCache().get(formatId);

                    // encodedFormat = settings.getFormatCache().get(formatId);
                    if (newFormat == null)
                        throw new InvalidOperationException(
                            "Format with specified ID not found in the cache: " + formatId);

                    this.setFormat(newFormat);
                    found = true;
                    continue;
                }

                if (el.getName().Equals(ELEMENT_FORMAT))
                {
                    encodedFormat = el.getValue();
                    setFormat(new TableFormat(encodedFormat, settings, validate));
                    found = true;
                    continue;
                }

                if (el.getName().Equals(ELEMENT_RECORD))
                {
                    // Using table's format if encodedFormat is not NULL (i.e. was found in the encoded data)
                    var fmt = found ? this.getFormat() : (settings != null ? settings.getFormat() : null);

                    if (fmt == null)
                        throw new InvalidOperationException(
                            "Table format is neither found in encoded table nor provided by decoding environment");

                    addRecord(new DataRecord(fmt, el.getValue(), settings, validate, fieldNames));
                    continue;
                }

                if (el.getName().Equals(ELEMENT_FIELD_NAME))
                {
                    if (fieldNames == null) fieldNames = new List<string>();
                    fieldNames.Add(el.getValue());
                    continue;
                }

                if (el.getName().Equals(ELEMENT_TIMESTAMP))
                {
                    var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    this.timestamp = epoch.AddSeconds(Convert.ToDouble(el.getValue()) / 1000).AddMilliseconds(Convert.ToDouble(el.getValue()) % 1000);
                    continue;
                }

                if (el.getName().Equals(ELEMENT_QUALITY))
                {
                    this.quality = Convert.ToInt32(el.getValue());
                    continue;
                }

                if (el.getName().Equals(ELEMENT_INVALIDATOR)) this.invalidationMessage = el.getValue();
            }
        }

        public DataTable addRecord(DataRecord record)
        {
            this.checkOrSetFormat(record);
            this.addRecordImpl(null, record);
            return this;
        }

        public DataTable addRecord(int index, DataRecord record)
        {
            this.checkOrSetFormat(record);
            this.addRecordImpl(index, record);
            return this;
        }

        public DataRecord addRecord()
        {
            if (this.getFormat() == null) throw new InvalidOperationException("Can't add empty record because format of data table was not set");
            var record = new DataRecord(this.getFormat());
            this.addRecordImpl(null, record);
            return record;
        }

        public bool conform(TableFormat rf)
        {
            return this.conformMessage(rf) == null;
        }

        public string conformMessage(TableFormat rf)
        {
            if (this.getRecordCount() < rf.getMinRecords())
                return "Number of records too small: need " + rf.getMinRecords() + " or more, found "
                       + this.getRecordCount();

            if (this.getRecordCount() > rf.getMaxRecords())
                return "Number of records too big: need " + rf.getMaxRecords() + " or less, found "
                       + this.getRecordCount();

            return this.getFormat().extendMessage(rf);
        }

        public string dataAsString()
        {
            return this.dataAsString(true, false, true);
        }

        public string dataAsString(bool showFieldNames, bool showHiddenFields, bool showPasswords)
        {
            var res = new StringBuilder();

            var recordSeparator = this.getFieldCount() > 1 ? " | " : ", ";

            for (var i = 0; i < this.getRecordCount(); i++)
            {
                if (i > 0)
                {
                    res.Append(recordSeparator);
                }

                var rec = this.getRecord(i);

                res.Append(rec.dataAsString(showFieldNames, showHiddenFields));
            }

            return res.ToString();
        }

        public string encode()
        {
            return this.encode(new ClassicEncodingSettings(false));
        }

        public string encode(bool useVisibleSeparators)
        {
            return this.encode(new ClassicEncodingSettings(useVisibleSeparators));
        }

        public string encode(ClassicEncodingSettings settings)
        {
            return encode(new StringBuilder(getEstimateDataSize()), settings, false, 0).ToString();
        }

        private int getEstimateDataSize()
        {
            return getFieldCount() * getRecordCount() * 3 + getFieldCount() * 7;
        }

        public StringBuilder encode(StringBuilder finalSB, ClassicEncodingSettings settings, bool isTransferEncode, int encodeLevel)
        {
            if (finalSB.Length + this.getEstimateDataSize() > finalSB.Capacity)
            {
                finalSB.EnsureCapacity(finalSB.Capacity + getEstimateDataSize());
            }

            int? formatId = null;

            bool formatWasInserted = false;

            bool needToInsertFormat = settings != null && settings.isEncodeFormat();

            bool isKnown = false;

            KnownFormatCollector collector = settings != null ? settings.getKnownFormatCollector() : null;


            if (needToInsertFormat)
            {
                if (this.getFormat().getFieldCount() > 0 && settings.getFormatCache() != null)
                {
                    formatId = settings.getFormatCache().addIfNotExists(this.getFormat());

                    if (collector != null)
                    {
                        needToInsertFormat = false;

                        if (collector.isKnown(formatId) && collector.isMarked(formatId))
                        {
                            // Format is known - inserting ID only
                            new Element(ELEMENT_FORMAT_ID, formatId.ToString()).encode(finalSB, settings, isTransferEncode, encodeLevel);

                            isKnown = true;
                        }
                        else
                        {
                            var oldEncodeFormat = settings.isEncodeFormat();
                            settings.setEncodeFormat(true);

                            try
                            {
                                // Format is not known - inserting both format and ID
                                new Element(ELEMENT_FORMAT, this.getFormat()).encode(finalSB, settings, isTransferEncode, encodeLevel);
                                new Element(ELEMENT_FORMAT_ID, formatId.ToString()).encode(finalSB, settings, isTransferEncode, encodeLevel);

                                formatWasInserted = true;
                            }
                            finally
                            {
                                settings.setEncodeFormat((bool) oldEncodeFormat);
                            }
                        }
                    }
                }

                if (needToInsertFormat)
                {
                    var oldEncodeFormat = settings.isEncodeFormat();
                    settings.setEncodeFormat(true);
                    try
                    {
                        new Element(ELEMENT_FORMAT, this.getFormat()).encode(finalSB, settings, isTransferEncode, encodeLevel);

                        formatWasInserted = true;
                    }
                    finally
                    {
                        settings.setEncodeFormat((bool) oldEncodeFormat);
                    }
                }
            }

            bool? oldEncodeFormat1 = null;
            if (settings != null) oldEncodeFormat1 = settings.isEncodeFormat();
            try
            {
                if (formatWasInserted)
                    settings.setEncodeFormat(false);

                this.getEncodedData(finalSB, settings, isTransferEncode, encodeLevel + 1);
            }
            finally
            {
                if (oldEncodeFormat1 != null)
                    settings.setEncodeFormat((bool) oldEncodeFormat1);
            }

            if (isInvalid())
            {
                new Element(ELEMENT_FORMAT, this.invalidationMessage).encode(finalSB, settings, isTransferEncode, encodeLevel);
            }


            if (!isKnown && formatId != null && collector != null)
            {
                // Marking format as known
                collector.makeKnown((int) formatId, true);
            }

            return finalSB;
        }

        public bool equals(object obj)
        {
            if (obj == null) return false;

            if (!(obj is DataTable)) return false;

            var other = (DataTable)obj;

            if (!this.format.Equals(other.getFormat())) return false;

            if (this.getRecordCount() != other.getRecordCount()) return false;

            for (var i = 0; i < this.getRecordCount(); i++) if (!this.getRecord(i).Equals(other.getRecord(i))) return false;

            return true;
        }

        public int? findIndex(Query query)
        {
            for (var i = 0; i < this.getRecordCount(); i++)
            {
                var meet = true;

                var rec = this.getRecord(i);

                foreach (var cond in query.getConditions()) if (!rec.meetToCondition(cond)) meet = false;

                if (meet) return i;
            }

            return null;
        }

        public int? findIndex(string field, object value)
        {
            return this.findIndex(new Query(new QueryCondition(field, value)));
        }

        public void fixRecords()
        {
            this.getFormat().setMinRecords(this.getRecordCount());
            this.getFormat().setMaxRecords(this.getRecordCount());
        }

        public object get()
        {
            return this.getRecord(0).getValue(0);
        }

        public string getDescription()
        {
            var namingExpression = this.format == null ? null : this.format.getNamingExpression();

            if (namingExpression == null) return this.ToString();

            throw new NotImplementedException();
        }

        public String getEncodedData(ClassicEncodingSettings settings)
        {
            return this.getEncodedData(new StringBuilder(), settings, false, 0).ToString();
        }

        public StringBuilder getEncodedData(StringBuilder finalSB, ClassicEncodingSettings settings, Boolean isTransferEncode, int encodeLevel)
        {
            bool encodeFieldNames = (settings != null) ? settings.isEncodeFieldNames() : true;

            if (encodeFieldNames)
            {
                for (int i = 0; i < format.getFieldCount(); i++)
                {
                    new Element(ELEMENT_FIELD_NAME, format.getField(i).getName()).encode(finalSB, settings, isTransferEncode, encodeLevel);
                }
            }

            for (int i = 0; i < getRecordCount(); i++)
            {
                new Element(ELEMENT_RECORD, getRecord(i)).encode(finalSB, settings, isTransferEncode, encodeLevel);
            }
            

            if (quality != null)
            {
                new Element(ELEMENT_QUALITY, quality.ToString()).encode(finalSB, settings, isTransferEncode, encodeLevel);
            }

            if (timestamp != null)
            {
                new Element(ELEMENT_TIMESTAMP, ((DateTime)this.timestamp).ToString(DateFieldFormat.FORMAT)).encode(finalSB, settings, isTransferEncode, encodeLevel);
            }

            return finalSB;
        }

        public IEnumerator<DataRecord> GetEnumerator()
        {
            return new Enumerator(this);
        }

        public int getFieldCount()
        {
            return this.format.getFieldCount();
        }

        public TableFormat getFormat()
        {
            return this.format;
        }

        public FieldFormat getFormat(int field)
        {
            return this.getFormat().getFieldFormat(field);
        }

        public FieldFormat getFormat(string name)
        {
            return this.getFormat().getFieldFormat(name);
        }

        public long? getId()
        {
            return this.id;
        }

        public string getInvalidationMessage()
        {
            return this.invalidationMessage;
        }

        public int? getQuality()
        {
            return this.quality;
        }

        public DataRecord getRecord(int number)
        {
            return this.records[number];
        }

        public DataRecord getRecordById(string aString)
        {
            if (aString == null) return null;

            foreach (var rec in this) if (rec.getId() != null && rec.getId().Equals(aString)) return rec;

            return null;
        }

        public int getRecordCount()
        {
            return this.records.Count;
        }

        public ReadOnlyCollection<DataRecord> getRecords()
        {
            return this.records.AsReadOnly();
        }

        public DateTime? getTimestamp()
        {
            return this.timestamp;
        }

        public bool isInvalid()
        {
            return this.invalidationMessage != null;
        }

        public bool isOneCellTable()
        {
            return this.getFieldCount() == 1 && this.getRecordCount() == 1;
        }

        public void joinFormats()
        {
            foreach (var rec in this.records) rec.setFormat(this.getFormat());
        }

        public DataRecord rec()
        {
            return this.getRecord(0);
        }

        public void removeRecord(int index)
        {
            var rec = this.records[index];
            if (rec != null) this.removeRecordImpl(rec);
        }

        public void removeRecord(DataRecord rec)
        {
            this.removeRecordImpl(rec);
        }

        public void removeRecords(Predicate<DataRecord> func)
        {
            this.records.RemoveAll(func);
        }

        public void reorderRecord(DataRecord record, int index)
        {
            var oi = this.records.IndexOf(record);

            if (oi == -1) throw new InvalidOperationException("Record is not from this table");

            if (this.records.Remove(record)) this.records.Insert(index - (oi < index ? 1 : 0), record);
        }

        public DataRecord select(Query query)
        {
            foreach (var rec in this)
            {
                var meet = true;

                foreach (var cond in query.getConditions()) if (!rec.meetToCondition(cond)) meet = false;

                if (meet) return rec;
            }

            return null;
        }

        public DataRecord select(string field, object value)
        {
            return this.select(new Query(new QueryCondition(field, value)));
        }

        public List<DataRecord> selectAll(Query query)
        {
            var r = new List<DataRecord>();

            foreach (var rec in this)
            {
                var meet = true;

                foreach (var cond in query.getConditions()) if (!rec.meetToCondition(cond)) meet = false;

                if (meet) r.Add(rec);
            }

            return r;
        }

        // Note, that resulting checking is not checked for validity. Format of existing records may be incompartible with new format of table.
        public DataTable setFormat(TableFormat aTableFormat)
        {
            if (aTableFormat != null)
            {
                format.makeImmutable(this);
                this.format = aTableFormat;
            }
            return this;
        }

        public void setId(long? idLong)
        {
            this.id = idLong;
        }

        public void setInvalidationMessage(string aString)
        {
            this.invalidationMessage = aString;
        }

        public void setQuality(int _quality)
        {
            this.quality = _quality;
        }

        public DataTable setRecord(int index, DataRecord record)
        {
            this.checkOrSetFormat(record);
            this.records[index].setTable(null);
            this.records[index] = record;
            record.setTable(this);
            return this;
        }

        public void setTimestamp(DateTime timestampDateTime)
        {
            this.timestamp = timestampDateTime;
        }

        public void sort(string field, bool ascending)
        {
            this.records.Sort(
                (r1, r2) =>
                    {
                        var v1 = r1.getValue(field);
                        var v2 = r2.getValue(field);

                        if (v1 is IComparable && v2 is IComparable)
                        {
                            var res = ((IComparable)v1).CompareTo(v2);
                            return ascending ? res : -res;
                        }

                        return 0;
                    });
        }

        public void splitFormat()
        {
            foreach (var rec in this.records) rec.cloneFormatFromTable();
        }

        public void swapRecords(int index1, int index2)
        {
            var r1 = this.records[index1];
            var r2 = this.records[index2];

            this.records[index1] = r2;
            this.records[index2] = r1;
        }

        public override string ToString()
        {
            if (this.getRecordCount() == 1) return this.dataAsString();
            return "Table: " + this.getRecordCount() + " record(s), " + this.getFieldCount() + " field(s)";
        }

        public void validate(Context context, ContextManager contextManager, CallerController<CallerData> caller)
        {
            if (this.isInvalid())
            {
                throw new ValidationException(invalidationMessage);
            }

            foreach (var tv in this.getFormat().getTableValidators())
            {
                tv.validate(this);
            }

            foreach (DataRecord rec in this)
            {
                foreach (RecordValidator rv in getFormat().getRecordValidators())
                {
                    rv.validate(this, rec);
                }
                foreach (FieldFormat ff in getFormat())
                {
                    List<FieldValidator> fvs = ff.getValidators();
                    foreach (FieldValidator fv in fvs)
                    {
                        try
                        {
                            fv.validate(context, contextManager, caller, rec.getValue(ff.getName()));
                        }
                        catch (ValidationException ex)
                        {
                            throw new ValidationException("Error validating value of field '" + ff.ToString() + "': " + ex.Message, ex);
                        }
                    }
                }
            }
            foreach (FieldFormat ff in getFormat())
            {
                if (ff.getType() == FieldFormat.DATATABLE_FIELD)
                {
                    foreach (DataRecord rec in this)
                    {
                        DataTable nested = rec.getDataTable(ff.getName());
                        if (nested != null)
                        {
                            nested.validate(context, contextManager, caller);
                        }
                    }
                }
            }
        }

        public void validateRecord(DataRecord record)
        {
            foreach (var rv in this.getFormat().getRecordValidators())
            {
                rv.validate(this, record);
            }
        }

        public object Clone()
        {
            var cl = (DataTable)this.MemberwiseClone();
            cl.format = this.format.Clone() as TableFormat;
            cl.records = (List<DataRecord>)CloneUtils.deepClone(this.records);

            foreach (var rec in cl.records) rec.setTable(cl);

            return cl;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        protected void checkOrSetFormat(DataRecord record)
        {
            if (this.getFormat().getFieldCount() != 0)
            {
                var message = record.getFormat().extendMessage(this.format);
                if (message != null)
                    throw new ArgumentException(
                        "Format of new record ('" + record.getFormat() + "') differs from format of data table ('"
                        + this.getFormat() + "'): " + message);
            }
            else
            {
                this.format = record.getFormat();
            }
        }

        private void addRecordImpl(long? index, DataRecord record)
        {
            if (this.getRecordCount() >= this.format.getMaxRecords())
                throw new InvalidOperationException(
                    "Cannot add record: maximum number of records is reached: " + this.format.getMaxRecords());

            try
            {
                this.validateRecord(record);
            }
            catch (ValidationException ex)
            {
                throw new InvalidOperationException(ex.Message, ex);
            }

            if (index != null) this.records.Insert((int)index, record);
            else this.records.Add(record);

            record.setTable(this);
        }

        private void removeRecordImpl(DataRecord rec)
        {
            if (this.getRecordCount() <= this.format.getMinRecords())
                throw new InvalidOperationException(
                    "Cannot remove record: minimum number of records is reached: " + this.format.getMinRecords());

            if (this.records.Remove(rec)) rec.setTable(null);
        }


        private void decodeAdvancedElement(Element el)
        {
            throw new NotImplementedException("Advanced element encoding is not supported yet: " + el.getName());
        }



        public override int GetHashCode()
        {
            int prime = 31;
            int result = 1;
            result = prime * result + ((format == null) ? 0 : format.GetHashCode());
            result = prime * result + ((records == null) ? 0 : records.GetHashCode());
            result = prime * result + ((quality == null) ? 0 : quality.GetHashCode());
            return result;
        }

        public override bool Equals(Object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (!(obj is DataTable))
            {
                return false;
            }

            DataTable other = (DataTable)obj;

            if (!format.Equals(other.getFormat()))
            {
                return false;
            }

            if (getRecordCount() != other.getRecordCount())
            {
                return false;
            }

            if (!Util.equals(quality, other.quality))
            {
                return false;
            }

            for (int i = 0; i < getRecordCount(); i++)
            {
                if (!getRecord(i).Equals(other.getRecord(i)))
                {
                    return false;
                }
            }

            return true;
        }

        private class Enumerator : IEnumerator<DataRecord>
        {
            private readonly IEnumerator<DataRecord> recsIter;

            public Enumerator(DataTable owner)
            {
                this.recsIter = owner.records.GetEnumerator();
            }

            public DataRecord Current
            {
                get
                {
                    return this.recsIter.Current;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return this.Current;
                }
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                return this.recsIter.MoveNext();
            }

            public void Reset()
            {
                this.recsIter.Reset();
            }
        }
    }
}