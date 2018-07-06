using System;
using System.Collections.Generic;
using System.Linq;
using com.tibbo.aggregate.common.data;
using com.tibbo.aggregate.common.@event;
using JavaCompatibility;

namespace com.tibbo.aggregate.common.context
{
    public class EventData : IComparable<EventData>
    {
        private readonly EventDefinition definition;
        private readonly HashSet<ContextEventListener> listeners = new HashSet<ContextEventListener>();

        public EventData(EventDefinition definition)
        {
            this.definition = definition;
        }

        public EventDefinition getDefinition()
        {
            return definition;
        }

        public HashSet<ContextEventListener> getListeners()
        {
            return listeners;
        }

        public Boolean addListener(ContextEventListener listener)
        {
            lock (listeners)
            {
                return listeners.Add(listener);
            }
        }

        public Boolean removeListener(ContextEventListener listener)
        {
            lock (listeners)
            {
                return listeners.Remove(listener);
            }
        }

        public void clearListeners()
        {
            lock (listeners)
            {
                listeners.Clear();
            }
        }

        public void dispatch(Event anEvent)
        {
            try
            {
                var logger = Log.CONTEXT_EVENTS;

                var localListeners = new HashSet<ContextEventListener>(this.listeners);

                if (logger.isDebugEnabled())
                {
                    logger.debug("Dispatching event '" + anEvent + "', " + localListeners.Count + " listeners");
                }

                foreach (var el in localListeners.Where(el => anEvent.getListener() == null || anEvent.getListener().Equals(el.getListenerCode())))
                {
                    try
                    {
                        if (!el.shouldHandle(anEvent))
                        {
                            if (logger.isDebugEnabled())
                            {
                                logger.debug("Listener '" + el + "' does not want to handle event: " + anEvent);
                            }
                            continue;
                        }

                        if (logger.isDebugEnabled())
                        {
                            logger.debug("Listener '" + el + "' is going to handle event: " + anEvent);
                        }

                        el.handle(anEvent);
                    }
                    catch (ObsoleteListenerException)
                    {
                        this.removeListener(el);
                    }
                    catch (Exception ex)
                    {
                        Log.CONTEXT_EVENTS.warn("Error handling event '" + this.getDefinition().getName() + "'", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.CONTEXT_EVENTS.error("Unexpected error occurred while dispatching event '" + anEvent + "'", ex);
            }
        }

        public override string ToString()
        {
            return this.definition + " - " + this.listeners.Count + " listeners";
        }

        public int CompareTo(EventData d)
        {
            return definition.CompareTo(d.getDefinition());
        }
    }
}