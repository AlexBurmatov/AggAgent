using System;
using System.Collections.Generic;
using com.tibbo.aggregate.common.context;
using com.tibbo.aggregate.common.datatable;
using com.tibbo.aggregate.common.device;

namespace com.tibbo.aggregate.common.agent
{
    public class AgentContext : AbstractContext
    {
        public const String V_DATE = "date";
        
        public const String F_LOGIN = "login";
        public const String F_REGISTER = "register";
        public const String F_SYNCHRONIZED = "synchronized";
        public const String F_CONFIRM_EVENT = "confirmEvent";
        public const String F_ACKNOWLEDGE_EVENT = "acknowledgeEvent";
        public const String F_GET_HISTORY = "getHistory";

        public const String V_ASSETS = "assets";

        public const String FIELD_ID = "id";
        public const String FIELD_ENABLED = "enabled";
        public const String FIELD_CHILDREN = "children";
        public const String FIELD_DESCRIPTION = "description";

        public const String AGGREGATE = "aggregate";

        public const String RECORDS = "records";

        public const String PROPERTY_ENABLED = "enabled";

        public const String E_EVENT_CONFIRMED = "eventConfirmed";

        public const String E_EVENT_ACKNOWLEDGED = "eventAcknowledged";

        public const String FIF_LOGIN_CHALLENGE = "challenge";

        public const String FIF_CONFIRM_EVENT_ID = "id";

        public const String FIF_ACKNOWLEDGE_EVENT_ID = "id";
        public const String FIF_ACKNOWLEDGE_EVENT_DATE = "date";
        public const String FIF_ACKNOWLEDGE_EVENT_AUTHOR = "author";
        public const String FIF_ACKNOWLEDGE_EVENT_ACKNOWLEDGEMENT = "acknowledgement";
        public const String FIF_ACKNOWLEDGE_EVENT_EVENT_DATA = "eventData";

        public const String FOF_LOGIN_OWNER = "owner";
        public const String FOF_LOGIN_NAME = "name";
        public const String FOF_LOGIN_RESPONSE = "response";

        public const String FOF_REGISTER_PASSWORD = "password";

        public const String FOF_GET_HISTORY_VARIABLE = "variable";
        public const String FOF_GET_HISTORY_TIMESTAMP = "timestamp";
        public const String FOF_GET_HISTORY_VALUE = "value";

        public const String EF_EVENT_CONFIRMED_ID = "id";

        public static readonly TableFormat FIFT_LOGIN = new TableFormat(1, 1, "<" + FIF_LOGIN_CHALLENGE + "><S>");

        public static readonly TableFormat FOFT_LOGIN = new TableFormat(1, 1);

        public static readonly TableFormat FOFT_REGISTER = new TableFormat(1, 1, "<" + FOF_REGISTER_PASSWORD + "><S>");

        public static readonly TableFormat FIFT_CONFIRM_EVENT = new TableFormat(1, 1, "<" + FIF_CONFIRM_EVENT_ID + "><L>");

        public static readonly TableFormat EFT_EVENT_CONFIRMED = new TableFormat(1, 1, "<" + EF_EVENT_CONFIRMED_ID + "><L>");

        public static readonly TableFormat FOFT_GET_HISTORY = new TableFormat();

        public static readonly TableFormat FIFT_ACKNOWLEDGE_EVENT = new TableFormat(1, 1);

        public static readonly TableFormat FOFT_ASSET = new TableFormat();

        private readonly RemoteLinkServer server;

        private readonly String name;

        private readonly bool eventConfirmation;

        private Boolean synchronized;

