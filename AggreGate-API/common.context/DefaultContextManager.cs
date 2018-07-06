using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Threading;
using System.Windows.Controls;
using Collections;
using com.tibbo.aggregate.common.data;
using com.tibbo.aggregate.common.@event;

namespace com.tibbo.aggregate.common.context
{
    using System.Collections;
    using System.Threading;

    using com.tibbo.aggregate.common.util;

    using JavaCompatibility;

    public class DefaultContextManager : ContextManager
    {
        private readonly Boolean async;

        private Boolean started;

        private Context rootContext;

        private readonly CallerController<CallerData> callerController = new UncheckedCallerController();

        private readonly IDictionary<String, IDictionary<String, ListSet<ContextEventListener>>> eventListeners =
            new SynchronizedDictionary<String, IDictionary<String, ListSet<ContextEventListener>>>(new Dictionary<String, IDictionary<String, ListSet<ContextEventListener>>>());

        private readonly IDictionary<String, IDictionary<String, ListSet<ContextEventListener>>> maskListeners =
            new SynchronizedDictionary<String, IDictionary<String, ListSet<ContextEventListener>>>(new Dictionary<String, IDictionary<String, ListSet<ContextEventListener>>>());

        private EventDispatcher eventDispatcher;

        public DefaultContextManager(Boolean async)
        {
            this.async = async;
            if (async)
            {
                ensureDispatcher();
            }
        }

        private void ensureDispatcher()
        {
            if (eventDispatcher == null)
            {
                eventDispatcher = new EventDispatcher();
            }
        }

        public void setRootAndStart(Context aContext)
        {
            setRoot(aContext);
            start();
        }

        public void start()
        {
            if (async)
            {
                ensureDispatcher();
                eventDispatcher.start();
            }

            if (rootContext != null)
            {
                rootContext.start();
            }
            started = true;
        }

        public virtual void stop()
        {
            started = false;
            if (eventDispatcher != null)
            {
                eventDispatcher.interrupt();
                eventDispatcher = null;
            }
            if (rootContext != null)
            {
                rootContext.stop();
            }
        }

        public void restart()
        {
            stop();
            start();
        }

        public Context getRoot()
        {
            if (rootContext == null)
            {
                throw new InvalidOperationException("Root context not defined");
            }
            return rootContext;
        }

        public void setRoot(Context newRoot)
        {
            rootContext = newRoot;
            rootContext.setup(this);
            contextAdded(newRoot);
        }

        public Context get(String contextName, CallerController<CallerData> caller)
        {
            return getRoot().get(contextName, caller);
        }

        public Context get(String contextName)
        {
            return getRoot().get(contextName);
        }

        private void addEventListener(String context, String eventString, ContextEventListener listener, Boolean mask)
        {
            var con = get(context, listener.getCallerController());

            if (con != null)
            {
                var events = EventUtils.getEvents(con, eventString, listener.getCallerController());

                foreach (var ed in events)
                {
                    addListenerToContext(con, ed.getName(), listener, mask);
                }
            }
            else
            {
                if (!mask)
                {
                    var eel = getListeners(context, eventString);

                    if (!eel.Contains(listener))
                    {
                        eel.Add(listener);
                    }
                }
            }
        }

        protected void addListenerToContext(Context con, String eventString, ContextEventListener listener, Boolean mask)
        {
            var ed = con.getEventDefinition(eventString, listener.getCallerController());
            if (ed != null)
            {
                con.addEventListener(eventString, listener);
            }
        }

        private void removeEventListener(String context, String eventString, ContextEventListener listener, Boolean mask)
        {
            var con = get(context, listener.getCallerController());

            if (con != null)
            {
                if (con.getEventDefinition(eventString) != null)
                {
                    removeListenerFromContext(con, eventString, listener, mask);
                }
            }
            else
            {
                if (!mask)
                {
                    var eel = getListeners(context, eventString);

                    if (eel != null)
                    {
                        eel.Remove(listener);
                    }
                }
            }
        }

        protected virtual void removeListenerFromContext(Context con, String eventString, ContextEventListener listener,
                                                         Boolean mask)
        {
            con.removeEventListener(eventString, listener);
        }

        public virtual void addMaskEventListener(String mask, String eventString, ContextEventListener listener)
        {
            var contexts = ContextUtils.expandContextMaskToPaths(mask, this, listener.getCallerController());

            foreach (var con in contexts)
            {
                addEventListener(con, eventString, listener, true);
            }

            getMaskListeners(mask, eventString).Add(listener);
        }

