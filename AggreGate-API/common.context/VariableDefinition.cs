using System;
using System.Threading;
using com.tibbo.aggregate.common.datatable;
using com.tibbo.aggregate.common.security;
using JavaCompatibility;

namespace com.tibbo.aggregate.common.context
{
    public class VariableDefinition : ICloneable, IComparable<VariableDefinition>
    {
        public enum UpdateEventsPolicy
        {
            NONE,
            TRANSIENT,
            PERSISTENT
        } ;

        private String name;
        private TableFormat format;
        private Boolean readable;
        private Boolean writable;
        private String description;
        private Boolean hidden;
        private Permissions readPermissions;
        private Permissions writePermissions;
        private String help;
        private String group;
        private String iconId;
        private String helpId;

        private VariableGetter getter;
        private VariableSetter setter;

        private bool allowUpdateEvents;

        private Int32? index;

        private UpdateEventsPolicy updateEvents = UpdateEventsPolicy.NONE;
        private Int64 changeEventsExpirationPeriod; // Milliseconds
        private bool localCachingEnabled = true;

        private ReaderWriterLockSlim readWriteLock = new ReaderWriterLockSlim();

        private Type valueClass;

        private Boolean persistent = true;
        private DataTable defaultValue;

        public VariableDefinition(String name, TableFormat format, Boolean readable, Boolean writable,
                                  String description)
        {
            init(name, format, readable, writable, description);
        }

        public VariableDefinition(String name, TableFormat format, bool readable, bool writable, String description, String group)
        {
            init(name, format, readable, writable, description);
            setGroup(group);
        }

        private void init(String nameString, TableFormat formatString, Boolean readableBoolean, Boolean writableBoolean,
                          String descriptionString)
        {
            name = nameString;
            format = formatString;
            readable = readableBoolean;
            writable = writableBoolean;
            description = descriptionString ?? nameString;
        }

        public void setName(String nameString)
        {
            name = nameString;
        }

        public void setFormat(TableFormat formatString)
        {
            format = formatString;
        }

        public void setReadable(Boolean readableBoolean)
        {
            readable = readableBoolean;
        }

        public void setWritable(Boolean writableBoolean)
        {
            writable = writableBoolean;
        }

        public void setDescription(String descriptionString)
        {
            description = descriptionString;
        }

        public void setHidden(Boolean hiddenBoolean)
        {
            hidden = hiddenBoolean;
        }

        public void setHelp(String helpString)
        {
            help = helpString;
        }

        public void setGroup(String groupString)
        {
            //base.setGroup(group);
            this.group = groupString;

            if (this.group != null)
            {
                this.allowUpdateEvents = true;
            }
        }

        public void setReadPermissions(Permissions readPermissionsParam)
        {
            readPermissions = readPermissionsParam;
        }

        public void setWritePermissions(Permissions writePermissionsParam)
        {
            writePermissions = writePermissionsParam;
        }

        public void setSetter(VariableSetter aVariableSetter)
        {
            setter = aVariableSetter;
        }

        public void setGetter(VariableGetter aVariableGetter)
        {
            getter = aVariableGetter;
        }

        public void setReadWriteLock(ReaderWriterLockSlim aReadWriteLock)
        {
            readWriteLock = aReadWriteLock;
        }

        public void setIconId(String iconIdString)
        {
            iconId = iconIdString;
        }

        public void setHelpId(String helpIdString)
        {
            helpId = helpIdString;
        }

        public void setUpdateEvents(UpdateEventsPolicy anUpdateEventsPolicy)
        {
            updateEvents = anUpdateEventsPolicy;
        }

        public String getName()
        {
            return name;
        }

        public TableFormat getFormat()
        {
            return format;
        }

        public Boolean isReadable()
        {
            return readable;
        }

        public Boolean isWritable()
        {
            return writable;
        }

        public String getDescription()
        {
            return description;
        }

        public Boolean isHidden()
        {
            return hidden;
        }

        public String getHelp()
        {
            return help;
        }

        public String getGroup()
        {
            return group;
        }

        public ReaderWriterLockSlim getReadWriteLock()
        {
            return readWriteLock;
        }

