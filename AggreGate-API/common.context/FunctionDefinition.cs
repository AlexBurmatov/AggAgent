using System;
using System.Threading;
using com.tibbo.aggregate.common.datatable;
using com.tibbo.aggregate.common.security;
using JavaCompatibility;

namespace com.tibbo.aggregate.common.context
{
    public class FunctionDefinition : ICloneable, IComparable<FunctionDefinition>
    {
        private String name;
        private TableFormat inputFormat;
        private TableFormat outputFormat;
        private String description;
        private Boolean hidden;
        private Permissions permissions;


        //private FunctionImplementation implementation;

        private bool concurrent = false;

        private String help;
        private String group;
        private String iconId;

        private FunctionImplementation implementation;

        private Int32? index;

        private readonly ReaderWriterLockSlim executionLock = new ReaderWriterLockSlim();

        public FunctionDefinition(String name, TableFormat inputFormat, TableFormat outputFormat)
        {
            init(name, inputFormat, outputFormat, null, null);
        }

        public FunctionDefinition(String name, TableFormat inputFormat, TableFormat outputFormat, String description)
        {
            init(name, inputFormat, outputFormat, description, null);
        }

        public FunctionDefinition(String name, TableFormat inputFormat, TableFormat outputFormat, String description,
                                  String group)
        {
            init(name, inputFormat, outputFormat, description, group);
        }

        private void init(String nameString, TableFormat inputTableFormat, TableFormat outputTableFormat,
                          String descriptionString,
                          String groupString)
        {
            name = nameString;
            inputFormat = inputTableFormat;
            outputFormat = outputTableFormat;
            description = descriptionString;
            group = groupString;
        }

        public String getDescription()
        {
            return description;
        }

        public TableFormat getInputFormat()
        {
            return inputFormat;
        }

        public String getName()
        {
            return name;
        }

        public TableFormat getOutputFormat()
        {
            return outputFormat;
        }

        public Boolean isHidden()
        {
            return hidden;
        }

        public Permissions getPermissions()
        {
            return permissions;
        }

        public String getHelp()
        {
            return help;
        }

        public ReaderWriterLockSlim getExecutionLock()
        {
            return executionLock;
        }

        public String getGroup()
        {
            return group;
        }

        public FunctionImplementation getImplementation()
        {
            return implementation;
        }

        public void setDescription(String descriptionString)
        {
            description = descriptionString;
        }

        public void setInputFormat(TableFormat inputTableFormat)
        {
            inputFormat = inputTableFormat;
        }

        public void setName(String nameString)
        {
            name = nameString;
        }

        public void setOutputFormat(TableFormat outputTableFormat)
        {
            outputFormat = outputTableFormat;
        }

        public void setHidden(Boolean hiddenBoolean)
        {
            hidden = hiddenBoolean;
        }

        public void setPermissions(Permissions perms)
        {
            permissions = perms;
        }

        public bool isConcurrent()
        {
            return this.concurrent;
        }

        public void setConcurrent(bool concurrent)
        {
            this.concurrent = concurrent;
        }

        public void setHelp(String helpString)
        {
            help = helpString;
        }

        public void setGroup(String groupString)
        {
            group = groupString;
        }

        public Int32? getIndex()
        {
            return index;
        }

        public void setIndex(Int32 indexInteger)
        {
            index = indexInteger;
        }

        public void setImplementation(FunctionImplementation aFunctionImplementation)
        {
            implementation = aFunctionImplementation;
        }

        public override String ToString()
        {
            return description ?? name;
        }

        public String toDetailedString()
        {
            return description != null ? description + " (" + name + ")" : name;
        }

        public Object Clone()
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

        public int CompareTo(FunctionDefinition d)
        {
            return (d.getIndex() ?? 0).CompareTo((int?) (getIndex() ?? 0));
        }

        public override int GetHashCode()
        {
            const int prime = 31;
            var result = 1;
            result = prime*result + ((description == null) ? 0 : description.GetHashCode());
            result = prime*result + ((group == null) ? 0 : group.GetHashCode());
            result = prime*result + ((help == null) ? 0 : help.GetHashCode());
            result = prime*result + (hidden ? 1231 : 1237);
            result = prime*result + ((implementation == null) ? 0 : implementation.GetHashCode());
            result = prime*result + ((index == null) ? 0 : index.GetHashCode());
            result = prime*result + ((inputFormat == null) ? 0 : inputFormat.GetHashCode());
            result = prime*result + ((name == null) ? 0 : name.GetHashCode());
            result = prime*result + ((outputFormat == null) ? 0 : outputFormat.GetHashCode());
            return result;
        }


        public override Boolean Equals(Object obj)
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
            var other = (FunctionDefinition) obj;
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
            if (implementation == null)
            {
                if (other.implementation != null)
                {
                    return false;
                }
            }
            else if (!implementation.Equals(other.implementation))
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
            if (inputFormat == null)
            {
                if (other.inputFormat != null)
                {
                    return false;
                }
            }
            else if (!inputFormat.Equals(other.inputFormat))
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
            if (outputFormat == null)
            {
                if (other.outputFormat != null)
                {
                    return false;
                }
            }
            else if (!outputFormat.Equals(other.outputFormat))
            {
                return false;
            }
            return true;
        }

        public void setIconId(String aString)
        {
            iconId = aString;
        }
    
        public String getIconId()
        {
            return iconId;
        }
    }
}