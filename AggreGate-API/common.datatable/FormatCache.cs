using System;
using System.Collections.Generic;
using System.Threading;
using com.tibbo.aggregate.common.context;
using com.tibbo.aggregate.common.device;
using com.tibbo.aggregate.common.server;
using com.tibbo.aggregate.common.util;

namespace com.tibbo.aggregate.common.datatable
{
    public class FormatCache
    {
        private const int RETRIES = 30;
        private const int TIMEOUT = 1000;

        private int currentId = 0;


        private readonly Dictionary<Int32, TableFormat> cache = new FormatMap<Int32, TableFormat>();

        private readonly Dictionary<TableFormat, Int32> reverse = new FormatMap<TableFormat, Int32>();

        private readonly ReaderWriterLockSlim cacheLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        private AggreGateDeviceController controller;

        private bool useExternalIds;

        public FormatCache(AggreGateDeviceController controller)
        {
            this.controller = controller;
        }

        public FormatCache()
        {
            this.useExternalIds = true;
        }

        public void put(int id, TableFormat format)
        {
            cacheLock.EnterWriteLock();
            try
            {
                if (format == null)
                {
                    throw new InvalidOperationException("Format is NULL");
                }

                if (addImpl(format, id) == null && Log.PROTOCOL_CACHING.isDebugEnabled())
                {
                    Log.PROTOCOL_CACHING.debug("Cached format as #" + id + ": " + format);
                }
            }
            finally
            {
                cacheLock.ExitWriteLock();
            }
        }


        public int addIfNotExists(TableFormat format)
        {
            var formatId = this.getId(format);

            if (formatId == null)
            {
                try
                {
                    cacheLock.EnterWriteLock();

                    formatId = this.getId(format);

                    if (formatId == null)
                    {
                        formatId = add(format);
                    }
                }
                finally
                {
                    cacheLock.ExitWriteLock();
                }
            }

            return (int) formatId;
        }


        public int add(TableFormat format)
        {
            cacheLock.EnterWriteLock();
            try
            {
                if (format == null)
                {
                    throw new ArgumentException("Format is NULL");
                }

                int id = this.currentId++;

                this.addImpl(format, id);

                if (!format.isImmutable())
                {
                    Log.PROTOCOL_CACHING.warn("Cached mutable format as #" + id + ": " + format, new Exception());
                }


                if (Log.PROTOCOL_CACHING.isDebugEnabled())
                {
                    Log.PROTOCOL_CACHING.debug("Cached format as #" + id + ": " + format);
                }

                return id;
            }
            finally
            {
                cacheLock.ExitWriteLock();
            }
        }

        private TableFormat addImpl(TableFormat format, int id)
        {
            // We consider all formats as immutable now. This can cause some bugs if we cache mutable formats
            if (this.useExternalIds && format.isImmutable())
            {
                format.setId(id);
            }

            this.reverse[format] = id;

            this.cache[id] = format;

            return format;
        }


        public Int32? getId(TableFormat format)
        {
            if (useExternalIds && format.getId() != null)
            {
                return format.getId();
            }

            cacheLock.EnterReadLock();
            try
            {
                if (!this.reverse.ContainsKey(format))
                {
                    return null;
                }

                return reverse[format];
            }
            finally
            {
                cacheLock.ExitReadLock();
            }
        }

        public TableFormat get(int id)
        {
            TableFormat result;
            var retry = 0;

            do
            {
                cacheLock.EnterReadLock();
                try
                {
                    result = this.cache.ContainsKey(id) ? cache[id] : null;
                }
                finally
                {
                    cacheLock.ExitReadLock();
                }

                if (result == null)
                {
                    if (controller != null && controller.getContextManager() != null)
                    {
                        try
                        {
                            if (Log.PROTOCOL_CACHING.isDebugEnabled())
                            {
                                Log.PROTOCOL_CACHING.debug("Requesting remote format #" + id);
                            }

                            var formatData = controller.getContextManager().get(ContextUtils.CTX_UTILS).callFunction(
                                    UtilsContextConstants.F_GET_FORMAT, id)
                                .rec()
                                .getString(UtilsContextConstants.FOF_GET_FORMAT_DATA);
                            result = new TableFormat(formatData, new ClassicEncodingSettings(false));

                            if (Log.PROTOCOL_CACHING.isDebugEnabled())
                            {
                                Log.PROTOCOL_CACHING.debug(
                                    "Received explicitely requested remote format #" + id + ": " + result);
                            }

                            cacheLock.EnterWriteLock();
                            try
                            {
                                cache.Add(id, result);
                            }
                            finally
                            {
                                cacheLock.ExitWriteLock();
                            }
                        }
                        catch (ContextException ex)
                        {
                            throw new InvalidOperationException(
                                "Error obtaining format #" + id + ": " + ex.Message, ex);
                        }
                    }
                    else
                    {
                        Log.PROTOCOL_CACHING.warn("Waiting for format #" + id);
                        try
                        {
                            Monitor.Wait(this, TIMEOUT);
                        }
                        catch (ThreadInterruptedException ex)
                        {
                            ex.GetType();
                            Log.PROTOCOL_CACHING.warn(
                                "Interrupted while waiting for format with ID: " + id);
                            return null;
                        }
                    }
                }

                retry++;
            } while (result == null && retry < RETRIES);

            if (result == null)
            {
                Log.PROTOCOL_CACHING.warn("Timeout while getting format #" + id + ", cache size: " +
                                          cache.Count);
                dump();
            }

            return result;
        }

        private void dump()
        {
            cacheLock.EnterReadLock();
            try
            {
                foreach (var entry in cache)
                {
                    Log.PROTOCOL_CACHING.info("Format cache entry with ID #" + entry.Key + ": " +
                                                                entry.Value);
                }
            }
            finally
            {
                cacheLock.ExitReadLock();
            }
        }

        private class FormatMap<K, V> : AgDictionary<K, V>
        {
            public FormatMap() : base(100)
            {
            }
        }

        public TableFormat getCachedVersion(TableFormat format)
        {
            if (format == null)
            {
                return null;
            }

            cacheLock.EnterReadLock();
            try
            {
                int? id = getId(format);

                if (id == null) return format;

                TableFormat result = this.cache.ContainsKey((int) id) ? cache[(int) id] : null;

                return result;
            }
            finally
            {
                cacheLock.ExitReadLock();
            }
        }
    }
}