        static AgentContext()
        {
            FOFT_LOGIN.addField("<" + FOF_LOGIN_OWNER + "><S>");
            FOFT_LOGIN.addField("<" + FOF_LOGIN_NAME + "><S>");
            FOFT_LOGIN.addField("<" + FOF_LOGIN_RESPONSE + "><S>");

            FOFT_GET_HISTORY.addField("<" + FOF_GET_HISTORY_VARIABLE + "><S>");
            FOFT_GET_HISTORY.addField("<" + FOF_GET_HISTORY_TIMESTAMP + "><D>");
            FOFT_GET_HISTORY.addField("<" + FOF_GET_HISTORY_VALUE + "><T>");

            FIFT_ACKNOWLEDGE_EVENT.addField("<" + FIF_ACKNOWLEDGE_EVENT_ID + "><L><F=N>");
            FIFT_ACKNOWLEDGE_EVENT.addField("<" + FIF_ACKNOWLEDGE_EVENT_DATE + "><D>");
            FIFT_ACKNOWLEDGE_EVENT.addField("<" + FIF_ACKNOWLEDGE_EVENT_AUTHOR + "><S><F=N>");
            FIFT_ACKNOWLEDGE_EVENT.addField("<" + FIF_ACKNOWLEDGE_EVENT_ACKNOWLEDGEMENT + "><S>");
            FIFT_ACKNOWLEDGE_EVENT.addField("<" + FIF_ACKNOWLEDGE_EVENT_EVENT_DATA + "><T>");

            FOFT_ASSET.addField("<" + FIELD_ID + "><S><F=HRK>");
            FOFT_ASSET.addField("<" + FIELD_DESCRIPTION + "><S><F=R><D=" + Cres.get().getString("description") + ">");
            FOFT_ASSET.addField("<" + FIELD_ENABLED + "><B><A=1><D=" + Cres.get().getString("enabled") + ">");

            FOFT_ASSET.addField(FieldFormat.create("<" + FIELD_CHILDREN + "><T><F=N><D=" + Cres.get().getString("devNestedAssets") + ">"));

            FOFT_ASSET.setNamingExpression(AGGREGATE + "({}, \"{env/previous} + ({" + FIELD_ENABLED + "} ? 1 : 0)\", 0) + '/' + {#" + RECORDS + "}");

            String reff = FIELD_CHILDREN + "#" + PROPERTY_ENABLED;
            String exp = "{" + FIELD_ENABLED + "}";
            FOFT_ASSET.addBinding(reff, exp);
        }

        public AgentContext(RemoteLinkServer aServer, String nameString, bool eventConfirmation)
            : base(ContextUtils.CTX_ROOT)
        {
            server = aServer;
            name = nameString;
            this.eventConfirmation = eventConfirmation;

            loginImpl = new DelegatedFunctionImplementation((con, def, caller, request, parameters) =>
                {
                    var challenge = parameters.rec().getString(FIF_LOGIN_CHALLENGE);
                    var response = Md5Utils.hexHash(challenge + server.getPassword());
                    return
                        new DataRecord(FOFT_LOGIN).addString(server.getUsername()).addString(name).addString(response).wrap();
                });

            registerImpl =
                new DelegatedFunctionImplementation(
                    (con, def, caller, request, parameters) =>
                    new DataRecord(FOFT_REGISTER).addString(server.getPassword()).wrap());

            synchronizedImpl = new DelegatedFunctionImplementation((con, def, caller, request, parameters) =>
                {
                    Console.Out.WriteLine("+++++++ " + "Synchronized!" + " +++++++");

                    setSynchronized(true);
                    return null;
                });

            confirmEventImpl = new DelegatedFunctionImplementation((con, def, caller, request, parameters) =>
                {
                    confirmEvent(parameters.rec().getLong(FIF_CONFIRM_EVENT_ID));
                    return null;
                });

            acknowledgeEventImpl = new DelegatedFunctionImplementation((con, def, caller, request, parameters) =>
            {
                acknowledgeEvent(parameters.rec().getLong(FIF_ACKNOWLEDGE_EVENT_ID),
                    parameters.rec().getDate(FIF_ACKNOWLEDGE_EVENT_DATE),
                    parameters.rec().getString(FIF_ACKNOWLEDGE_EVENT_AUTHOR),
                    parameters.rec().getString(FIF_ACKNOWLEDGE_EVENT_ACKNOWLEDGEMENT),
                    parameters.rec().getDataTable(FIF_ACKNOWLEDGE_EVENT_EVENT_DATA));
                    return null;
                });

            getHistoryImpl = new DelegatedFunctionImplementation((con, def, caller, request, parameters) =>
            {
                  DataTable res = new DataTable(def.getOutputFormat());

                  foreach (HistoricalValue hv in getHistory())
                  {
                        DataRecord rec = res.addRecord();
                        rec.addString(hv.getVariable());
                        rec.addDate(hv.getTimestamp());
                        rec.addDataTable(hv.getValue());   
                  }
      
                  return res;
            });
        }

