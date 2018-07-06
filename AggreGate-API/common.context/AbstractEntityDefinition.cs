using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.tibbo.aggregate.common.context
{
    using com.tibbo.aggregate.common.datatable;

    public class AbstractEntityDefinition : EntityDefinition
    {
        private string name;
        private string description;
        private string help;
        private string group;
        private int index;
        private string iconId;
        private object owner;


        public string getName()
        {
            return name;
        }

        public void setName(string nameString)
        {
            //if (Log.CONTEXT.isDebugEnabled())
            //{
            //    try
            //    {
            //        ValidatorHelper.NAME_SYNTAX_VALIDATOR.validate(name);
            //    }
            //    catch (ValidationException ve)
            //    {
            //        Log.CONTEXT.debug(getClass().getSimpleName() + " name '" + name + "' breaks naming policy. The entity can be broken after ecnoding->decoding sequence.", new Exception());
            //    }
            //}
            this.name = nameString;
        }

        public virtual void setDescription(string descriptionString)
        {
            this.description = descriptionString;
        }

        public void setHelp(string helpString)
        {
            this.help = helpString;
        }

        public void setGroup(string groupString)
        {
            this.group = groupString;
        }

        public string getDescription()
        {
            return this.description;
        }

        public string getHelp()
        {
            return this.help;
        }

        public string getGroup()
        {
            return this.group;
        }

        public int getIndex()
        {
            return this.index;
        }

        public void setIndex(int index)
        {
            this.index = index;
        }

        public void setIconId(string iconId)
        {
            this.iconId = iconId;
        }

        public string getIconId()
        {
            return this.iconId;
        }

        public object getOwner()
        {
            return this.owner;
        }

        public void setOwner(object owner)
        {
            this.owner = owner;
        }

        public string toString()
        {
            return this.description != null ? this.description : this.name;
        }

        public string toDetailedString()
        {
            return this.description != null ? this.description + " (" + this.name + ")" : this.name;
        }

        internal EventDefinition Clone()
        {
            throw new NotImplementedException();
        }
    }
}