        public virtual void removeMaskEventListener(String mask, String eventString, ContextEventListener listener)
        {
            var contexts = ContextUtils.expandContextMaskToContexts(mask, this, listener.getCallerController());

            foreach (var con in contexts)
            {
                var events = EventUtils.getEvents(con, eventString, listener.getCallerController());

                foreach (var ed in events)
                {
                    removeEventListener(con.getPath(), ed.getName(), listener, true);
                }
            }

            getMaskListeners(mask, eventString).Remove(listener);
        }

        protected Collections.ISet<ContextEventListener> getListeners(String context, String eventString)
        {
            var cel = getContextListeners(context);

            var eel = cel[eventString];

            if (eel == null)
            {
                eel = new SynchronizedSet<ContextEventListener>(new ListSet<ContextEventListener>());
                cel.Add(eventString, eel);
            }

            return eel;
        }

        private IDictionary<String, Collections.ISet<ContextEventListener>> getContextListeners(String context)
        {
            var cel = eventListeners[context];

            if (cel == null)
            {
                cel =
                    new SynchronizedDictionary<String, ListSet<ContextEventListener>>(
                        new Dictionary<String, ListSet<ContextEventListener>>());
                eventListeners.Add(context, cel);
            }

            return (IDictionary<string, Collections.ISet<ContextEventListener>>) cel;
        }

        protected Collections.ISet<ContextEventListener> getMaskListeners(String mask, String eventString)
        {
            var cel = getContextMaskListeners(mask);

            var eel = cel[eventString];

            if (eel == null)
            {
                eel = new SynchronizedSet<ContextEventListener>(new ListSet<ContextEventListener>());
                cel.Add(eventString, eel);
            }

            return eel;
        }

        private IDictionary<String, Collections.ISet<ContextEventListener>> getContextMaskListeners(String mask)
        {
            var cel = maskListeners[mask];

            if (cel == null)
            {
                cel =
                    new SynchronizedDictionary<String, ListSet<ContextEventListener>>(
                        new Dictionary<String, ListSet<ContextEventListener>>());
                maskListeners.Add(mask, cel);
            }

            return (IDictionary<string, Collections.ISet<ContextEventListener>>) cel;
        }

        public void contextAdded(Context con)
        {
            lock (eventListeners)
            {
                IDictionary<string, ListSet<ContextEventListener>> cel = null;

                if (eventListeners.ContainsKey(con.getPath()))
                    cel = eventListeners[con.getPath()];

                if (cel != null)
                {
                    foreach (var ev in cel.Keys)
                    {
                        var listeners = cel[ev];

                        lock (listeners)
                        {
                            foreach (var el in listeners)
                            {
                                if (con.getEventData(ev) != null)
                                {
                                    con.addEventListener(ev, el);
                                }
                            }
                        }
                    }
                }
            }

            lock (maskListeners)
            {
                foreach (var mask in maskListeners.Keys)
                {
                    if (!ContextUtils.matchesToMask(mask, con.getPath())) continue;
                    var cel = getContextMaskListeners(mask);

                    foreach (var ev in cel.Keys)
                    {
                        var listeners = cel[ev];

                        lock (listeners)
                        {
                            foreach (var el in listeners)
                            {
                                var events = EventUtils.getEvents(con, ev,
                                                                  el.getCallerController());

                                foreach (var ed in events)
                                {
                                    addListenerToContext(con, ed.getName(), el, true);
                                }
                            }
                        }
                    }
                }
            }
        }


        internal class EventListenerRemover : ContextVisitor<Context>
        {
            private readonly DefaultContextManager owner;

            public EventListenerRemover(DefaultContextManager aManager)
            {
                owner = aManager;
            }

            public void visit(Context vc)
            {
                foreach (var mask in owner.maskListeners.Keys)
                {
                    if (!ContextUtils.matchesToMask(mask, vc.getPath())) continue;
                    var cel = owner.getContextMaskListeners(mask);

                    foreach (var ev in cel.Keys)
                    {
                        foreach (var lis in owner.getMaskListeners(mask, ev))
                        {
                            vc.removeEventListener(ev, lis);
                        }
                    }
                }
            }
        }

        internal class EventListenerAdder : ContextVisitor<Context>
        {
            private readonly DefaultContextManager owner;

            public EventListenerAdder(DefaultContextManager aManager)
            {
                owner = aManager;
            }

            public void visit(Context vc)
            {
                var cel = this.owner.getContextListeners(vc.getPath());
                foreach (var ed in vc.getEventDefinitions(owner.callerController))
                {
                    var listeners = cel[ed.getName()];
                    if (listeners != null)
                    {
                        listeners.AddAll(vc.getEventData(ed.getName()).getListeners());
                    }
                }
            }
        }


