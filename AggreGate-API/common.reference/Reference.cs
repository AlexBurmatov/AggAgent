using System;
using System.Collections.Generic;
using System.Text;
using com.tibbo.aggregate.common.context;
using com.tibbo.aggregate.common.expression;
using JavaCompatibility;

namespace com.tibbo.aggregate.common.reference
{
    public class Reference : ICloneable
    {
        // Format: schema/server^context:entity('param1', expression, null, ...)$field[row]#property

        public const String SCHEMA_FORM = "form";
        public const String SCHEMA_TABLE = "table";
        public const String SCHEMA_STATISTICS = "statistics";
        public const String SCHEMA_ENVIRONMENT = "env";
        public const String SCHEMA_PARENT = "parent";
        public const String SCHEMA_MENU = "menu";

        // Deprecated in favour of new action reference syntax: context.path:action!
        // Support should be terminated in AggreGate 6
        public const String SCHEMA_ACTION = "action";

        public const char EVENT_SIGN = '@';
        public const char ACTION_SIGN = '!';
        public const char PARAMS_BEGIN = '(';
        public const char PARAMS_END = ')';
        public const char SCHEMA_END = '/';
        public const char SERVER_END = '^';
        public const char CONTEXT_END = ':';
        public const char FIELD_BEGIN = '$';
        public const char ROW_BEGIN = '[';
        public const char ROW_END = ']';
        public const char PROPERTY_BEGIN = '#';

        public const int APPEARANCE_LINK = 1;
        public const int APPEARANCE_BUTTON = 2;

        private String image;

        private String schema;
        private String server;
        private String context;
        private String entity;
        private int entityType = ContextUtils.ENTITY_VARIABLE;
        private List<Object> parameters = new List<object>(); // May contain Strings, Expressions and NULLs
        private String field;
        private int row;
        private String property;
        private int appearance = APPEARANCE_LINK;

        public Reference()
        {

        }

        public Reference(String source)
        {
            parse(source);
        }

        public Reference(String server, String context)
        {
            this.server = server;
            this.context = context;
        }

        public Reference(String entity, int entityType, String field)
        {
            this.entity = entity;
            this.entityType = entityType;
            this.field = field;
        }

        public Reference(String context, String entity, int entityType, String field)
            : this(entity, entityType, field)
        {
            this.context = context;
        }

        public Reference(String context, String entity, int entityType)
        {
            this.context = context;
            this.entity = entity;
            this.entityType = entityType;
        }

        public Reference(String context, String function, Object[] parameters)
        {
            this.context = context;
            this.entity = function;
            this.entityType = ContextUtils.ENTITY_FUNCTION;
            this.parameters.AddRange(parameters);
        }

        protected void parse(String source)
        {
            source = source.Trim();

            bool isFunction = false;
            bool isEvent = false;
            bool isAction = false;

            image = source;

            String src = image;

            int paramsBegin = src.IndexOf(PARAMS_BEGIN);
            int paramsEnd = src.LastIndexOf(PARAMS_END);

            if (paramsBegin != -1)
            {
                if (paramsEnd == -1)
                {
                    throw new ArgumentException("No closing ')' for function parameters");
                }

                isFunction = true;

                String paramsSrc = src.Substring(paramsBegin + 1, paramsEnd);

                parameters = ExpressionUtils.getFunctionParameters(paramsSrc, true);

                entityType = ContextUtils.ENTITY_FUNCTION;

                src = src.Substring(0, paramsBegin) + src.Substring(paramsEnd + 1);
            }
            else
            {
                int eventSignPos = src.LastIndexOf(EVENT_SIGN);

                if (eventSignPos != -1)
                {
                    isEvent = true;

                    entityType = ContextUtils.ENTITY_EVENT;

                    src = src.Substring(0, eventSignPos) + src.Substring(eventSignPos + 1);
                }
                else
                {
                    int actionSignPos = src.LastIndexOf(ACTION_SIGN);

                    if (actionSignPos != -1)
                    {
                        isAction = true;

                        entityType = ContextUtils.ENTITY_ACTION;

                        src = src.Substring(0, actionSignPos) + src.Substring(actionSignPos + 1);
                    }
                }
            }

            int schemaEnd = src.IndexOf(SCHEMA_END);

            if (schemaEnd != -1)
            {
                schema = src.Substring(0, schemaEnd);
                src = src.Substring(schemaEnd + 1);
            }

            int serverEnd = src.IndexOf(SERVER_END);

            if (serverEnd != -1)
            {
                server = src.Substring(0, serverEnd);
                src = src.Substring(serverEnd + 1);
            }

            int contextEnd = src.IndexOf(CONTEXT_END);

            if (contextEnd != -1)
            {
                context = src.Substring(0, contextEnd);
                src = src.Substring(contextEnd + 1);
            }

            int propertyBegin = src.IndexOf(PROPERTY_BEGIN);

            if (propertyBegin != -1)
            {
                property = src.Substring(propertyBegin + 1);
                src = src.Substring(0, propertyBegin);
            }

            int rowBegin = src.IndexOf(ROW_BEGIN);
            int rowEnd = src.IndexOf(ROW_END);

            if (rowBegin != -1)
            {
                if (rowEnd == -1)
                {
                    throw new ArgumentException("No closing ']' in row reference");
                }

                row = Convert.ToInt32(src.Substring(rowBegin + 1, rowEnd));

                src = src.Substring(0, rowBegin);
            }

            int fieldBegin = src.IndexOf(FIELD_BEGIN);

            if (fieldBegin != -1)
            {
                entity = src.Substring(0, fieldBegin);
                field = (fieldBegin != src.Length - 1) ? src.Substring(fieldBegin + 1) : null;
            }
            else if (src.Length > 0)
            {
                if (context != null || isFunction || isEvent || isAction)
                {
                    entity = src;
                }
                else
                {
                    field = src;
                }
            }
        }

