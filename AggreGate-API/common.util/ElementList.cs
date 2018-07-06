using System;
using System.Collections.Generic;

namespace com.tibbo.aggregate.common.util
{
    public class ElementList : List<Element>
    {
        public Element getElement(String name)
        {
            foreach (var el in this)
            {
                if (el.getName() != null && el.getName() == name)
                {
                    return el;
                }
            }
            return null;
        }
    }
}