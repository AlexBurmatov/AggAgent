using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.tibbo.aggregate.common.datatable
{
    using com.tibbo.aggregate.common.util;

    using JavaCompatibility;

    class DataTableReplication
    {
        public static HashSet<String> copy(DataTable source, DataTable target)
        {
            return copy(source, target, false, false, true, null);
        }

        public static HashSet<String> copy(DataTable source, DataTable target, bool copyReadOnlyFields, bool copyNonReplicatableFields, bool removeRecordsFromTarget)
        {
            return copy(source, target, copyReadOnlyFields, copyNonReplicatableFields, removeRecordsFromTarget, null);
        }

        public static HashSet<String> copy(DataTable source, DataTable target, bool copyReadOnlyFields,
                                           bool copyNonReplicatableFields, bool removeRecordsFromTarget,
                                           ICollection<String> fields)
        {
            if (target.getFormat().getKeyFields().Count == 0)
            {
                return copyWithoutKeyFields(source, target, copyReadOnlyFields, copyNonReplicatableFields,
                                            removeRecordsFromTarget, fields);
            }
            return copyWithKeyFields(source, target, copyReadOnlyFields, copyNonReplicatableFields,
                                     removeRecordsFromTarget, fields);
        }

        private static HashSet<String> copyWithKeyFields(DataTable source, DataTable target, bool copyReadOnlyFields,
                                                         bool copyNonReplicatableFields,
                                                         bool removeRecordsFromTarget,
                                                         ICollection<String> fields)
        {
            var errors = new HashSet<String>();

            var keyFields = target.getFormat().getKeyFields();

            foreach (var fieldName in keyFields)
            {
                if (!source.getFormat().hasField(fieldName))
                {
                    return copyWithoutKeyFields(source, target, copyReadOnlyFields, copyNonReplicatableFields,
                                                removeRecordsFromTarget, fields);
                }
            }

            var recordsToRemove = new List<DataRecord>();
            foreach (var targetRec in target)
            {
                var query = new Query();

                foreach (var keyField in keyFields)
                {
                    query.addCondition(new QueryCondition(keyField, targetRec.getValue(keyField)));
                }

                var sourceRec = source.select(query);

                if (!removeRecordsFromTarget || sourceRec != null) continue;
                if (target.getRecordCount() > target.getFormat().getMinRecords())
                {
                    recordsToRemove.Add(targetRec);
                }
                else
                {
                    if (source.getFormat().getMinRecords() != source.getFormat().getMaxRecords())
                    {
                        errors.Add(Cres.get().getString("dtTargetTableMinRecordsReached"));
                    }
                    break;
                }
            }
            foreach (var each in recordsToRemove)
                target.removeRecord(each);

            foreach (var sourceRec in source)
            {
                var query = new Query();

                foreach (var keyField in keyFields)
                {
                    query.addCondition(new QueryCondition(keyField, sourceRec.getValue(keyField)));
                }

                var targetRec = target.select(query);

                if (targetRec == null)
                {
                    if (target.getRecordCount() < target.getFormat().getMaxRecords())
                    {
                        var newRec = new DataRecord(target.getFormat());
                        // We are not using target.addRecord() to avoid key field validation errors
                        foreach (
                            var each in
                                copyRecord(sourceRec, newRec, copyReadOnlyFields, copyNonReplicatableFields, fields))
                        {
                            errors.Add(each);
                        }
                        target.addRecord(newRec);
                    }
                    else
                    {
                        if (source.getFormat().getMinRecords() != source.getFormat().getMaxRecords())
                        {
                            errors.Add(Cres.get().getString("dtTargetTableMaxRecordsReached"));
                        }
                    }
                }
                else
                {
                    foreach (
                        var each in
                            copyRecord(sourceRec, targetRec, copyReadOnlyFields, copyNonReplicatableFields, fields))
                    {
                        errors.Add(each);
                    }
                }
            }

            return errors;
        }

        private static HashSet<string> copyWithoutKeyFields(DataTable source, DataTable target,
                                                            bool copyReadOnlyFields,
                                                            bool copyNonReplicatableFields,
                                                            bool removeRecordsFromTarget,
                                                            ICollection<String> fields)
        {
            var errors = new HashSet<String>();

            while (removeRecordsFromTarget && target.getRecordCount() > source.getRecordCount())
            {
                if (target.getRecordCount() > target.getFormat().getMinRecords())
                {
                    target.removeRecord(target.getRecordCount() - 1);
                }
                else
                {
                    if (source.getFormat().getMinRecords() != source.getFormat().getMaxRecords())
                    {
                        errors.Add(Cres.get().getString("dtTargetTableMinRecordsReached"));
                    }
                    break;
                }
            }

            for (var i = 0; i < Math.Min(source.getRecordCount(), target.getRecordCount()); i++)
            {
                var srcRec = source.getRecord(i);
                var tgtRec = target.getRecord(i);

                foreach (var each in copyRecord(srcRec, tgtRec, copyReadOnlyFields, copyNonReplicatableFields, fields))
                    errors.Add(each);
            }

            if (source.getRecordCount() > target.getRecordCount())
            {
                for (var i = target.getRecordCount();
                     i < Math.Min(target.getFormat().getMaxRecords(), source.getRecordCount());
                     i++)
                {
                    foreach (
                        var each in
                            copyRecord(source.getRecord(i), target.addRecord(), copyReadOnlyFields,
                                       copyNonReplicatableFields, fields))
                        errors.Add(each);
                }
            }

            if (source.getRecordCount() > target.getFormat().getMaxRecords())
            {
                if (source.getFormat().getMinRecords() != source.getFormat().getMaxRecords())
                {
                    errors.Add(Cres.get().getString("dtTargetTableMaxRecordsReached"));
                }
            }

            return errors;
        }

        private static HashSet<String> copyRecord(DataRecord source, DataRecord target, bool copyReadOnlyFields,
                                                  bool copyNonReplicatableFields, ICollection<String> fields)
        {
            var errors = new HashSet<String>();

            foreach (var tgtFf in target.getFormat())
            {
                var fieldName = tgtFf.getName();

                var srcFf = source.getFormat().getFieldFormat(fieldName);

                if (fields != null && !fields.Contains(tgtFf.getName()))
                {
                    continue;
                }

                if (srcFf == null)
                {
                    continue;
                }

                if (tgtFf.isReadonly() && !copyReadOnlyFields)
                {
                    continue;
                }

                if (!copyNonReplicatableFields)
                {
                    if (tgtFf.isNotReplicated() || srcFf.isNotReplicated())
                    {
                        continue;
                    }
                }

                try
                {
                    if (srcFf.getFieldWrappedClass().Equals(tgtFf.getFieldWrappedClass()))
                    {
                        target.setValue(fieldName, CloneUtils.genericClone(source.getValue(fieldName)));
                    }
                    else
                    {
                        target.setValue(fieldName, tgtFf.valueFromString(srcFf.valueToString(source.getValue(fieldName))));
                    }
                }
                catch (Exception ex2)
                {
                    var msg = String.Format(Cres.get().getString("dtErrCopyingField"), fieldName);

                    Logger.getLogger(Log.DATATABLE).debug(msg, ex2);

                    errors.Add(msg + ": " + ex2.Message);
                    continue;
                }
            }

            return errors;
        }

    }
}
