using System;

namespace com.tibbo.aggregate.common.command
{
    public class ClientCommandUtils
    {
        public const int CLIENT_PROTOCOL_VERSION = 2;

        public static OutgoingAggreGateCommand startMessage()
        {
            var cmd = new OutgoingAggreGateCommand();
            cmd.addParam(AggreGateCommand.COMMAND_CODE_MESSAGE.ToString());
            cmd.addParam(AggreGateCommand.generateId());
            cmd.addParam(AggreGateCommand.MESSAGE_CODE_START.ToString());
            cmd.addParam(CLIENT_PROTOCOL_VERSION.ToString());
            return cmd;
        }

        public static OutgoingAggreGateCommand operationMessage()
        {
            var cmd = new OutgoingAggreGateCommand();
            cmd.addParam(AggreGateCommand.COMMAND_CODE_MESSAGE.ToString());
            cmd.addParam(AggreGateCommand.generateId());
            cmd.addParam(AggreGateCommand.MESSAGE_CODE_OPERATION.ToString());
            return cmd;
        }

        public static OutgoingAggreGateCommand getVariableOperation(String context, String name)
        {
            var cmd = operationMessage();
            cmd.addParam(AggreGateCommand.COMMAND_OPERATION_GET_VAR.ToString());
            cmd.addParam(context);
            cmd.addParam(name);
            return cmd;
        }

        public static OutgoingAggreGateCommand setVariableOperation(String context, String name, String encodedValue)
        {
            var cmd = operationMessage();
            cmd.addParam(AggreGateCommand.COMMAND_OPERATION_SET_VAR.ToString());
            cmd.addParam(context);
            cmd.addParam(name);
            cmd.addParam(encodedValue);
            return cmd;
        }

        public static OutgoingAggreGateCommand callFunctionOperation(String context, String name,
                                                                     String encodedParameters)
        {
            var cmd = operationMessage();
            cmd.addParam(AggreGateCommand.COMMAND_OPERATION_CALL_FUNCTION.ToString());
            cmd.addParam(context);
            cmd.addParam(name);
            cmd.addParam(encodedParameters);
            return cmd;
        }

        public static OutgoingAggreGateCommand addEventListenerOperation(String context, String name,
                                                                         Int32? listenerHashCode)
        {
            var cmd = operationMessage();
            cmd.addParam(AggreGateCommand.COMMAND_OPERATION_ADD_EVENT_LISTENER.ToString());
            cmd.addParam(context);
            cmd.addParam(name);
            cmd.addParam(listenerHashCode != null ? listenerHashCode.ToString() : "");
            return cmd;
        }

        public static OutgoingAggreGateCommand removeEventListenerOperation(String context, String name,
                                                                            Int32? listenerHashCode)
        {
            var cmd = operationMessage();
            cmd.addParam(AggreGateCommand.COMMAND_OPERATION_REMOVE_EVENT_LISTENER.ToString());
            cmd.addParam(context);
            cmd.addParam(name);
            cmd.addParam(listenerHashCode != null ? listenerHashCode.ToString() : "");
            return cmd;
        }
    }
}