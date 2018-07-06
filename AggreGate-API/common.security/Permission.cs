using System;
using System.Runtime.CompilerServices;
using System.Text;
using com.tibbo.aggregate.common.util;

namespace com.tibbo.aggregate.common.security
{
    public class Permission
    {
        private const char PERMISSION_FIELDS_SEPARATOR = ':';

        private String entity;
        private String type;

        public Permission(String data, PermissionChecker checker)
        {
            var spd = StringUtils.split(data, PERMISSION_FIELDS_SEPARATOR);

            switch (spd.Count)
            {
                case 1:
                    type = spd[0];
                    break;

                case 2:
                    entity = spd[0];
                    type = spd[1];
                    break;

                default:
                    throw new ArgumentException("Invalid permission: '" + data + "'");
            }

            if (checker == null)
            {
            }
            else
            {
                if (!checker.isValid(type))
                {
                    throw new ArgumentException("Invalid permission type: '" + type + "'");
                }
            }
        }

        public Permission(String entity, String type)
        {
            this.entity = entity;
            this.type = type;
        }

        public String getEntity()
        {
            return entity;
        }

        public String getType()
        {
            return type;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public String encode()
        {
            var enc = new StringBuilder();

            if (entity != null)
            {
                enc.Append(entity);
                enc.Append(PERMISSION_FIELDS_SEPARATOR);
            }

            enc.Append(type);

            return enc.ToString();
        }

        public override String ToString()
        {
            return encode();
        }

        public void setEntity(String entityString)
        {
            entity = entityString;
        }

        public void setType(String typeString)
        {
            type = typeString;
        }
    }
}