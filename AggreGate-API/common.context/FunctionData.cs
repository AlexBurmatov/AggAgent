using System;
using System.Reflection;

namespace com.tibbo.aggregate.common.context
{
    using System.Threading;

    public class FunctionData : IComparable<FunctionData>
    {
        private FunctionDefinition definition;

        private readonly ReentrantLock executionLock = new ReentrantLock();

        private long executionCount;

        private bool implementationCached;

        private MethodInfo implementationMethod;

        public FunctionData(FunctionDefinition definition) : base()
        {
            this.definition = definition;
        }

        public void registerExecution()
        {
            this.executionCount++;
        }

        public FunctionDefinition getDefinition()
        {
            return this.definition;
        }

        public ReentrantLock getExecutionLock()
        {
            return this.executionLock;
        }

        public long getExecutionCount()
        {
            return this.executionCount;
        }

        public bool isImplementationCached()
        {
            return this.implementationCached;
        }

        public void setImplementationCached(bool implementationCached)
        {
            this.implementationCached = implementationCached;
        }

        public MethodInfo getImplementationMethod()
        {
            return this.implementationMethod;
        }

        public void setImplementationMethod(MethodInfo implementationMethod)
        {
            this.implementationMethod = implementationMethod;
        }

        public int CompareTo(FunctionData d)
        {
            if (d != null)
            {
                return this.definition.CompareTo(d.getDefinition());
            }

            return 0;
        }

    }
}