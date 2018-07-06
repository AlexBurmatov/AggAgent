using System;
using System.Collections.Generic;
using System.Text;
using com.tibbo.aggregate.common.command;
using com.tibbo.aggregate.common.context;
using com.tibbo.aggregate.common.datatable;
using com.tibbo.aggregate.common.server;
using JavaCompatibility;

namespace com.tibbo.aggregate.common.device
{
    public class RemoteDeviceContextProxy<TDevice> : AbstractContext where TDevice : AggreGateDevice
    {
        public const String F_LOCAL_REINITIALIZE = "localReinitialize";

        private Boolean notManageRemoteListenersBoolean;

        private Boolean localInitComplete;

        private Boolean initializing;
        private Boolean initialized;

        protected AggreGateDeviceController controller;

        private static readonly List<string> AUTO_LISTENED_EVENTS = new List<String>();

        static RemoteDeviceContextProxy()
        {
            AUTO_LISTENED_EVENTS.Add(E_CHILD_ADDED);
            AUTO_LISTENED_EVENTS.Add(E_CHILD_REMOVED);
            AUTO_LISTENED_EVENTS.Add(E_VARIABLE_ADDED);
            AUTO_LISTENED_EVENTS.Add(E_VARIABLE_REMOVED);
            AUTO_LISTENED_EVENTS.Add(E_FUNCTION_ADDED);
            AUTO_LISTENED_EVENTS.Add(E_FUNCTION_REMOVED);
            AUTO_LISTENED_EVENTS.Add(E_EVENT_ADDED);
            AUTO_LISTENED_EVENTS.Add(E_EVENT_REMOVED);
            AUTO_LISTENED_EVENTS.Add(E_INFO_CHANGED);
            AUTO_LISTENED_EVENTS.Add(E_DESTROYED);

            AUTO_LISTENED_EVENTS.Add(LinkServerContextConstants.E_ACTION_ADDED);
            AUTO_LISTENED_EVENTS.Add(LinkServerContextConstants.E_ACTION_REMOVED);
            AUTO_LISTENED_EVENTS.Add(LinkServerContextConstants.E_ACTION_STATE_CHANGED);
            AUTO_LISTENED_EVENTS.Add(LinkServerContextConstants.E_VISIBLE_INFO_CHANGED);
            AUTO_LISTENED_EVENTS.Add(LinkServerContextConstants.E_VISIBLE_CHILD_ADDED);
            AUTO_LISTENED_EVENTS.Add(LinkServerContextConstants.E_VISIBLE_CHILD_REMOVED);
        }

        public RemoteDeviceContextProxy(String name, AggreGateDeviceController aController) : base(name)
        {
            controller = aController;
            clear();
        }

        public override void setupMyself()
        {
            base.setupMyself();

            setFireUpdateEvents(false);
            setPermissionCheckingEnabled(false);
            setChildrenSortingEnabled(false);

            addLocalFunctionDefinitions();

            addEventListener(E_CHILD_ADDED, new DefaultContextEventListener<CallerController<CallerData>>(ev =>
                {
                    var child = ev.getData().rec().getString(EF_CHILD_ADDED_CHILD);
                    if (getChild(child) == null)
                    {
                        addChild(createNewProxyContext(child));
                    }
                }));

            addEventListener(E_CHILD_REMOVED,
                             new DefaultContextEventListener<CallerController<CallerData>>(ev =>
                                 {
                                     var child = ev.getData().rec().getString(EF_CHILD_REMOVED_CHILD);
                                     if (child != null)
                                     {
                                         removeChild(child);
                                     }
                                 }));

            addEventListener(E_VARIABLE_ADDED,
                             new DefaultContextEventListener<CallerController<CallerData>>(ev =>
                                 {
                                     var def = varDefFromDataRecord(ev.getData().rec());
                                     if (getVariableDefinition(def.getName()) == null)
                                     {
                                         addVariableDefinition(def);
                                     }
                                 }));

            addEventListener(E_VARIABLE_REMOVED,
                             new DefaultContextEventListener<CallerController<CallerData>>(
                                 ev => removeVariableDefinition(ev.getData().rec().getString(EF_VARIABLE_REMOVED_NAME))));

            addEventListener(E_FUNCTION_ADDED,
                             new DefaultContextEventListener<CallerController<CallerData>>(ev =>
                                 {
                                     var def = funcDefFromDataRecord(ev.getData().rec());
                                     if (getFunctionDefinition(def.getName()) == null)
                                     {
                                         addFunctionDefinition(def);
                                     }
                                 }));

            addEventListener(E_FUNCTION_REMOVED,
                             new DefaultContextEventListener<CallerController<CallerData>>(
                                 ev => removeFunctionDefinition(ev.getData().rec().getString(EF_FUNCTION_REMOVED_NAME))));

            addEventListener(E_EVENT_ADDED,
                             new DefaultContextEventListener<CallerController<CallerData>>(ev =>
                                 {
                                     var def = evtDefFromDataRecord(ev.getData().rec());
                                     if (getEventDefinition(def.getName()) == null)
                                     {
                                         addEventDefinition(def);
                                     }
                                 }));

            addEventListener(E_EVENT_REMOVED,
                             new DefaultContextEventListener<CallerController<CallerData>>(
                                 ev => removeEventDefinition(ev.getData().rec().getString(EF_EVENT_REMOVED_NAME))));

            addEventListener(E_INFO_CHANGED,
                             new DefaultContextEventListener<CallerController<CallerData>>(
                                 ev => initInfo(ev.getData())));

            localInitComplete = true;
        }

