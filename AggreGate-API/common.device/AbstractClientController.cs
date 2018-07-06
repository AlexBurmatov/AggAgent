using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using com.tibbo.aggregate.common.command;
using com.tibbo.aggregate.common.context;
using com.tibbo.aggregate.common.data;
using com.tibbo.aggregate.common.datatable;
using com.tibbo.aggregate.common.@event;
using com.tibbo.aggregate.common.server;
using com.tibbo.aggregate.common.util;
using Collections;
using JavaCompatibility;
using DataTable = com.tibbo.aggregate.common.datatable.DataTable;

namespace com.tibbo.aggregate.common.device
{
    public abstract class AbstractClientController
    {
        public int commandsInProgress { get; set; }

        private const int THREAD_SHUTDOWN_TIMEOUT = 10;

        private readonly CommandBuffer commandBuffer;
        private StreamWrapper streamWrapper;

        private Boolean startMessageReceived;
        private Boolean shutDown;

        private CallerController<CallerData> callerController;
        private readonly ContextManager contextManager;

        private readonly ContextEventListener defaultEventListener;

        private readonly Dictionary<EntityReference, ContextEventListener> eventListeners =
            new Dictionary<EntityReference, ContextEventListener>();

        private readonly ThreadPoolExectutor commandExecutors = new ThreadPoolExectutor();

        protected AbstractClientController(StreamWrapper wrapper, ContextManager contextManager)
        {
            this.contextManager = contextManager;

            this.streamWrapper = wrapper; //Stream.Synchronized(aStream);

            commandBuffer = new CommandBuffer(wrapper, Command.START_CHAR, Command.END_CHAR);

            defaultEventListener = new ForwardingEventListener(this, null);
        }

        protected virtual ClassicEncodingSettings createClassicEncodingSettings(Boolean useFormatCache)
        {
            return new ClassicEncodingSettings(false);
        }

        public void processOperationGetVar(String id, Context con, String name, OutgoingAggreGateCommand ans)
        {
            var vd = con.getVariableDefinition(name);

            if (vd == null)
            {
                ans.constructReply(id, AggreGateCommand.REPLY_CODE_DENIED, Cres.get().getString("conVarNotAvail") + name);
                return;
            }

            var result = con.getVariable(name, callerController);

            ans.constructReply(id, AggreGateCommand.REPLY_CODE_OK);


            ans.addParam(result.encode(createClassicEncodingSettings(vd.getFormat() != null)));
        }

        public void processOperationSetVar(String id, Context con, String name, String encodedValue,
                                           OutgoingAggreGateCommand ans)
        {
            var vd = con.getVariableDefinition(name);

            if (vd == null)
            {
                ans.constructReply(id, AggreGateCommand.REPLY_CODE_DENIED, Cres.get().getString("conVarNotAvail") + name);
                return;
            }

            var settings = new ClassicEncodingSettings(false, vd.getFormat());

            var value = new DataTable(encodedValue, settings, true);

            con.setVariable(name, callerController, value);

            ans.constructReply(id, AggreGateCommand.REPLY_CODE_OK);
        }

        public void processOperationCallFunction(String id, Context con, String name, String encodedParameters,
                                                 OutgoingAggreGateCommand ans)
        {
            Logger.getLogger(Log.CLIENTS).debug("Calling function '" + name + "' of context '" + con.getPath() + "'");

            var fd = con.getFunctionDefinition(name);

            if (fd == null)
            {
                ans.constructReply(id, AggreGateCommand.REPLY_CODE_DENIED, Cres.get().getString("conFuncNotAvail") + name);
                return;
            }

            var settings = new ClassicEncodingSettings(false, fd.getInputFormat());

            var parameters = new DataTable(encodedParameters, settings, true);

            var result = con.callFunction(name, callerController, parameters);

            ans.constructReply(id, AggreGateCommand.REPLY_CODE_OK);
            ans.addParam(result.encode(createClassicEncodingSettings(fd.getOutputFormat() != null)));
        }

        private void addMaskListener(String context, String name, ContextEventListener cel)
        {
            contextManager.addMaskEventListener(context, name, cel);
            eventListeners.Add(new EntityReference(context, name), cel);
        }