        public String getServer()
        {
            return server;
        }

        public String getContext()
        {
            return context;
        }

        public String getEntity()
        {
            return entity;
        }

        public int getEntityType()
        {
            return entityType;
        }

        public String getField()
        {
            return field;
        }

        public List<Object> getParameters()
        {
            return parameters;
        }

        public int getRow()
        {
            return row;
        }

        public String getSchema()
        {
            return schema;
        }

        public String getProperty()
        {
            return property;
        }

        public String getImage()
        {
            if (image != null)
            {
                return image;
            }
            else
            {
                return createImage();
            }
        }

        private String createImage()
        {
            StringBuilder sb = new StringBuilder();

            if (schema != null)
            {
                sb.Append(schema);
                sb.Append(SCHEMA_END);
            }

            if (server != null)
            {
                sb.Append(server);
                sb.Append(SERVER_END);
            }

            if (context != null)
            {
                sb.Append(context);
                sb.Append(CONTEXT_END);
            }

            if (entity != null)
            {
                sb.Append(entity);

                if (entityType == ContextUtils.ENTITY_FUNCTION)
                {
                    sb.Append(PARAMS_BEGIN);
                    sb.Append(ExpressionUtils.getFunctionParameters(parameters));
                    sb.Append(PARAMS_END);
                }

                if (entityType == ContextUtils.ENTITY_EVENT)
                {
                    sb.Append(EVENT_SIGN);
                }

                if (entityType == ContextUtils.ENTITY_ACTION)
                {
                    sb.Append(ACTION_SIGN);
                }

                if (field != null || (context == null && entityType == ContextUtils.ENTITY_VARIABLE))
                {
                    sb.Append(FIELD_BEGIN);
                }
            }

            if (field != null)
            {
                sb.Append(field);
            }

            sb.Append(ROW_BEGIN);
            sb.Append(row);
            sb.Append(ROW_END);

            if (property != null)
            {
                sb.Append(PROPERTY_BEGIN);
                sb.Append(property);
            }

            image = sb.ToString();

            return image;
        }

        public override String ToString()
        {
            return getImage();
        }

        public void setContext(String context)
        {
            this.context = context;
            image = null;
        }

        public void setEntity(String entity)
        {
            this.entity = entity;
            image = null;
        }

        public void setEntityType(int entityType)
        {
            this.entityType = entityType;
            image = null;
        }

        public void addParameter(String parameter)
        {
            parameters.Add(parameter);
        }

        public void addParameter(Expression parameter)
        {
            parameters.Add(parameter);
        }

        public void setField(String field)
        {
            this.field = field;
            image = null;
        }

        public void setProperty(String property)
        {
            this.property = property;
            image = null;
        }

        public void setSchema(String schema)
        {
            this.schema = schema;
            image = null;
        }

        public void setRow(int row)
        {
            this.row = row;
            image = null;
        }

        public void setServer(String server)
        {
            this.server = server;
            image = null;
        }

        public object Clone()
        {
            try
            {
                return MemberwiseClone();
            }
            catch (CloneNotSupportedException ex)
            {
                throw new InvalidOperationException(ex.ToString(), ex);
            }
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

            if (GetType() != obj.GetType())
            {
                return false;
            }

            return getImage().Equals(((Reference)obj).getImage());
        }

        public override int GetHashCode()
        {
            return getImage().GetHashCode();
        }

        public int getAppearance()
        {
            return appearance;
        }

        public void setAppearance(int appearance)
        {
            this.appearance = appearance;
        }

    }
}