        private void addLocalFunctionDefinitions()
        {
            addFunctionDefinition(new FunctionDefinition(F_LOCAL_REINITIALIZE, TableFormat.EMPTY_FORMAT,
                                                         TableFormat.EMPTY_FORMAT, "Reinitialize Context"));
        }

        protected override TableFormat decodeFormat(String source, CallerController<CallerData> caller)
        {

            if (source == null)
            {
                return null;
            }

            var idSourceBuilder = new StringBuilder();

            int i;

            for (i = 0; i < source.Length; i++)
            {
                var c = source[i];
                if (Char.IsDigit(c))
                {
                    idSourceBuilder.Append(c);
                }
                else
                {
                    break;
                }
            }

            source = source.Substring(i);

            var idSource = idSourceBuilder.ToString();

            var formatIdNullable = idSource.Length > 0 ? int.Parse(idSource) : (int?)null;

            var format = source.Length > 0 ? new TableFormat(source, new ClassicEncodingSettings(false)) : null;

            if (formatIdNullable == null)
            {
                return format;
            }

            var formatId = (int)formatIdNullable;
            if (format == null)
            {
                TableFormat cached = controller.getFormatCache().get(formatId);

                if (cached == null)
                {
                    throw new ArgumentException("Unknown format ID: " + formatId);
                }

                return cached;
            }

            controller.getFormatCache().put(formatId, format);
            return format;

        }

        protected override sealed void clear()
        {
            try
            {
                accept(new DelegatedContextVisitor<Context>(context =>
                    {
                        initialized = false;
                        initializing = false;
                    }));
            }
            catch (ContextException ex)
            {
                throw new ContextRuntimeException(ex);
            }
        }

        private void init()
        {
            try
            {
                if (controller.getContextManager() != null)
                {
                    controller.getContextManager().initialize();
                }

                lock (this)
                {
                    if (!localInitComplete || initializing)
                    {
                        return;
                    }

                    try
                    {
                        initializing = true;

                        if (initialized)
                        {
                            return;
                        }

                        initInfo(getRemoteVariable(INFO_DEFINITION_FORMAT, V_INFO));
                        initChildren(getRemoteVariable(VFT_CHILDREN, V_CHILDREN));
                        initVariables(getRemoteVariable(VARIABLE_DEFINITION_FORMAT, V_VARIABLES));
                        initFunctions(getRemoteVariable(FUNCTION_DEFINITION_FORMAT, V_FUNCTIONS));
                        initEvents(getRemoteVariable(EVENT_DEFINITION_FORMAT, V_EVENTS));

                        initialized = true;
                    }
                    finally
                    {
                        initializing = false;
                    }
                }
            }
            catch (ContextException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ContextException(ex);
            }
        }

        private void initInfo(DataTable info)
        {
            setDescription(info.rec().getString(VF_INFO_DESCRIPTION));
            setType(info.rec().getString(VF_INFO_TYPE));

            if (info.getFormat().hasField(VF_INFO_GROUP))
            {
                setGroup(info.rec().getString(VF_INFO_GROUP));
            }

            if (info.getFormat().hasField(VF_INFO_ICON))
            {
                setIconId(info.rec().getString(VF_INFO_ICON));
            }
        }

