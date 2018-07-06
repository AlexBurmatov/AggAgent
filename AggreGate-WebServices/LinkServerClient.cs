using System;
using System.Text;
using System.Web;
using com.tibbo.aggregate.common.datatable;

namespace com.tibbo.aggregate.webservice
{
    public class LinkServerClient
    {
        private readonly String username;
        private readonly String password;

        private readonly LinkSeverProxy service;

        public LinkServerClient(String usernameString, String passwordString, String url)
        {
            username = usernameString;
            password = passwordString;
            service = new LinkSeverProxy(url);
        }

        public DataTable get(String context, String variable)
        {
            var res = service.get(username, password, context, variable);
            var dt = HttpUtility.UrlDecode(res, Encoding.UTF8);

            return new DataTable(dt);
        }

        public String getXML(String context, String variable)
        {
            return HttpUtility.UrlDecode(service.getXML(username, password, context, variable),
                                         Encoding.UTF8);
        }

        public void set(String context, String variable, DataTable value)
        {
            service.set(username, password, context, variable,
                        HttpUtility.UrlEncode(value.encode(), Encoding.UTF8));
        }

        public void setXML(String context, String variable, String value)
        {
            service.setXML(username, password, context, variable,
                           HttpUtility.UrlEncode(value, Encoding.UTF8));
        }

        public DataTable call(String context, String variable, DataTable parameters)
        {
            return
                new DataTable(
                    HttpUtility.UrlDecode(
                        service.call(username, password, context, variable,
                                     HttpUtility.UrlEncode(parameters.encode(), Encoding.UTF8)),
                        Encoding.UTF8));
        }

        public void setByStringArray(String context, String variable, String[] values)
        {
            service.setByStringArray(username, password, context, variable, values);
        }

        public DataTable callByStringArray(String context, String variable, String[] values)
        {
            return
                new DataTable(
                    HttpUtility.UrlDecode(service.callByStringArray(username, password, context, variable, values),
                                          Encoding.UTF8));
        }

        public String callXML(String context, String variable, String parameters)
        {
            return
                HttpUtility.UrlDecode(
                    service.callXML(username, password, context, variable,
                                    HttpUtility.UrlEncode(parameters, Encoding.UTF8)),
                    Encoding.UTF8);
        }
    }
}