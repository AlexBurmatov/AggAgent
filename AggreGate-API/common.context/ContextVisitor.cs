namespace com.tibbo.aggregate.common.context
{
    public interface ContextVisitor<T> where T : Context
    {
        void visit(T context);
    }

    internal class DelegatedContextVisitor<T> : ContextVisitor<T> where T : Context
    {
        public delegate void VisitDelegate(T context);

        public DelegatedContextVisitor(VisitDelegate aVisitDelegate)
        {
            visitor = aVisitDelegate;
        }

        protected VisitDelegate visitor;

        public void visit(T context)
        {
            visitor(context);
        }
    }
}