        private void initChildren(DataTable children)
        {
            foreach (var child in getChildren())
            {
                if (children.select(VF_CHILDREN_NAME, child.getName()) == null)
                {
                    removeChild(child);
                }
            }

            foreach (var rec in children)
            {
                var cn = rec.getString(VF_CHILDREN_NAME);
                if (getChild(cn) == null)
                {
                    addChild(createNewProxyContext(cn));
                }
            }
        }

        private RemoteDeviceContextProxy<TDevice> createNewProxyContext(String name)
        {
            var proxy = new RemoteDeviceContextProxy<TDevice>(name, controller);
            proxy.setNotManageRemoteListeners(isNotManageRemoteListeners());
            return proxy;
        }

        private void initVariables(DataTable variables)
        {
            foreach (var def in getVariableDefinitions())
            {
                if (variables.select(FIELD_VD_NAME, def.getName()) == null)
                {
                    removeVariableDefinition(def.getName());
                }
            }

            foreach (var rec in variables)
            {
                var def = varDefFromDataRecord(rec);
                var existing = getVariableDefinition(def.getName());
                if (existing == null || !existing.equals(def))
                {
                    if (existing != null)
                    {
                        removeVariableDefinition(existing.getName());
                    }
                    addVariableDefinition(def);
                }
            }
        }

