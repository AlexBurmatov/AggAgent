using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.tibbo.aggregate.common.context
{
    interface EntityDefinition
    {
        string getName();

        string getDescription();

        string getHelp();

        string getGroup();

        int getIndex();

        string getIconId();

        string toDetailedString();

        object getOwner();

    }
}
