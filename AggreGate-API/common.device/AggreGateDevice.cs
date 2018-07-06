using System;

namespace com.tibbo.aggregate.common.device
{
    public class AggreGateDevice
    {
        protected static String id = "generic";
        protected static String type = "Generic AggreGate Device";

        private String name;
        private String description;
        private Boolean disabled;

        public AggreGateDevice()
        {
            name = id;
            description = type;
        }

        public AggreGateDevice(String name)
        {
            this.name = name;
        }

        public String getName()
        {
            return name;
        }

        public void setName(String nameString)
        {
            name = nameString;
        }

        public void setDescription(String descriptionString)
        {
            description = descriptionString;
        }

        public String getType()
        {
            return type;
        }

        public String getId()
        {
            return id;
        }

        public String getDescription()
        {
            return description;
        }

        public Boolean isDisabled()
        {
            return disabled;
        }

        public void setDisabled(Boolean disabledBoolean)
        {
            disabled = disabledBoolean;
        }

        public virtual String getInfo()
        {
            return type;
        }

        public override String ToString()
        {
            var res = (!string.IsNullOrEmpty(getDescription())) ? getDescription() : getType();
            return res + " (" + getInfo() + ")";
        }
    }
}