using System;
using System.Collections.Generic;

namespace com.tibbo.aggregate.common.datatable
{
    public class ClassicEncodingSettings : EncodingSettings
    {
        private bool useVisibleSeparators;

        private FormatCache formatCache;
        private KnownFormatCollector knownFormatCollector;

        private Boolean encodeDefaultValues = true;
        private Boolean encodeFieldNames;

        public ClassicEncodingSettings(Boolean useVisibleSeparators)
            : base(true, null)
        {
            this.useVisibleSeparators = useVisibleSeparators;
        }

        public ClassicEncodingSettings(Boolean useVisibleSeparators, Boolean encodeFieldNames)
            : this(useVisibleSeparators)
        {
            this.encodeFieldNames = encodeFieldNames;
        }

        public bool isEncodeDefaultValues()
        {
            return encodeDefaultValues;
        }

        public void setEncodeDefaultValues(Boolean encodeDefaultValuesBoolean)
        {
            encodeDefaultValues = encodeDefaultValuesBoolean;
        }


        public ClassicEncodingSettings(Boolean useVisibleSeparatorsBoolean, TableFormat format) : base(true, format)
        {
            useVisibleSeparators = useVisibleSeparatorsBoolean;
        }


        public bool isUseVisibleSeparators()
        {
            return useVisibleSeparators;
        }

        public void setUseVisibleSeparators(Boolean useVisibleSeparatorsBoolean)
        {
            useVisibleSeparators = useVisibleSeparatorsBoolean;
        }


        public FormatCache getFormatCache()
        {
            return formatCache;
        }

        public void setFormatCache(FormatCache aFormatCache)
        {
            formatCache = aFormatCache;
        }

        public Boolean isEncodeFieldNames()
        {
            return encodeFieldNames;
        }

        public void setEncodeFieldNames(Boolean encodeFieldNamesBoolean)
        {
            encodeFieldNames = encodeFieldNamesBoolean;
        }

        public void setKnownFormatCollector(KnownFormatCollector aKnownFromatCollector)
        {
            this.knownFormatCollector = aKnownFromatCollector;
        }

        public KnownFormatCollector getKnownFormatCollector()
        {
            return this.knownFormatCollector;
        }
    }
}