        protected Boolean addNormalListener(String context, String name, ContextEventListener cel)
        {
            var con = contextManager.get(context, callerController);
            return con != null && con.addEventListener(name, cel);
        }

        private void removeMaskListener(String context, String name, ContextEventListener cel)
        {
            contextManager.removeMaskEventListener(context, name, cel);
            eventListeners.Remove(new EntityReference(context, name));
        }

        public void processOperationAddEventListener(String id, String context, String name, Int32? listener,
                                                     OutgoingAggreGateCommand ans)
        {
            Logger.getLogger(Log.CLIENTS).debug("Adding listener for event '" + name + "' of context '" + context + "'");

            var cel = createListener(listener);

            addMaskListener(context, name, cel);
            ans.constructReply(id, AggreGateCommand.REPLY_CODE_OK);
        }

        public void processOperationRemoveEventListener(String id, String context, String name, Int32? listener,
                                                        OutgoingAggreGateCommand ans)
        {
            Logger.getLogger(Log.CLIENTS).debug("Removing listener for event '" + name + "' of context '" + context +
                                                "'");
            var cel = createListener(listener);
            removeMaskListener(context, name, cel);
            ans.constructReply(id, AggreGateCommand.REPLY_CODE_OK);
        }

        public void processMessageStart(IncomingAggreGateCommand cmd, OutgoingAggreGateCommand ans)
        {
            var version = Int32.Parse(cmd.getParameter(AggreGateCommand.INDEX_PROTOCOL_VERSION));

            Logger.getLogger(Log.CLIENTS).debug("Processing start command, client protocol version: " + version);

            if (version == ClientCommandUtils.CLIENT_PROTOCOL_VERSION)
            {
                ans.constructReply(cmd.getId(), AggreGateCommand.REPLY_CODE_OK);
                startMessageReceived = true;
            }
            else
            {
                ans.constructReply(cmd.getId(), AggreGateCommand.REPLY_CODE_DENIED);
            }
        }

        protected virtual void processMessageOperation(IncomingAggreGateCommand cmd, OutgoingAggreGateCommand ans)
        {
            var operation = cmd.getParameter(AggreGateCommand.INDEX_OPERATION_CODE);
            var context = cmd.getParameter(AggreGateCommand.INDEX_OPERATION_CONTEXT);
            var target = cmd.getParameter(AggreGateCommand.INDEX_OPERATION_TARGET);

            if (operation.Length > 1)
            {
                throw new SyntaxErrorException(Cres.get().getString("clInvalidOpcode") + operation);
            }

            Logger.getLogger(Log.CLIENTS).debug("Processing message, context '" + context + "', target '" + target +
                                                "', operation '" + operation + "'");

            switch (operation[0])
            {
                case AggreGateCommand.COMMAND_OPERATION_ADD_EVENT_LISTENER:
                    var listenerStr = cmd.getParameter(AggreGateCommand.INDEX_OPERATION_LISTENER);
                    var listener = listenerStr.Length > 0 ? (Int32?)Int32.Parse(listenerStr) : null;
                    processOperationAddEventListener(cmd.getId(), context, target, listener, ans);
                    return;

                case AggreGateCommand.COMMAND_OPERATION_REMOVE_EVENT_LISTENER:
                    listenerStr = cmd.getParameter(AggreGateCommand.INDEX_OPERATION_LISTENER);
                    listener = listenerStr.Length > 0 ? (Int32?)Int32.Parse(listenerStr) : null;
                    processOperationRemoveEventListener(cmd.getId(), context, target, listener, ans);
                    return;
            }

            var con = contextManager.get(context, callerController);
            if (con == null)
            {
                throw new ContextException(Cres.get().getString("conNotAvail") + context);
            }

            if (addNormalListener(con.getPath(), AbstractContext.E_DESTROYED, defaultEventListener))
            {
                addNormalListener(con.getPath(), AbstractContext.E_CHILD_ADDED, defaultEventListener);
                addNormalListener(con.getPath(), AbstractContext.E_CHILD_REMOVED, defaultEventListener);
                addNormalListener(con.getPath(), AbstractContext.E_VARIABLE_ADDED, defaultEventListener);
                addNormalListener(con.getPath(), AbstractContext.E_VARIABLE_REMOVED, defaultEventListener);
                addNormalListener(con.getPath(), AbstractContext.E_FUNCTION_ADDED, defaultEventListener);
                addNormalListener(con.getPath(), AbstractContext.E_FUNCTION_REMOVED, defaultEventListener);
                addNormalListener(con.getPath(), AbstractContext.E_EVENT_ADDED, defaultEventListener);
                addNormalListener(con.getPath(), AbstractContext.E_EVENT_REMOVED, defaultEventListener);
                addNormalListener(con.getPath(), AbstractContext.E_INFO_CHANGED, defaultEventListener);

                addCustomListeners(con);
            }

            switch (operation[0])
            {
                case AggreGateCommand.COMMAND_OPERATION_GET_VAR:
                    processOperationGetVar(cmd.getId(), con, target, ans);
                    break;

                case AggreGateCommand.COMMAND_OPERATION_SET_VAR:
                    processOperationSetVar(cmd.getId(), con, target, cmd.getEncodedDataTableFromOperationMessage(), ans);
                    break;

                case AggreGateCommand.COMMAND_OPERATION_CALL_FUNCTION:
                    processOperationCallFunction(cmd.getId(), con, target, cmd.getEncodedDataTableFromOperationMessage(),
                                                 ans);
                    break;

                default:
                    throw new SyntaxErrorException(Cres.get().getString("clUnknownOpcode") + operation[0]);
            }
        }

