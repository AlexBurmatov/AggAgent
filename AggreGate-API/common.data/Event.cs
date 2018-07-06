using System;
using System.Collections.Generic;
using Collections;
using com.tibbo.aggregate.common.context;
using com.tibbo.aggregate.common.datatable;
using com.tibbo.aggregate.common.@event;
using com.tibbo.aggregate.common.util;
using JavaCompatibility;

namespace com.tibbo.aggregate.common.data
{
    using com.tibbo.aggregate.common.security;

    public class Event : ICloneable
    {
        public const long DEFAULT_EVENT_EXPIRATION_PERIOD = 100 * TimeHelper.DAY_IN_MS;

        private long? id;

        private readonly DateTime instantiationtime = new DateTime();

        private DateTime creationtime;

        private DateTime expirationtime;

        private string context;

        private string name;

        private AgList<Acknowledgement> acknowledgements = new AgList<Acknowledgement>();

        private DataTable data;

        private int? listener;

        private int? level;

        private Permissions permissions;

        private int count = 1;

        private object originator;

        private string deduplicationId;

        public Event()
        {
            this.setCreationtime(DateTime.Now);
        }

        public Event(string context, EventDefinition def, int? level, DataTable data, long? id, DateTime? creationDateTime, Permissions aPermissions) : this()
        {
            this.init(context, def.getName(), level, data, id);
            this.name = def.getName();
            this.permissions = aPermissions;

            if (creationDateTime != null)
            {
                this.creationtime = (DateTime)creationDateTime;
            }

            if (def.getExpirationPeriod() > 0)
            {
                this.setExpirationtime(DateTime.Now.AddMilliseconds((double)def.getExpirationPeriod()));
            }
        }

        public Event(string context, string name, int level, DataTable data, long id)
        {
            this.init(context, name, level, data, id);
        }

        private void init(string aContext, string nameString, int? levelInteger, DataTable aDataTable, long? idLong)
        {
            this.context = aContext;
            this.name = nameString;
            this.level = levelInteger;
            this.data = aDataTable;
            this.id = idLong;
        }

        public long? getId()
        {
            return this.id;
        }

        public DateTime getInstantiationtime()
        {
            return this.instantiationtime;
        }


        public DateTime getCreationtime()
        {
            return this.creationtime;
        }

        public string getContext()
        {
            return this.context;
        }

        public string getName()
        {
            return this.name;
        }

        public DateTime getExpirationtime()
        {
            return this.expirationtime;
        }

 
        public void addAcknowledgement(Acknowledgement ack)
        {
            this.acknowledgements.Add(ack);
        }

        public IList<Acknowledgement> getAcknowledgements()
        {
            return this.acknowledgements;
        }

 
        
        public void setId(long idInteger)
        {
            this.id = idInteger;
        }

        public void setCreationtime(DateTime creationDateTime)
        {
            this.creationtime = creationDateTime;
        }

        public void setName(string nameString)
        {
            this.name = nameString;
        }

        public void setContext(string contextString)
        {
            this.context = contextString;
        }

        public void setExpirationtime(DateTime expirationDateTime)
        {
            this.expirationtime = expirationDateTime;
        }

        public void setData(DataTable aDataTable)
        {
            this.data = aDataTable;
        }

        public void setListener(int? listenerInteger)
        {
            this.listener = listenerInteger;
        }

        public void setLevel(int levelInteger)
        {
            this.level = levelInteger;
        }

        public void setOriginator(object originatorObject)
        {
            this.originator = originatorObject;
        }

        public DataTable getData()
        {
            return this.data;
        }

        public int? getListener()
        {
            return this.listener;
        }

        public int? getLevel()
        {
            return this.level;
        }

        public object getOriginator()
        {
            return this.originator;
        }

        public int getCount()
        {
            return this.count;
        }

        public void setCount(int countInt)
        {
            this.count = countInt;
        }

        public string getDeduplicationId()
        {
            return this.deduplicationId;
        }

        public void setDeduplicationId(String deduplicationIdString)
        {
            this.deduplicationId = deduplicationIdString;
        }

        public object Clone()
        {
            try
            {
                var clone = (Event)this.MemberwiseClone();
                clone.acknowledgements = (AgList<Acknowledgement>)CloneUtils.deepClone(this.acknowledgements);
                //clone.enrichments = (AgList)CloneUtils.deepClone(enrichments);
                return clone;
            }
            catch (CloneNotSupportedException ex)
            {
                throw new InvalidOperationException(string.Empty, ex);
            }
        }

        public override string ToString()
        {
            return "Event '" + this.name + "' in context '" + this.context + "': " + (this.data != null ? this.data.dataAsString() : "no data")
                   + (this.listener != null ? ", for listener '" + this.listener + "'" : string.Empty);
        }
    }
}