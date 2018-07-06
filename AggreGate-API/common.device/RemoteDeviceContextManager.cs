using System;
using com.tibbo.aggregate.common.command;
using com.tibbo.aggregate.common.context;

namespace com.tibbo.aggregate.common.device
{
    public class RemoteDeviceContextManager<TDevice> : DefaultContextManager
        where TDevice : AggreGateDevice
    {
        private Boolean initialized;
        private Boolean initializing;

        private readonly AggreGateDeviceController controller;

        public RemoteDeviceContextManager(AggreGateDeviceController controller, Boolean async) : base(async)
        {
            this.controller = controller;
        }

        public void initialize()
        {
            if (initialized || initializing)
            {
                return;
            }

            initializing = true;

            try
            {
                controller.connect();
            }
            finally
            {
                initializing = false;
            }

            initialized = true;
        }

        public override void stop()
        {
            initialized = false;

            base.stop();
        }

        public AggreGateDeviceController getController()
        {
            return controller;
        }

        private void sendAddListener(String context, String eventString, ContextEventListener listener)
        {
            try
            {
                getController().sendCommandAndCheckReplyCode(ClientCommandUtils.addEventListenerOperation(context,
                                                                                                          eventString,
                                                                                                          listener.
                                                                                                              getListenerCode
                                                                                                              ()));
            }
            catch (Exception ex)
            {
                var msg = String.Format(Cres.get().getString("conErrAddingListener"), eventString, context);
                throw new InvalidOperationException(msg + ": " + ex.Message, ex);
            }
        }

        private void sendRemoveListener(String context, String eventString, ContextEventListener listener)
        {
            try
            {
                getController().sendCommandAndCheckReplyCode(ClientCommandUtils.removeEventListenerOperation(context,
                                                                                                             eventString,
                                                                                                             listener.
                                                                                                                 getListenerCode
                                                                                                                 ()));
            }
            catch (Exception ex)
            {
                var msg = String.Format(Cres.get().getString("conErrRemovingListener"), eventString, context);
                throw new InvalidOperationException(msg + ": " + ex.Message, ex);
            }
        }

        protected void addListenerToContext(RemoteDeviceContextProxy<TDevice> con, String eventString,
                                            ContextEventListener listener, Boolean mask)
        {
            con.addEventListener(eventString, listener, !mask); // Don't sent remote command if adding as mask listener
        }

        protected void removeListenerFromContext(RemoteDeviceContextProxy<TDevice> con, String eventString,
                                                 ContextEventListener listener, Boolean mask)
        {
            con.removeEventListener(eventString, listener, !mask);
            // Don't send remote command if adding as mask listener
        }

        public override void addMaskEventListener(String mask, String eventString, ContextEventListener listener)
        {
            base.addMaskEventListener(mask, eventString, listener);

            sendAddListener(mask, eventString, listener);
        }

        public override void removeMaskEventListener(String mask, String eventString, ContextEventListener listener)
        {
            base.removeMaskEventListener(mask, eventString, listener);

            sendRemoveListener(mask, eventString, listener);
        }

        public override void contextRemoved(Context con)
        {
            // We don't store listeners from removed contexts because server do that itself
        }
    }
}