        protected void addCustomListeners(Context con)
        {
        }

        protected void processMessage(IncomingAggreGateCommand cmd, OutgoingAggreGateCommand ans)
        {
            var messageCode = cmd.getMessageCode();

            if (messageCode.Length > 1)
            {
                throw new SyntaxErrorException(Cres.get().getString("clInvalidMsgCode") + messageCode);
            }

            var code = messageCode[0];

            if ((code != AggreGateCommand.MESSAGE_CODE_START) && (!startMessageReceived))
            {
                Logger.getLogger(Log.CLIENTS).debug("Can't process message: start message was not received yet");
                ans.constructReply(cmd.getId(), AggreGateCommand.REPLY_CODE_DENIED);
                return;
            }

            switch (code)
            {
                case AggreGateCommand.MESSAGE_CODE_START:
                    processMessageStart(cmd, ans);
                    break;

                case AggreGateCommand.MESSAGE_CODE_OPERATION:
                    processMessageOperation(cmd, ans);
                    break;

                default:
                    throw new SyntaxErrorException(Cres.get().getString("clUnknownMsgCode") + messageCode[0]);
            }
        }

        public OutgoingAggreGateCommand processCommand(IncomingAggreGateCommand cmd)
        {
            var ans = new OutgoingAggreGateCommand();

            try
            {
                var commandCode = cmd.getParameter(AggreGateCommand.INDEX_COMMAND_CODE);

                if (commandCode.Length > 1)
                {
                    throw new AggreGateException(Cres.get().getString("clInvalidCmdCode") + commandCode);
                }

                switch (commandCode[0])
                {
                    case AggreGateCommand.COMMAND_CODE_MESSAGE:
                        processMessage(cmd, ans);
                        break;

                    default:
                        throw new AggreGateException(Cres.get().getString("clUnknownCmdCode") + commandCode[0]);
                }
            }
            catch (ContextSecurityException ex)
            {
                Logger.getLogger(Log.CLIENTS).info("Access denied while processing command '" + cmd + "': ", ex);
                ans.constructReply(cmd.getId(), AggreGateCommand.REPLY_CODE_DENIED);
            }
            catch (Exception ex)
            {
                Logger.getLogger(Log.CLIENTS).info("Error processing command '" + cmd + "': ", ex);
                ans.constructReply(cmd.getId(), AggreGateCommand.REPLY_CODE_ERROR, ex.Message ?? ex.ToString(),
                                   getErrorDetails(ex));
            }
            return ans;
        }