        public override void setupMyself()
        {
            base.setupMyself();
        
            var def = new FunctionDefinition(F_LOGIN, FIFT_LOGIN, FOFT_LOGIN, Cres.get().getString("login"));
            def.setImplementation(loginImpl);
            addFunctionDefinition(def);

            def = new FunctionDefinition(F_REGISTER, TableFormat.EMPTY_FORMAT, FOFT_REGISTER, Cres.get().getString("register"));
            def.setImplementation(registerImpl);
            addFunctionDefinition(def);

            def = new FunctionDefinition(F_SYNCHRONIZED, TableFormat.EMPTY_FORMAT, TableFormat.EMPTY_FORMAT);
            def.setImplementation(synchronizedImpl);
            addFunctionDefinition(def);

            var fd = new FunctionDefinition(F_CONFIRM_EVENT, FIFT_CONFIRM_EVENT, TableFormat.EMPTY_FORMAT);
            fd.setImplementation(confirmEventImpl);
            addFunctionDefinition(fd);

            fd = new FunctionDefinition(F_ACKNOWLEDGE_EVENT, FIFT_ACKNOWLEDGE_EVENT, TableFormat.EMPTY_FORMAT);
            fd.setImplementation(acknowledgeEventImpl);
            addFunctionDefinition(fd);

            fd = new FunctionDefinition(F_GET_HISTORY, TableFormat.EMPTY_FORMAT, FOFT_GET_HISTORY);
            fd.setImplementation(getHistoryImpl);
            addFunctionDefinition(fd);

            if (eventConfirmation)
            {
                EventDefinition edCon = new EventDefinition(E_EVENT_CONFIRMED, EFT_EVENT_CONFIRMED);
                addEventDefinition(edCon);
            }

            EventDefinition edAck = new EventDefinition(E_EVENT_ACKNOWLEDGED, FIFT_ACKNOWLEDGE_EVENT);
            addEventDefinition(edAck);
        }

        public override DataTable decodeRemoteDataTable(TableFormat format, string s)
        {
            throw new NotSupportedException();
        }

        public override void reinitialize()
        {
            throw new NotSupportedException();
        }

        public override DataTable callRemoteFunction(string nameString, TableFormat outputFormat, DataTable parameters)
        {
            throw new NotSupportedException();
        }

        public override DataTable getRemoteVariable(TableFormat format, string nameString)
        {
            throw new NotSupportedException();
        }

        public Boolean isSynchronized()
        {
            return synchronized;
        }

        public void setSynchronized(Boolean isSynchronized)
        {
            synchronized = isSynchronized;
            Console.Out.WriteLine("[" + DateTime.Now + "] Set synchronized: " + synchronized);
        }

        public RemoteLinkServer getServer()
        {
            return server;
        }

        protected List<HistoricalValue> getHistory()
        {
            return new List<HistoricalValue>();
        }

        protected void confirmEvent(long id)
        {
            if (getEventDefinition(E_EVENT_CONFIRMED) != null)
            {
                fireEvent(E_EVENT_CONFIRMED, id);
            }
        }

        protected void acknowledgeEvent(long id, DateTime date, String author, String acknowledgement, DataTable data)
        {
            if (getEventDefinition(E_EVENT_ACKNOWLEDGED) != null)
            {
                fireEvent(E_EVENT_ACKNOWLEDGED, id, date, author, acknowledgement, data);
            }
        }

        private readonly FunctionImplementation confirmEventImpl;

        private readonly FunctionImplementation loginImpl;

        private readonly FunctionImplementation registerImpl;

        private readonly FunctionImplementation synchronizedImpl;

        private readonly FunctionImplementation acknowledgeEventImpl;

        private readonly FunctionImplementation getHistoryImpl;

    }
}
