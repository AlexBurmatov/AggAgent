using System;
using LinkServer.LinkServer;

namespace com.tibbo.aggregate.webservice
{
    internal class LinkSeverProxy : LinkServerWebService
    {
        public LinkSeverProxy(String url)
        {
            Url = url;
        }
    }
}