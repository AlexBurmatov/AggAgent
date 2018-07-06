using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.tibbo.aggregate.common.@event
{
    using com.tibbo.aggregate.common.util;

    using JavaCompatibility;

    public class PersistenceOptions : ICloneable
    {

        //    private List<PersistenceBinding> persistenceBindings;

        //    private boolean dedicatedTablePreferred;
        //    private boolean perContextTablePreferred;

        //    private boolean persistContext = true;
        //    private boolean persistName = true;
        //    private boolean persistExpirationtime = true;
        //    private boolean persistLevel = true;
        //    private boolean persistPermissions = true;
        //    private boolean persistCount = true;
        //    private boolean persistAcknowledgements = true;
        //    private boolean persistEnrichments = true;
        //    private boolean persistFormat = true;
        //    private boolean persistData = true;

        //    @Override
        //  public PersistenceOptions clone()
        //    {
        //        try
        //        {
        //            PersistenceOptions clone = (PersistenceOptions)super.clone();
        //            clone.persistenceBindings = (List<PersistenceBinding>)CloneUtils.deepClone(persistenceBindings);
        //            return clone;
        //        }
        //        catch (CloneNotSupportedException ex)
        //        {
        //            throw new IllegalStateException(ex.getMessage(), ex);
        //        }
        //    }
        public object Clone()
        {
            return this.MemberwiseClone();
        }

        //    public List<PersistenceBinding> getPersistenceBindings()
            //    {
            //        return persistenceBindings != null ? Collections.unmodifiableList(persistenceBindings) : null;
            //    }

            //    public void addPersistenceBinding(PersistenceBinding binding)
            //    {
            //        if (persistenceBindings == null)
            //        {
            //            persistenceBindings = new LinkedList();
            //        }
            //        persistenceBindings.add(binding);
            //    }

            //    public boolean isDedicatedTablePreferred()
            //    {
            //        return dedicatedTablePreferred;
            //    }

            //    public void setDedicatedTablePreferred(boolean dedicatedTablePreferred)
            //    {
            //        this.dedicatedTablePreferred = dedicatedTablePreferred;
            //    }

            //    public boolean isPerContextTablePreferred()
            //    {
            //        return perContextTablePreferred;
            //    }

            //    public void setPerContextTablePreferred(boolean perContextTablePreferred)
            //    {
            //        this.perContextTablePreferred = perContextTablePreferred;
            //    }

            //    public boolean isPersistContext()
            //    {
            //        return persistContext;
            //    }

            //    public void setPersistContext(boolean persistContext)
            //    {
            //        this.persistContext = persistContext;
            //    }

            //    public boolean isPersistName()
            //    {
            //        return persistName;
            //    }

            //    public void setPersistName(boolean persistName)
            //    {
            //        this.persistName = persistName;
            //    }

            //    public boolean isPersistExpirationtime()
            //    {
            //        return persistExpirationtime;
            //    }

            //    public void setPersistExpirationtime(boolean persistExpirationtime)
            //    {
            //        this.persistExpirationtime = persistExpirationtime;
            //    }

            //    public boolean isPersistLevel()
            //    {
            //        return persistLevel;
            //    }

            //    public void setPersistLevel(boolean persistLevel)
            //    {
            //        this.persistLevel = persistLevel;
            //    }

            //    public boolean isPersistPermissions()
            //    {
            //        return persistPermissions;
            //    }

            //    public void setPersistPermissions(boolean persistPermissions)
            //    {
            //        this.persistPermissions = persistPermissions;
            //    }

            //    public boolean isPersistCount()
            //    {
            //        return persistCount;
            //    }

            //    public void setPersistCount(boolean persistCount)
            //    {
            //        this.persistCount = persistCount;
            //    }

            //    public boolean isPersistAcknowledgements()
            //    {
            //        return persistAcknowledgements;
            //    }

            //    public void setPersistAcknowledgements(boolean persistAcknowledgements)
            //    {
            //        this.persistAcknowledgements = persistAcknowledgements;
            //    }

            //    public boolean isPersistEnrichments()
            //    {
            //        return persistEnrichments;
            //    }

            //    public void setPersistEnrichments(boolean persistEnrichments)
            //    {
            //        this.persistEnrichments = persistEnrichments;
            //    }

            //    public boolean isPersistFormat()
            //    {
            //        return persistFormat;
            //    }

            //    public void setPersistFormat(boolean persistFormat)
            //    {
            //        this.persistFormat = persistFormat;
            //    }

            //    public boolean isPersistData()
            //    {
            //        return persistData;
            //    }

            //    public void setPersistData(boolean persistData)
            //    {
            //        this.persistData = persistData;
            //    }

            //    public void setPersistenceBindings(List<PersistenceBinding> persistenceBindings)
            //    {
            //        this.persistenceBindings = persistenceBindings;
            //    }
        }

}