        public virtual void contextRemoved(Context con)
        {
            try
            {
                con.accept(new EventListenerRemover(this));

                con.accept(new EventListenerAdder(this));
            }
            catch (ContextException ex)
            {
                throw new InvalidOperationException(ex.Message, ex);
            }
        }

        public void contextInfoChanged(Context con)
        {

        }


        public void variableAdded(Context con, VariableDefinition vd)
        {
        }

        public void variableRemoved(Context con, VariableDefinition vd)
        {
        }

        public void functionAdded(Context con, FunctionDefinition fd)
        {
        }

        public void functionRemoved(Context con, FunctionDefinition fd)
        {
        }

        public virtual void eventAdded(Context con, EventDefinition ed)
        {
            var maskListenersCopy =
                new Dictionary<String, IDictionary<String, ListSet<ContextEventListener>>>(maskListeners);

            foreach (var mask in maskListenersCopy.Keys)
            {
                if (!ContextUtils.matchesToMask(mask, con.getPath())) continue;
                var cel = getContextMaskListeners(mask);

                foreach (var ev in cel.Keys)
                {
                    if (!EventUtils.matchesToMask(ev, ed)) continue;
                    var listeners = cel[ev];

                    lock (listeners)
                    {
                        foreach (var el in listeners)
                        {
                            addListenerToContext(con, ed.getName(), el, true);
                        }
                    }
                }
            }
        }

        public void eventRemoved(Context con, EventDefinition ed)
        {
        }

        public void queue(EventData ed, Event ev)
        {
            if (!async || ed.getDefinition().isSynchronous())
            {
                ed.dispatch(ev);
            }
            else
            {
                if (eventDispatcher == null)
                {
//                    Log.CONTEXT_EVENTS.debug("Cannot queue event '" + ev + "': context manager is not running");
                    return;
                }

                QueuedEvent qe = new QueuedEvent(ed, ev);

                try
                {
                    eventDispatcher.queue(qe);
                }
                catch (Exception ex1)
                {
//                    Log.CONTEXT_EVENTS.debug("Interrupted while queueing event: " + ev);
                }
            }
        }

        public Boolean isStarted()
        {
            return started;
        }

        private class EventDispatcher
        {
            private readonly Thread dispatcher;
            private readonly BlockingQueue<QueuedEvent> undispatchedEvents;
            private bool interrupted = false;

            public EventDispatcher()
            {
                dispatcher = new Thread(run);
                dispatcher.Name = "EventDispatcher/" + Thread.CurrentThread.Name;
                dispatcher.Priority = ThreadPriority.AboveNormal; // Setting very high priority to avoid bottlenecks
                undispatchedEvents = new BlockingQueue<QueuedEvent>();
            }

            public void interrupt()
            {
                interrupted = true;
            }

            public void start()
            {
                dispatcher.Start();
            }

            public void queue(QueuedEvent ev)
            {

                // FIXME: must implementing:  Protecting from deadlocks by prohibiting new event submission from the dispatcher thread

                undispatchedEvents.enqueue(ev);

                /*
                                if (Thread.CurrentThread == this && es != null) // Protecting from deadlocks by prohibiting new event submission from the dispatcher thread
                                {
                                    es.submit(new Runnable()
                                {
                                    public void run()
                                    {
                                        try {
                                                undispatchedEvents.put(ev);
                                         }
                                        catch (Exception ex)
                                        {
                                            // Ignoring
                                         }
                                    }
                                });
                      }
                      else
                      {
                        undispatchedEvents.put(ev);
                      }
                */
            }
    
    private void run()
    {
        while (!interrupted)
        {
            try
            {
                QueuedEvent ev = null;

                try
                {
                    ev = undispatchedEvents.dequeue();
                }
                catch (Exception ex)
                {
                    break;
                }

                if (ev != null)
                {
                    ev.dispatch();
                }
            }
        catch (Exception ex)
            {
                // Normally all errors should be handled in EventData.dispatch(), so there are almost no chances we'll get here
                Log.CONTEXT_EVENTS.error("Unexpected critical error in event dispatcher", ex);
            }
        }
    }

        public int getQueueLength()
        {
            return undispatchedEvents.size;
        }
    }

        public class QueuedEvent
        {
            private readonly EventData ed;
            private readonly Event ev;
    
            public QueuedEvent(EventData ed, Event ev)
            {
                this.ed = ed;
                this.ev = ev;
            }

            public void dispatch()
            {
                ed.dispatch(ev);
            }
        }
    }
}