        public void shutdown()
        {
            try
            {
                Logger.getLogger(Log.CLIENTS).debug("Shutting down client controller");

                shutDown = true;

                if (!commandExecutors.awaitTermination(THREAD_SHUTDOWN_TIMEOUT))
                {
                    Logger.getLogger(Log.CLIENTS).debug("Unable to shutdown all command executors");
                }

                if (streamWrapper != null)
                {
                    streamWrapper.Close();
                }

                foreach (var entry in eventListeners)
                {
                    var listener = entry.Value;
                    if (listener != null)
                    {
                        contextManager.removeMaskEventListener(entry.Key.getContext(), entry.Key.getEntity(), listener);
                    }
                }

                if (contextManager.getRoot().getFunctionDefinition(RootContextConstants.F_LOGOUT) != null)
                {
                    contextManager.getRoot().callFunction(RootContextConstants.F_LOGOUT, callerController);
                }
            }
            catch (Exception ex)
            {
                Logger.getLogger(Log.CLIENTS).info("Error shutting down client controller: ", ex);
            }
        }

        public void run()
        {
            try
            {
                runImpl();
            }
            catch (IOException ex)
            {
                const string msg = "I/O error while communicating with client";
                Logger.getLogger(Log.CLIENTS).debug(msg, ex);
                Logger.getLogger(Log.CLIENTS).info(msg + ": " + ex);
                Thread.CurrentThread.Interrupt();
            }
            catch (DisconnectionException ex)
            {
                Logger.getLogger(Log.CLIENTS).info("Client disconnected: " + ex.Message);
                Thread.CurrentThread.Interrupt();
            }
            catch (Exception ex)
            {
                Logger.getLogger(Log.CLIENTS).warn("Error in client controller", ex);
                Thread.CurrentThread.Interrupt();
            }
        }

        public void runImpl()
        {
            commandBuffer.readCommand();
            if (!commandBuffer.isFull()) return;
            var command = new IncomingAggreGateCommand(commandBuffer);

            commandsInProgress++;

            Logger.getLogger(Log.COMMANDS_CLIENT).debug("Received: " + command);

            commandExecutors.submit(new ProcessCommandTask(this, command));
        }

        private static String getErrorDetails(Exception error)
        {
            var buf = new StringBuilder();
            buf.Append(Cres.get().getString("clServerTime") + ": " + new DateTime() + "\n");
            buf.Append(error.StackTrace);
            return buf.ToString();
        }

        public void sendCommand(OutgoingAggreGateCommand cmd)
        {
            cmd.send(streamWrapper);
        }

        public ContextEventListener createListener(Int32? listenerHashCode)
        {
            return listenerHashCode == null ? defaultEventListener : new ForwardingEventListener(this, listenerHashCode);
        }

        public OutgoingAggreGateCommand constructEventCommand(Event anEvent, int? listenerCode)
        {
            var cmd = new OutgoingAggreGateCommand();

            string data = anEvent.getData().encode(this.createClassicEncodingSettings(true));

            cmd.constructEvent(anEvent.getContext(), anEvent.getName(), anEvent.getLevel(), data, anEvent.getId(), anEvent.getCreationtime(), listenerCode);

            cmd.setAsync(true);

            return cmd;
        }

        public abstract Boolean controllerShouldHandle(Event ev, ContextEventListener listener);

        public class ForwardingEventListener : DefaultContextEventListener<CallerController<CallerData>>
        {
            private readonly AbstractClientController owner;

            public ForwardingEventListener(AbstractClientController ownerController, Int32? listenerCode)
                : base(null, listenerCode)
            {
                owner = ownerController;
            }

