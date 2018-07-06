using System;
using com.tibbo.aggregate.common.util;

namespace com.tibbo.aggregate.common.device
{
    public class EntityReference
    {
        private String context;
        private String entity;

        public EntityReference(String context, String entity)
        {
            this.context = context;
            this.entity = entity;
        }

        public String getContext()
        {
            return context;
        }

        public String getEntity()
        {
            return entity;
        }

        public void setContext(String contextString)
        {
            context = contextString;
        }

        public void setEntity(String entityString)
        {
            entity = entityString;
        }

        public Boolean equals(Object obj)
        {
            if (obj == null || !(obj is EntityReference))
            {
                return false;
            }

            var other = (EntityReference) obj;

            return Util.equals(context, other.context) && Util.equals(entity, other.entity);
        }

        public override int GetHashCode()
        {
            return context.GetHashCode() ^ entity.GetHashCode();
        }

        public String toString()
        {
            return context + ":" + entity;
        }
    }
}