        public Permissions getReadPermissions()
        {
            return readPermissions;
        }

        public Permissions getWritePermissions()
        {
            return writePermissions;
        }

        public VariableSetter getSetter()
        {
            return setter;
        }

        public VariableGetter getGetter()
        {
            return getter;
        }

        public String getIconId()
        {
            return iconId;
        }

        public String getHelpId()
        {
            return helpId;
        }

        public UpdateEventsPolicy getUpdateEvents()
        {
            return updateEvents;
        }

        public Int32? getIndex()
        {
            return index;
        }

        public void setIndex(Int32 indexInteger)
        {
            index = indexInteger;
        }

        public Type getValueClass()
        {
            return valueClass;
        }

        public void setValueClass(Type valueType)
        {
            valueClass = valueType;
        }

        public Int64? getChangeEventsExpirationPeriod()
        {
            return changeEventsExpirationPeriod;
        }

        public void setChangeEventsExpirationPeriod(Int64 changeEventsExpirationPeriodLong)
        {
            changeEventsExpirationPeriod = changeEventsExpirationPeriodLong;
        }

        public bool isLocalCachingEnabled()
        {
            return localCachingEnabled;
        }

        public DataTable getDefaultValue()
        {
            return defaultValue;
        }

        public void setDefaultValue(DataTable aDataTable)
        {
            defaultValue = aDataTable;
        }

        public Boolean isPersistent()
        {
            return persistent;
        }

        public void setPersistent(Boolean persistentBoolean)
        {
            persistent = persistentBoolean;
        }

        public bool isAllowUpdateEvents()
        {
            return this.allowUpdateEvents;
        }

        public override String ToString()
        {
            return description ?? name;
        }

        public String toDetailedString()
        {
            return description != null ? description + " (" + name + ")" : name;
        }

        public object Clone()
        {
            try
            {
                return MemberwiseClone();
            }
            catch (CloneNotSupportedException ex)
            {
                throw new InvalidOperationException(ex.Message, ex);
            }
        }

        public override int GetHashCode()
        {
            const int prime = 31;
            var result = 1;
            result = prime*result + ((description == null) ? 0 : description.GetHashCode());
            result = prime*result + ((format == null) ? 0 : format.GetHashCode());
            result = prime*result + ((group == null) ? 0 : group.GetHashCode());
            result = prime*result + ((help == null) ? 0 : help.GetHashCode());
            result = prime*result + (hidden ? 1231 : 1237);
            result = prime*result + ((iconId == null) ? 0 : iconId.GetHashCode());
            result = prime*result + ((helpId == null) ? 0 : helpId.GetHashCode());
            result = prime*result + ((index == null) ? 0 : index.GetHashCode());
            result = prime*result + ((name == null) ? 0 : name.GetHashCode());
            result = prime*result + (readable ? 1231 : 1237);
            result = prime*result + (writable ? 1231 : 1237);
            return result;
        }

        public Boolean equals(Object obj)
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
            var other = (VariableDefinition) obj;
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
            if (format == null)
            {
                if (other.format != null)
                {
                    return false;
                }
            }
            else if (!format.Equals(other.format))
            {
                return false;
            }
            if (group == null)
            {
                if (other.group != null)
                {
                    return false;
                }
            }
            else if (!group.Equals(other.group))
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
            if (hidden != other.hidden)
            {
                return false;
            }
            if (iconId == null)
            {
                if (other.iconId != null)
                {
                    return false;
                }
            }
            else if (!iconId.Equals(other.iconId))
            {
                return false;
            }
            if (helpId == null)
            {
                if (other.helpId != null)
                {
                    return false;
                }
            }
            else if (!helpId.Equals(other.helpId))
            {
                return false;
            }
            if (index == null)
            {
                if (other.index != null)
                {
                    return false;
                }
            }
            else if (!index.Equals(other.index))
            {
                return false;
            }
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
            if (readable != other.readable)
            {
                return false;
            }
            return writable == other.writable;
        }

        public int CompareTo(VariableDefinition d)
        {
            if (getIndex() != null || d.getIndex() != null)
            {
                return (d.getIndex() ?? 0).CompareTo(getIndex() ?? 0);
            }

            return 0;
        }

    }
}