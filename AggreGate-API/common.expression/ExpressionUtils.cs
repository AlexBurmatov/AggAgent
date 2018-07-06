using System;
using System.Collections.Generic;
using System.Text;

namespace com.tibbo.aggregate.common.expression
{
    public class ExpressionUtils
    {
        public const char PARAM_ESCAPE_SINGLE = '\'';
        public const char PARAM_ESCAPE_DOUBLE = '\"';
        private const char PARAMS_DELIM = ',';
        private const char PARAMS_ESCAPE = '\\';
  
        public const String NULL_PARAM = "null";
    
        public static List<Object> getFunctionParameters(String paramsString, bool allowExpressions)
        {
            List<Object> parameters = new List<object>();
            bool insideSingleQuotedLiteral = false;
            bool insideDoubleQuotedLiteral = false;
            bool escaped = false;
            StringBuilder buf = new StringBuilder();
            for (int i = 0; i < paramsString.Length; i++)
            {
                char c = paramsString[i];
                if (c == PARAMS_ESCAPE)
                {
                    if (escaped)
                    {
                        escaped = false;
                        buf.Append(c);
                        continue;
                    }
                    else
                    {
                        escaped = true;
                        continue;
                    }
                }
                else if (insideSingleQuotedLiteral)
                {
                    if (c == PARAM_ESCAPE_SINGLE)
                    {
                        if (!escaped)
                        {
                            insideSingleQuotedLiteral = false;
                            String param = buf.ToString();
                            if (allowExpressions)
                            {
                                parameters.Add(new Expression(prepareParameter(param)));
                            }
                            else
                            {
                                parameters.Add(prepareParameter(param));
                            }
                            buf = null;
                        }
                    }
                }
                else if (insideDoubleQuotedLiteral)
                {
                    if (c == PARAM_ESCAPE_DOUBLE)
                    {
                        if (!escaped)
                        {
                            insideDoubleQuotedLiteral = false;
                            String param = buf.ToString();
                            parameters.Add(prepareParameter(param));
                            buf = null;
                        }
                    }
                 }
                else if (c == PARAMS_DELIM)
                {
                    if (!insideSingleQuotedLiteral && !insideDoubleQuotedLiteral)
                    {
                        if (buf != null)
                        {
                            String param = buf.ToString().Trim();
                            if (param.Length > 0)
                            {
                                parameters.Add(new Expression(prepareParameter(param)));
                            }
                        }
          
                        buf = new StringBuilder();
                        continue;
                    }
                 }
                else if (c == PARAM_ESCAPE_SINGLE && !insideDoubleQuotedLiteral)
                {
                    insideSingleQuotedLiteral = true;
                    buf = new StringBuilder();
                    continue;
                }
                else if (c == PARAM_ESCAPE_DOUBLE && !insideSingleQuotedLiteral)
                {
                    insideDoubleQuotedLiteral = true;
                    buf = new StringBuilder();
                    continue;
                }
      
                if (c != PARAMS_ESCAPE)
                {
                    escaped = false;
                }
      
                if (buf != null)
                {
                    buf.Append(c);
                }
            }
    
            if (buf != null)
            {
                String param = buf.ToString().Trim();
                if (param.Length > 0)
                {
                    parameters.Add(new Expression(prepareParameter(param)));
                }
            }
    
            if (insideSingleQuotedLiteral)
            {
                throw new ArgumentException("Illegal function parameters: " + parameters);
            }
    
            if (insideDoubleQuotedLiteral)
            {
                throw new ArgumentException("Illegal function parameters: " + parameters);
            }
    
            return parameters;
      }
  
      private static String prepareParameter(String parameter)
      {
            return parameter;
      }
  
      public static String getFunctionParameters(List<Object> parameters)
      {
            StringBuilder sb = new StringBuilder();
    
            int i = 0;
            foreach (Object param in parameters)
            {
                if (param == null)
                {
                    sb.Append(NULL_PARAM);
                }
                else
                {
                    if (param.GetType() == typeof(Expression))
                    {   
                        String value = param.ToString();
          
                        if (value.IndexOf(PARAMS_DELIM) != -1)
                        {
                            sb.Append(PARAM_ESCAPE_SINGLE);
                            sb.Append(value);
                            sb.Append(PARAM_ESCAPE_SINGLE);
                        }
                        else
                        {
                            sb.Append(value);
                        }   
          
                    }
                    else
                    {
                        sb.Append(PARAM_ESCAPE_DOUBLE);
                        sb.Append(param);
                        sb.Append(PARAM_ESCAPE_DOUBLE);
                    }  
                }
                if (i < parameters.Count - 1)
                {
                    sb.Append(PARAMS_DELIM);
                }
                    i++;
                }
    
                return sb.ToString();
        }
  
        public static long? generateBindingId()
        {
            Random rnd = new Random();
            long id = rnd.Next() * 2;
            return id;
        }
    }
}
