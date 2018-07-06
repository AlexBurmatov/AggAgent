using System;
using com.tibbo.aggregate.common.data;
using com.tibbo.aggregate.common.datatable;
using com.tibbo.aggregate.common.security;
using JavaCompatibility;

namespace com.tibbo.aggregate.common.context
{
    using com.tibbo.aggregate.common.@event;

    using OX.Copyable;

    public class EventDefinition : AbstractEntityDefinition, IComparable<EventDefinition>, ICloneable
    {
        private TableFormat format;
        private bool hidden;
        private Permissions permissions;
        private long expirationPeriod = Event.DEFAULT_EVENT_EXPIRATION_PERIOD; // Milliseconds, 0 for non-persistent
        private int level;
        private Permissions firePermissions;

        private bool synchronous;

        private PersistenceOptions persistenceOptions = new PersistenceOptions();
        private int memoryStorageSize;

        public EventDefinition(string name, TableFormat format)
        {
            this.init(name, format, null);
        }

        public EventDefinition(string name, TableFormat format, string description)
        {
            this.init(name, format, description);
        }

        public EventDefinition(string name, TableFormat format, string description, string group)
        {
            this.init(name, format, description);
            this.setGroup(group);
        }

        private void init(string nameString, TableFormat formatString, string descriptionString)
        {
            this.setName(nameString);

            this.setFormat(formatString);

            this.setDescription(descriptionString);
        }

        public void setFormat(TableFormat formatString)
        {
            if (this.format != null)
            {
                this.format.makeImmutable(null);
            }

            this.format = formatString;
        }

        public void setHidden(bool hiddenBoolean)
        {
            this.hidden = hiddenBoolean;
        }

        public void setPermissions(Permissions perms)
        {
            this.permissions = perms;
        }

        public void setExpirationPeriod(long expirationPeriodLong)
        {
            this.expirationPeriod = expirationPeriodLong;
        }

        public void setLevel(int levelString)
        {
            this.level = levelString;
        }

        public void setSynchronous(bool synchronousBool)
        {
            this.synchronous = synchronousBool;
        }

        public TableFormat getFormat()
        {
            return this.format;
        }

        public bool isHidden()
        {
            return this.hidden;
        }

        public Permissions getPermissions()
        {
            return this.permissions;
        }

        public long? getExpirationPeriod()
        {
            return this.expirationPeriod;
        }

        public int getLevel()
        {
            return this.level;
        }

        public Permissions getFirePermissions()
        {
            return this.firePermissions;
        }

        public void setFirePermissions(Permissions aPermissions)
        {
            this.firePermissions = aPermissions;
        }

        public bool isSynchronous()
        {
            return this.synchronous;
        }

        public PersistenceOptions getPersistenceOptions()
        {
            return this.persistenceOptions;
        }

        public int getMemoryStorageSize()
        {
            return this.memoryStorageSize;
        }

        public void setMemoryStorageSize(int memoryStorageSizeInt)
        {
            this.memoryStorageSize = memoryStorageSizeInt;
        }

        public object Clone()
        {
            try
            {
                EventDefinition clone = (EventDefinition)this.MemberwiseClone();

                clone.persistenceOptions = (PersistenceOptions)this.persistenceOptions.Clone();

                return clone;
            }
            catch (CloneNotSupportedException ex)
            {
                throw new IllegalStateException(ex.Message, ex);
            }
        }

        public int CompareTo(EventDefinition d)
        {
            if (this.getIndex() != null || d.getIndex() != null)
            {
                var my = this.getIndex() != null ? (int)this.getIndex() : 0;
                var other = d.getIndex() != null ? (int)d.getIndex() : 0;
                return other.CompareTo(my);
            }

            return 0;
        }

        public override int GetHashCode()
        {
            int prime = 31;
            int result = 1;
            result = (prime * result) + ((this.getDescription() == null) ? 0 : this.getDescription().GetHashCode());
            result = (prime * result) + (int)(this.expirationPeriod ^ ((long)((ulong?)this.expirationPeriod >> 32)));
            result = (prime * result) + ((this.format == null) ? 0 : this.format.GetHashCode());
            result = (prime * result) + ((this.getGroup() == null) ? 0 : this.getGroup().GetHashCode());
            result = (prime * result) + ((this.getHelp() == null) ? 0 : this.getHelp().GetHashCode());
            result = (prime * result) + (this.hidden ? 1231 : 1237);
            result = (prime * result) + ((this.getIconId() == null) ? 0 : this.getIconId().GetHashCode());
            result = (prime * result) + ((this.getIndex() == null) ? 0 : this.getIndex().GetHashCode());
            result = (prime * result) + this.level;
            result = (prime * result) + ((this.getName() == null) ? 0 : this.getName().GetHashCode());
            result = (prime * result) + ((this.permissions == null) ? 0 : this.permissions.GetHashCode());
            result = (prime * result) + ((this.firePermissions == null) ? 0 : this.firePermissions.GetHashCode());
            return result;
        }

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
            if (this.GetType() != obj.GetType())
            {
                return false;
            }
            EventDefinition other = (EventDefinition)obj;
            if (this.getDescription() == null)
            {
                if (other.getDescription() != null)
                {
                    return false;
                }
            }
            else if (!this.getDescription().Equals(other.getDescription()))
            {
                return false;
            }
            if (this.expirationPeriod != other.expirationPeriod)
            {
                return false;
            }
            if (this.format == null)
            {
                if (other.format != null)
                {
                    return false;
                }
            }
            else if (!this.format.Equals(other.format))
            {
                return false;
            }
            if (this.getGroup() == null)
            {
                if (other.getGroup() != null)
                {
                    return false;
                }
            }
            else if (!this.getGroup().Equals(other.getGroup()))
            {
                return false;
            }
            if (this.getHelp() == null)
            {
                if (other.getHelp() != null)
                {
                    return false;
                }
            }
            else if (!this.getHelp().Equals(other.getHelp()))
            {
                return false;
            }
            if (this.hidden != other.hidden)
            {
                return false;
            }
            if (this.getIconId() == null)
            {
                if (other.getIconId() != null)
                {
                    return false;
                }
            }
            else if (!this.getIconId().Equals(other.getIconId()))
            {
                return false;
            }
            if (this.getIndex() == null)
            {
                if (other.getIndex() != null)
                {
                    return false;
                }
            }
            else if (!this.getIndex().Equals(other.getIndex()))
            {
                return false;
            }
            if (this.level != other.level)
            {
                return false;
            }
            if (this.getName() == null)
            {
                if (other.getName() != null)
                {
                    return false;
                }
            }
            else if (!this.getName().Equals(other.getName()))
            {
                return false;
            }
            if (this.permissions == null)
            {
                if (other.permissions != null)
                {
                    return false;
                }
            }
            else if (!this.permissions.Equals(other.permissions))
            {
                return false;
            }
            if (this.firePermissions == null)
            {
                if (other.firePermissions != null)
                    return false;
            }
            else if (!this.firePermissions.Equals(other.firePermissions))
            {
                return false;
            }
            return true;
        }

    }
}