using System;
using System.Collections.Generic;
using System.IO;
using com.tibbo.aggregate.common.command;
using com.tibbo.aggregate.common.context;
using com.tibbo.aggregate.common.data;
using com.tibbo.aggregate.common.datatable;
using com.tibbo.aggregate.common.device;
using com.tibbo.aggregate.common.util;

namespace com.tibbo.aggregate.common.agent
{
    public class AgentImplementationController : AbstractClientController
    {
        private FormatCache formatCache;
        private KnownFormatCollector knownFormatCollector;

        public AgentImplementationController(StreamWrapper wrapper, ContextManager contextManager, FormatCache aFormatCache)
            : base(wrapper, contextManager)
        {
            formatCache = aFormatCache;
            knownFormatCollector = new KnownFormatCollector();
        }

        public override Boolean controllerShouldHandle(Event ev, ContextEventListener listener)
        {
            return true;
        }

        protected override void processMessageOperation(IncomingAggreGateCommand cmd, OutgoingAggreGateCommand ans)
        {
            base.processMessageOperation(cmd, ans);

            var context = cmd.getParameter(AggreGateCommand.INDEX_OPERATION_CONTEXT);

            var conManager = getContextManager();
            var con = conManager.get(context, getCallerController());

            if (con != null)
            {
                addNormalListener(con.getPath(), AbstractContext.E_UPDATED, getDefaultEventListener());
            }
        }

        protected override ClassicEncodingSettings createClassicEncodingSettings(Boolean useFormatCache)
        {
            ClassicEncodingSettings settings = base.createClassicEncodingSettings(useFormatCache);

            if (useFormatCache)
            {
                settings.setFormatCache(formatCache);
                settings.setKnownFormatCollector(knownFormatCollector);
            }

            settings.setEncodeFormat(!useFormatCache);
            return settings;
        }
    }
}