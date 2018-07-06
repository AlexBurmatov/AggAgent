using System;
using com.tibbo.aggregate.common.expression;
using com.tibbo.aggregate.common.reference;
using JavaCompatibility;

namespace com.tibbo.aggregate.common.binding
{
    public class Binding
    {
        private long? id;
  
        private Reference target;
  
        private Expression expression;
  
        private String queue;
  
        public Binding(Reference target, Expression expression)
        {
            this.target = target;
            this.expression = expression;
        }
  
        public Binding(String reference, String expression)
        {
            this.target = new Reference(reference);
            this.expression = new Expression(expression);
        }
  
        public Expression getExpression()
        {
            return expression;
        }
  
        public Reference getTarget()
        {
            return target;
        }
  
        public long? getId()
        {
            return id;
        }
  
        public void setId(long id)
        {
            this.id = id;
        }
  
        public String getQueue()
        {
            return queue;
        }
  
        public void setQueue(String queue)
        {
            this.queue = queue;
        }
  
        public override int GetHashCode()
        {
            const int prime = 31;
            int result = 1;
            result = prime * result + ((expression == null) ? 0 : expression.GetHashCode());
            result = prime * result + ((id == null) ? 0 : id.GetHashCode());
            result = prime * result + ((target == null) ? 0 : target.GetHashCode());
            return result;
        }

        public override bool Equals(Object obj)
        {
            if (this == obj)
            {
                return true;
            }

            if (obj == null)
            {
                return false;
            }
            if (this.GetType() != obj.GetType())
            {
                return false;
            }
            Binding other = (Binding) obj;
            if (expression == null)
            {
                if (other.expression != null)
                {
                    return false;
                }
            }
            else if (!expression.Equals(other.expression))
            {
                return false;
            }
            if (id == null)
            {
                if (other.id != null)
                {
                    return false;
                }
            }
            else if (!id.Equals(other.id))
            {
                return false;
            }
            if (target == null)
            {
            if (other.target != null)
            {
                return false;
            }
        }
        else if (!target.Equals(other.target))
        {
            return false;
        }
        return true;
    }
  
    public object Clone()
    {
        try
        {
            Binding clone = (Binding) MemberwiseClone();
            clone.target = (Reference) target.Clone();
            clone.expression = expression != null ? (Expression) expression.Clone() : null;
            clone.id = id == null ? null : ExpressionUtils.generateBindingId();
            return clone;
        }
        catch (CloneNotSupportedException ex)
        {
            throw new InvalidOperationException(ex.ToString(), ex);
        }
    }
  
        public override String ToString()
        {
            return target + " = " + expression;
        }
    }
}
