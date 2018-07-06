using System;
using System.Reflection;
using System.Threading;

namespace com.tibbo.aggregate.common.context
{
    public class VariableData : IComparable<VariableData>
    {
        private VariableDefinition definition;

        private Object value;

        private ReaderWriterLockSlim readWriteLock = new ReaderWriterLockSlim();
        private long getCount;
        private long setCount;
        private bool getterCached;
        private bool setterCached;
        private MethodInfo getterMethod;
        private MethodInfo setterMethod;

        public VariableData(VariableDefinition definition)
        {
            this.definition = definition;
        }

        public void registerGetOperation()
        {
            getCount++;
        }

        public void registerSetOperation()
        {
            setCount++;
        }

        public VariableDefinition getDefinition()
        {
            return definition;
        }

        public object getValue()
        {
            return value;
        }

        public void setValue(Object value)
        {
            this.value = value;
        }

        public ReaderWriterLockSlim getReadWriteLock()
        {
            return readWriteLock;
        }

        public long getGetCount()
        {
            return getCount;
        }

        public long getSetCount()
        {
            return setCount;
        }

        public bool isGetterCached()
        {
            return getterCached;
        }

        public void setGetterCached(bool getterCached)
        {
            this.getterCached = getterCached;
        }

        public bool isSetterCached()
        {
            return setterCached;
        }

        public void setSetterCached(bool setterCached)
        {
            this.setterCached = setterCached;
        }

        public MethodInfo getGetterMethod()
        {
            return getterMethod;
        }

        public void setGetterMethod(MethodInfo getter)
        {
            this.getterMethod = getter;
        }

        public MethodInfo getSetterMethod()
        {
            return setterMethod;
        }

        public void setSetterMethod(MethodInfo setter)
        {
            this.setterMethod = setter;
        }

        public int CompareTo(VariableData d)
        {
            if (d != null)
            {
                return definition.CompareTo(d.getDefinition());
            }

            return 0;
        }

    }
}