using System;

namespace com.tibbo.aggregate.common.security
{
    public class PermissionType
    {
        private int pattern;
        private String name;
        private String description;

        public PermissionType(int pattern, String name, String description)
        {
            this.pattern = pattern;
            this.name = name;
            this.description = description;
        }

        public int getPattern()
        {
            return pattern;
        }

        public void setPattern(Int16 patternInteger)
        {
            pattern = patternInteger;
        }

        public String getName()
        {
            return name;
        }

        public void setName(String nameString)
        {
            name = nameString;
        }

        public String getDescription()
        {
            return description;
        }

        public void setDescription(String descriptionString)
        {
            description = descriptionString;
        }
    }
}