            public override Boolean shouldHandle(Event anEvent)
            {
                try
                {
                    if (!base.shouldHandle(anEvent))
                    {
                        return false;
                    }

                    if (!owner.controllerShouldHandle(anEvent, this))
                    {
                        return false;
                    }

                    if (anEvent.getName().Equals(AbstractContext.E_CHILD_ADDED))
                    {
                        var con = owner.getContextManager().get(anEvent.getContext(), callerController);
                        if (con == null ||
                            con.getChild(anEvent.getData().rec().getString(AbstractContext.EF_CHILD_ADDED_CHILD),
                                         getCallerController()) == null)
                        {
                            return false;
                        }
                    }

                    if (anEvent.getName().Equals(LinkServerContextConstants.E_VISIBLE_CHILD_ADDED))
                    {
                        if (
                            owner.contextManager.get(
                                anEvent.getData().rec().getString(LinkServerContextConstants.EF_VISIBLE_CHILD_ADDED_PATH),
                                callerController) == null)
                        {
                            return false;
                        }
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    throw new EventHandlingException(ex.Message, ex);
                }
            }

            private static OutgoingAggreGateCommand cmd = null;

            private static long LAST_TICKS = 0;

            public override void handle(Event anEvent)
            {
                if (owner.shutDown)
                {
                    throw new ObsoleteListenerException("Client constoller was stopped");
                }

                try
                {
                    OutgoingAggreGateCommand cmd = this.owner.constructEventCommand(anEvent, this.getListenerCode());

                    owner.sendCommand(cmd);
            }
                catch (SocketException ex)
                {
                    owner.shutdown();
                    const string msg = "Socket exception while forwarding event to client: ";
                    Logger.getLogger(Log.CLIENTS).info(msg + ex);
                    Logger.getLogger(Log.CLIENTS).debug(msg + ex.Message, ex);
                }
                catch (Exception ex)
                {
                    throw new EventHandlingException(
                        String.Format(Cres.get().getString("clErrorHandlingEvent"), anEvent.getName()) + ex.Message, ex);
                }
            }

            public override String ToString()
            {
                return "ForwardingEventListener: " + getListenerCode();
            }

            public override int GetHashCode()
            {
                const int prime = 31;
                var result = 1;
                result = prime * result + getOwningController().GetHashCode();
                result = prime * result + ((getListenerCode() == null) ? 0 : getListenerCode().GetHashCode());
                return result;
            }

            public override Boolean Equals(Object obj)
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

                var other = (ForwardingEventListener)obj;
                if (getOwningController() != other.getOwningController())
                {
                    return false;
                }

                if (getListenerCode() == null)
                {
                    if (other.getListenerCode() != null)
                    {
                        return false;
                    }
                }
                else if (other.getListenerCode() == null)
                {
                    return false;
                }
                return (getListenerCode() ?? 0) == (other.getListenerCode() ?? 0);
            }

            private AbstractClientController getOwningController()
            {
                return owner;
            }
        }

        public class ProcessCommandTask
        {
            protected readonly AbstractClientController owner;

            private readonly IncomingAggreGateCommand cmd;
            public Boolean isFinished;
            private Thread thread;

            public ProcessCommandTask(AbstractClientController ownerController, IncomingAggreGateCommand aCommand)
            {
                owner = ownerController;
                cmd = aCommand;
            }

            public virtual void call(Object stateInfo)
            {
                isFinished = false;
                thread = Thread.CurrentThread;

                try
                {
                    if (!owner.shutDown)
                    {
                        owner.sendCommand(owner.processCommand(cmd));
                    }
                }
                catch (SocketException ex)
                {
                    Logger.getLogger(Log.CLIENTS).info("Socket exception while processing command: " + ex.Message, ex);
                }
                catch (Exception ex)
                {
                    Logger.getLogger(Log.CLIENTS).warn("Error processing command: " + ex.Message, ex);
                }
                finally
                {
                    owner.commandsInProgress--;
                    isFinished = true;
                }
            }

            public void interrupt()
            {
                if (thread == null)
                    return;

                thread.Interrupt();
            }
        }

        protected void setCallerController(CallerController<CallerData> aCallerController)
        {
            callerController = aCallerController;
        }

        public CallerController<CallerData> getCallerController()
        {
            return callerController;
        }

        public ContextManager getContextManager()
        {
            return contextManager;
        }

        public ContextEventListener getDefaultEventListener()
        {
            return defaultEventListener;
        }
    }
}