        private void initFunctions(DataTable functions)
        {
            try
            {
                foreach (var def in getFunctionDefinitions())
                {
                    if (functions.select(FIELD_FD_NAME, def.getName()) == null)
                    {
                        removeFunctionDefinition(def.getName());
                    }
                }


                addLocalFunctionDefinitions();

                foreach (var rec in functions)
                {
                    var def = funcDefFromDataRecord(rec);
                    var existing = getFunctionDefinition(def.getName());
                    if (existing != null && existing.Equals(def)) continue;
                    if (existing != null)
                    {
                        removeFunctionDefinition(existing.getName());
                    }
                    addFunctionDefinition(def);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private void initEvents(DataTable events)
        {
            foreach (var ed in getEventDefinitions())
            {
                if (events.select(FIELD_ED_NAME, ed.getName()) == null)
                {
                    removeEventDefinition(ed.getName());
                }
            }

            foreach (var rec in events)
            {
                var def = evtDefFromDataRecord(rec);
                var existing = getEventDefinition(def.getName());
                if (existing != null && existing.Equals(def)) continue;
                if (existing != null)
                {
                    removeEventDefinition(existing.getName());
                }
                addEventDefinition(def);
            }
        }

        public override String getDescription()
        {
            try
            {
                init();
            }
            catch (ContextException ex)
            {
                Log.CONTEXT_VARIABLES.warn("Error getting description of remote context", ex);
            }
            return base.getDescription();
        }

        public override String getType()
        {
            try
            {
                init();
            }
            catch (ContextException ex)
            {
                throw new ContextRuntimeException("Error getting type of remote context: " + ex.Message, ex);
            }
            return base.getType();
        }

        public override String getIconId()
        {
            try
            {
                init();
            }
            catch (ContextException ex)
            {
                Log.CONTEXT_VARIABLES.warn("Error getting icon of remote context", ex);
            }
            return base.getIconId();
        }

        public override Context getChild(String name, CallerController<CallerData> callerController)
        {
            if (base.getChild(name, callerController) == null)
            {
                try
                {
                    init();
                }
                catch (ContextException ex)
                {
                    Log.CONTEXT_CHILDREN.warn("Error initializing children of remote context", ex);
                }
            }
            return base.getChild(name, callerController);
        }

        public override VariableDefinition getVariableDefinition(String name)
        {
            if (base.getVariableDefinition(name) == null && isSetupComplete())
            {
                initVariablesLoggingErrors();
            }
            return base.getVariableDefinition(name);
        }

        public override FunctionDefinition getFunctionDefinition(String name)
        {
            if (base.getFunctionDefinition(name) == null && isSetupComplete())
            {
                initFunctionsLoggingErrors();
            }
            return base.getFunctionDefinition(name);
        }

        public override EventData getEventData(String name)
        {
            if (base.getEventData(name) == null && isSetupComplete())
            {
                initEventsLoggingErrors();
            }
            return base.getEventData(name);
        }

        public override List<VariableDefinition> getVariableDefinitions(CallerController<CallerData> caller)
        {
            initVariablesLoggingErrors();
            return base.getVariableDefinitions(caller);
        }

        public override List<FunctionDefinition> getFunctionDefinitions(CallerController<CallerData> caller)
        {
            initFunctionsLoggingErrors();
            return base.getFunctionDefinitions(caller);
        }

        public override List<EventDefinition> getEventDefinitions(CallerController<CallerData> caller)
        {
            initEventsLoggingErrors();
            return base.getEventDefinitions(caller);
        }

        private void initVariablesLoggingErrors()
        {
            try
            {
                init();
            }
            catch (ContextSecurityException ex)
            {
                Log.CONTEXT_VARIABLES.warn("Security exception while initializing variables of remote context: " + ex.Message);
            }
            catch (ContextException ex)
            {
                Log.CONTEXT_VARIABLES.warn("Error initializing variables of remote context '" + this.getPath() + "': " + ex.Message, ex);
            }
        }

        private void initFunctionsLoggingErrors()
        {
            try
            {
                init();
            }
            catch (ContextSecurityException ex)
            {
                Log.CONTEXT_FUNCTIONS.warn("Security exception while initializing functions of remote context: " + ex.Message);
            }
            catch (ContextException ex)
            {
                Log.CONTEXT_FUNCTIONS.warn("Error initializing functions of remote context '" + this.getPath() + "': " + ex.Message, ex);
            }
        }

        private void initEventsLoggingErrors()
        {
            try
            {
                init();
            }
            catch (ContextSecurityException ex)
            {
                Log.CONTEXT_EVENTS.warn("Security exception while initializing events of remote context: " + ex.Message);
            }
            catch (ContextException ex)
            {
                Log.CONTEXT_EVENTS.warn("Error initializing events of remote context '" + this.getPath() + "': " + ex.Message, ex);
            }
        }

        private IncomingAggreGateCommand sendGetVariable(String name)
        {
            return this.controller.sendCommandAndCheckReplyCode(ClientCommandUtils.getVariableOperation(this.getPath(), name));
        }

        public override DataTable getRemoteVariable(TableFormat format, String name)
        {
            var encodedReply = sendGetVariable(name).getEncodedDataTableFromReply();
            try
            {
                return decodeRemoteDataTable(format, encodedReply);
            }
            catch (Exception ex)
            {
                throw new ContextException("Error parsing encoded data table: " + encodedReply, ex);
            }
        }

        public override DataTable decodeRemoteDataTable(TableFormat format, String encodedReply)
        {
            if (controller.isAvoidSendingFormats())
            {
                if (format != null)
                {
                    return new DataTable(format, encodedReply);
                }
            }

            return new DataTable(encodedReply, controller.createClassicEncodingSettings(false), false);
        }

        protected override void setupVariables()
        {
            init();
            base.setupVariables();
        }

        protected override DataTable getVariableImpl(VariableDefinition def, CallerController<CallerData> caller,
                                                     RequestController<RequestData> request)
        {
            try
            {
                IncomingAggreGateCommand ans = sendGetVariable(def.getName());
                return decodeRemoteDataTable(def.getFormat(), ans.getEncodedDataTableFromReply());
            }
            catch (Exception ex)
            {
                Log.CONTEXT_VARIABLES.debug("Error getting variable '" + def.getName() + "' from context '" + this.getPath() + "'", ex);
                throw new ContextException(ex.Message, ex);
            }
        }

        protected override Boolean setVariableImpl(VariableDefinition def, CallerController<CallerData> caller,
                                                   RequestController<RequestData> request, DataTable value)
        {
            try
            {
                controller.sendCommandAndCheckReplyCode(ClientCommandUtils.setVariableOperation(this.getPath(), def.getName(), value.encode(controller.createClassicEncodingSettings(true))));
                return true;
            }
            catch (Exception ex)
            {
                Log.CONTEXT_VARIABLES.debug("Error setting veriable '" + def.getName() + "' of context '" + this.getPath() + "'", ex);
                throw new ContextException(ex.Message, ex);
            }
        }

        protected override void setupFunctions()
        {
            init();
            base.setupFunctions();
        }

        protected override DataTable callFunctionImpl(FunctionDefinition def, CallerController<CallerData> caller,
                                                      RequestController<RequestData> request, DataTable parameters)
        {
            if (def.getName().Equals(F_LOCAL_REINITIALIZE))
            {
                reinitialize();
                return new DataTable(def.getOutputFormat(), true);
            }

            return callRemoteFunction(def.getName(), def.getOutputFormat(), parameters);
        }

        public override DataTable callRemoteFunction(String name, TableFormat outputFormat, DataTable parameters)
        {
            try
            {
                var ans =
                    controller.sendCommandAndCheckReplyCode(ClientCommandUtils.callFunctionOperation(this.getPath(), name, parameters.encode(controller.createClassicEncodingSettings(true))));
                return decodeRemoteDataTable(outputFormat, ans.getEncodedDataTableFromReply());
            }
            catch (Exception ex)
            {
                Log.CONTEXT_FUNCTIONS.debug("Error calling function '" + name + "' of context '" + this.getPath() + "'", ex);
                throw new ContextException(ex.Message, ex);
            }
        }

        public override Boolean addEventListener(String name, ContextEventListener contextEventListener)
        {
            return addEventListener(name, contextEventListener, true);
        }

        public Boolean addEventListener(String name, ContextEventListener contextEventListener,
                                        Boolean sendRemoteCommand)
        {
            try
            {
                init();

                var ed = getEventData(name);

                if (ed == null)
                {
                    throw new ContextException(Cres.get().getString("conEventNotFound") + name);
                }

                if (sendRemoteCommand)
                {
                    addRemoteListener(ed.getDefinition().getName(), contextEventListener);
                }

                return base.addEventListener(name, contextEventListener);
            }
            catch (Exception ex)
            {
                var msg = String.Format(Cres.get().getString("conErrAddingListener"), name, this.getPath());
                throw new InvalidOperationException(msg + ": " + ex.Message, ex);
            }
        }

        public override Boolean removeEventListener(String name, ContextEventListener contextEventListener)
        {
            return removeEventListener(name, contextEventListener, true);
        }

        public Boolean removeEventListener(String name, ContextEventListener contextEventListener,
                                           Boolean sendRemoteCommand)
        {
            try
            {
                init();

                Log.CONTEXT_EVENTS.debug("Removing listener for event '" + name + "' from context '" + this.getPath() + "'");

                var res = base.removeEventListener(name, contextEventListener);

                var ed = getEventData(name);

                if (sendRemoteCommand && ed != null && ed.getListeners().Count == 0)
                {
                    var hashCode = contextEventListener.getListenerCode();

                    if (!notManageRemoteListenersBoolean)
                    {
                        this.controller.sendCommandAndCheckReplyCode(ClientCommandUtils.removeEventListenerOperation(this.getPath(), name, hashCode));
                    }
                }

                return res;
            }
            catch (Exception ex)
            {
                var msg = String.Format(Cres.get().getString("conErrRemovingListener"), name, this.getPath());
                throw new InvalidOperationException(msg + ": " + ex.Message, ex);
            }
        }

        private void addRemoteListener(String ename, ContextEventListener contextEventListener)
        {
            var hashCode = contextEventListener.getListenerCode();

            if (hashCode == null && AUTO_LISTENED_EVENTS.Contains(ename))
            {
                return;
            }

            if (!notManageRemoteListenersBoolean)
            {
                controller.sendCommandAndCheckReplyCode(ClientCommandUtils.addEventListenerOperation(this.getPath(), ename, hashCode));
            }
        }

        public override List<Context> getChildren(CallerController<CallerData> caller)
        {
            try
            {
                init();
            }
            catch (ContextException ex)
            {
                Log.CONTEXT_CHILDREN.warn("Error initializing children of remote context", ex);
            }
            return base.getChildren(caller);
        }

        private void restoreEventListeners()
        {
            foreach (var ed in base.getEventDefinitions((CallerController<CallerData>)null))
            // Calling method of superclass directly to avoid fetching remote events info
            {
                EventData edata = getEventData(ed.getName());
                lock (edata.getListeners())
                {
                    foreach (ContextEventListener listener in edata.getListeners())
                    {
                        try
                        {
                            addRemoteListener(ed.getName(), listener);
                        }
                        catch (Exception ex)
                        {
                            Log.CONTEXT_EVENTS.warn("Error restoring listener for event '" + ed.getName() + "'", ex);
                        }
                    }
                }
            }
        }

        public override void reinitialize()
        {
            clear();
            restoreEventListeners();
        }

        public Boolean isNotManageRemoteListeners()
        {
            return notManageRemoteListenersBoolean;
        }

        public void setNotManageRemoteListeners(Boolean notManageRemoteListeners)
        {
            notManageRemoteListenersBoolean = notManageRemoteListeners;
        }
    }
}