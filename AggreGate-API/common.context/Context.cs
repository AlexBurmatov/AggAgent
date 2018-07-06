using System;
using System.Collections.Generic;
using com.tibbo.aggregate.common.data;
using com.tibbo.aggregate.common.datatable;
using com.tibbo.aggregate.common.security;

namespace com.tibbo.aggregate.common.context
{
    using com.tibbo.aggregate.common.@event;

    public interface Context : IComparable
    {
        /**
         * This method is called after the context has been added to a context tree and it became aware of its full path. Note, that default implementation of this method in AbstractContext calls tree
         * methods: setupPermissions(), setupMyself() and setupChildren(). These methods should provide initialization logic in inherited classes instead of overridden setup() method.
         * 
         * @param contextManager
         *          ContextManager heading current context tree
         */
        void setup(ContextManager contextManager);

        /**
         * This method is called when the context is being removed from context tree..
         * 
         * @param contextManager
         *          ContextManager heading current context tree
         */
        void teardown();

        /**
         * This method should return true if the context has already been initialized and setupMyself() finished execution. Its default implementation in AbstractContext should not be overridden.
         * 
         * @return true if setupMyself() has already been completed.
         */
        bool isSetupComplete();

        /**
         * This method should return true if the context has already been initialized its basic information (description, type, etc).
         * 
         * @return true if basic context information has been initialized.
         */
        bool isInitializedInfo();

        /**
         * This method should return true if the context has already been initialized its children.
         * 
         * @return true if context children have been initialized.
         */
        bool isInitializedChildren();

        /**
         * This method should return true if the context has already been initialized its variables.
         * 
         * @return true if context variables have been initialized.
         */
        bool isInitializedVariables();

        /**
         * This method should return true if the context has already been initialized its functions.
         * 
         * @return true if context functions have been initialized.
         */
        bool isInitializedFunctions();

        /**
         * This method should return true if the context has already been initialized its events.
         * 
         * @return true if context events have been initialized.
         */
        bool isInitializedEvents();

        /**
         * This method is called when context tree is being started after its initialization. All contexts in the tree should be available at the moment of call.
         */
        void start();

        /**
         * This method is called when context tree is being stopped before its de-initialization. All contexts in the tree should be available at the moment of call.
         */
        void stop();

        /**
         * Returns true if context was started but not yet stopped.
         */
        bool isStarted();

        /**
         * Returns context name.
         */
        string getName();

        /**
         * Returns context path (full name).
         */
        string getPath();

        /**
         * Returns path of context's peer in distributed environment.
         */
        string getRemotePath();

        /**
         * When a certain context subtree from one server is connected to another server, this method will return the remote path of this subtree's root context. If current context doesn't have a remote
         * peer, this method returns null.
         */
        string getRemoteRoot();

        /**
         * Returns true if context is a remote context's proxy.
         */
        bool isProxy();

        /**
         * Returns true if context has a remote peer in the distributed architecture.
         */
        bool isDistributed();

        /**
         * Returns context detailed description that includes description and path.
         */
        string toDetailedString();

        /**
         * Returns context description.
         */
        string getDescription();

        /**
         * Returns context type.
         */
        string getType();

        /**
         * Returns context group name of NULL if context does not belong to a group.
         */
        string getGroup();

        /**
         * Returns context comparison index or NULL if index is not defined.
         */
        int? getIndex();

        /**
         * Returns context icon ID.
         */
        string getIconId();

        /**
         * Returns context status or null if status is not enabled;
         */
        //ContextStatus getStatus();

        /**
         * Returns context manager those context tree contains this context.
         */
        ContextManager getContextManager();

        /**
         * Returns list of children contexts that are accessible by the specified <code>CallerController</code>.
         */
        List<Context> getChildren(CallerController<CallerData> caller);

        /**
         * Returns list of children contexts.
         */
        List<Context> getChildren();

        /**
         * Returns list of visible children contexts.
         */
        List<Context> getVisibleChildren(CallerController<CallerData> caller);

        /**
         * Returns list of visible children contexts.
         */
        List<Context> getVisibleChildren();

        /**
         * Returns true if context's visible children are mapped (e.g. for group and aggregation contexts).
         */
        bool isMapped();

        /**
         * Returns list of mapped children contexts.
         */
        List<Context> getMappedChildren(CallerController<CallerData> caller);

        /**
         * Returns list of mapped children contexts.
         */
        List<Context> getMappedChildren();

        /**
         * Returns root context of the context tree containing this context.
         */
        Context getRoot();

        /**
         * Returns context with the selected path.
         * 
         * <code>path</code> argument may be absolute of relative to this context. This method uses provided <code>CallerController</code> for permission checking.
         */
        Context get(string path, CallerController<CallerData> caller);

        /**
         * Returns context with the selected path.
         * 
         * <code>path</code> argument may be absolute of relative to this context.
         * 
         * Note: if this Context is a part of distributed context tree and path argument is not relative, the method will return local context matching its remote "peer" with given path. To get the local
         * context with the given path, use {@link ContextManager#get(String)} instead.
         */
        Context get(string path);

        /**
         * Returns child of this context with the specified name.
         * 
         * <code>path</code> argument may be absolute of relative to this context.
         * 
         * Note: if this Context is a part of distributed context tree and path argument is not relative, the method will return local context matching its remote "peer" with given path. To get the local
         * context with the given path, use {@link ContextManager#get(String, CallerController)} instead.
         * 
         * This method uses provided <code>CallerController</code> for permission checking.
         */
        Context getChild(string name, CallerController<CallerData> caller);

        /**
         * Returns child of this context with the specified name.
         */
        Context getChild(string name);

        /**
         * Adds new child to the current context.
         */
        void addChild(Context child);

        /**
         * Removes child of current context.
         */
        void removeChild(Context child);

        /**
         * Removes child with specified name.
         */
        void removeChild(string name);

        /**
         * Permanently destroys child of current context.
         */
        void destroyChild(Context child, bool moving);

        /**
         * Permanently destroys child with specified name.
         */
        void destroyChild(string name, bool moving);

        /**
         * Permanently destroys this context.
         */
        void destroy(bool moving);

        /**
         * Prepare context to update.
         */
        void updatePrepare();

        /**
         * Moves and/or renames the context.
         */
        void move(Context newParent, string newName);

        void setParent(Context parent);

        /**
         * Returns parent of this context.
         */
        Context getParent();

        /**
         * Returns true if parentContext is a parent of this context or some of its parents.
         */
        bool hasParent(Context parentContext);

        /**
         * Adds variable definition to this context.
         */
        void addVariableDefinition(VariableDefinition def);

        /**
         * Removes variable definition from this context.
         */
        void removeVariableDefinition(string name);

        /**
         * Returns data of variable with specified name.
         */
        VariableData getVariableData(string name);

        /**
         * Returns definition of variable with specified name.
         */
        VariableDefinition getVariableDefinition(string name);

        /**
         * Returns definition of variable with specified name if it's accessible by caller controller.
         */
        VariableDefinition getVariableDefinition(string name, CallerController<CallerData> caller);

        /**
         * Returns list of variables available for specified <code>CallerController</code>.
         */
        List<VariableDefinition> getVariableDefinitions(CallerController<CallerData> caller);

        /**
         * Returns list of variables.
         */
        List<VariableDefinition> getVariableDefinitions();

        /**
         * Returns list of variables belonging to <code>group</code> that are available for specified <code>CallerController</code>.
         */
        List<VariableDefinition> getVariableDefinitions(CallerController<CallerData> caller, string group);

        /**
         * Returns list of variables belonging to <code>group</code>.
         */
        List<VariableDefinition> getVariableDefinitions(string group);

        /**
         * Returns list of variables.
         */
        List<VariableDefinition> getVariableDefinitions(CallerController<CallerData> caller, bool includeHidden);

        /**
         * Adds function definition to this context.
         */
        void addFunctionDefinition(FunctionDefinition def);

        /**
         * Removes function definition from this context.
         */
        void removeFunctionDefinition(string name);

        /**
         * Returns data of function with specified name.
         */
        FunctionData getFunctionData(string name);

        /**
         * Returns definition of function with specified name.
         */
        FunctionDefinition getFunctionDefinition(string name);

        /**
         * Returns definition of function with specified name if it's accessible by caller controller.
         */
        FunctionDefinition getFunctionDefinition(string name, CallerController<CallerData> caller);

        /**
         * Returns list of functions available for specified <code>CallerController</code>.
         */
        List<FunctionDefinition> getFunctionDefinitions(CallerController<CallerData> caller);

        /**
         * Returns list of functions.
         */
        List<FunctionDefinition> getFunctionDefinitions();

        /**
         * Returns list of functions belonging to <code>group</code> that are available for specified <code>CallerController</code>.
         */
        List<FunctionDefinition> getFunctionDefinitions(CallerController<CallerData> caller, string group);

        /**
         * Returns list of functions belonging to <code>group</code>.
         */
        List<FunctionDefinition> getFunctionDefinitions(string group);

        /**
         * Returns list of functions.
         */
        List<FunctionDefinition> getFunctionDefinitions(CallerController<CallerData> caller, bool includeHidden);

        /**
         * Adds event definition to this context.
         */
        void addEventDefinition(EventDefinition def);

        /**
         * Removes event definition from this context.
         */
        void removeEventDefinition(string name);

        /**
         * Returns definition of event with specified name.
         */
        EventDefinition getEventDefinition(string name);

        /**
         * Returns definition of event with specified name if it's accessible by caller controller.
         */
        EventDefinition getEventDefinition(string name, CallerController<CallerData> caller);

        /**
         * Returns <code>EventData</code> of event with specified name.
         */
        EventData getEventData(string name);

        /**
         * Returns list of events available for specified <code>CallerController</code>.
         */
        List<EventDefinition> getEventDefinitions(CallerController<CallerData> caller);

        /**
         * Returns list of events.
         */
        List<EventDefinition> getEventDefinitions();

        /**
         * Returns list of events belonging to <code>group</code> that are available for specified <code>CallerController</code>.
         */
        List<EventDefinition> getEventDefinitions(CallerController<CallerData> caller, string group);

        /**
         * Returns list of events belonging to <code>group</code>.
         */
        List<EventDefinition> getEventDefinitions(string group);

        /**
         * Returns list of events.
         */
        List<EventDefinition> getEventDefinitions(CallerController<CallerData> caller, bool includeHidden);

        /**
         * Gets variable from context and returns its value.
         */
        DataTable getVariable(string name, CallerController<CallerData> caller, RequestController<RequestData> request);

        /**
         * Gets variable from context and returns its value.
         */
        DataTable getVariable(string name, CallerController<CallerData> caller);

        /**
         * Gets variable from context and returns its value.
         */
        DataTable getVariable(string name);

        /**
         * Returns value of variable as bean or list of beans.
         */
        //object getVariableObject(string name, CallerController<CallerData> caller);

        /**
         * Sets context variable to specified <code>value</code>.
         */
        void setVariable(string name, CallerController<CallerData> caller, DataTable value);

        /**
         * Sets context variable to specified <code>value</code>.
         */
        void setVariable(string name, CallerController<CallerData> caller, RequestController<RequestData> request, DataTable value);

        /**
         * Sets context variable to specified <code>value</code>.
         */
        void setVariable(string name, CallerController<CallerData> caller, params object[] value);

        /**
         * Sets context variable to specified <code>value</code>.
         */
        void setVariable(string name, DataTable value);

        /**
         * Sets context variable to specified <code>value</code>.
         */
        void setVariable(string name, params object[] value);

        /**
         * Gets variable, updates field value in the first record, and sets variable.
         */
        bool setVariableField(string variable, string field, Object value, CallerController<CallerData> cc);

        /**
         * Gets variable, updates field value in the specified record, and sets variable.
         */
        bool setVariableField(string variable, string field, int record, Object value, CallerController<CallerData> cc);

        /**
         * Gets variable, updates field value in the records for those value of compareField equals compareValue, and sets variable.
         */
        void setVariableField(string variable, string field, Object value, string compareField, Object compareValue, CallerController<CallerData> cc);

        /**
         * Executes context function with specified <code>parameters</code> and returns its output.
         */
        DataTable callFunction(string name, CallerController<CallerData> caller, DataTable parameters);

        /**
         * Executes context function with specified <code>parameters</code> and returns its output.
         */
        DataTable callFunction(string name, DataTable parameters);

        /**
         * Executes context function with specified <code>parameters</code> and returns its output.
         */
        DataTable callFunction(string name);

        /**
         * Executes context function with specified <code>parameters</code> and returns its output.
         */
        DataTable callFunction(string name, CallerController<CallerData> caller, RequestController<RequestData> request, DataTable parameters);

        /**
         * Executes context function with specified <code>parameters</code> and returns its output.
         */
        DataTable callFunction(string name, CallerController<CallerData> caller, params object[] parameters);

        /**
         * Executes context function with specified <code>parameters</code> and returns its output.
         */
        DataTable callFunction(string name, params object[] parameters);

        /**
         * Executes context function with specified <code>parameters</code> and returns its output.
         */
        DataTable callFunction(string name, CallerController<CallerData> caller);

        /**
         * Fires context event.
         * 
         * @return Event object or null if event was suppressed by context.
         */
        Event fireEvent(string name);

        /**
         * Fires context event.
         * 
         * @return Event object or null if event was suppressed by context.
         */
        Event fireEvent(string name, CallerController<CallerData> caller);

        /**
         * Fires context event.
         * 
         * @return Event object or null if event was suppressed by context.
         */
        Event fireEvent(string name, DataTable data);

        /**
         * Fires context event.
         * 
         * @return Event object or null if event was suppressed by context.
         */
        Event fireEvent(string name, CallerController<CallerData> caller, DataTable data);

        /**
         * Fires context event.
         * 
         * @return Event object or null if event was suppressed by context.
         */
        Event fireEvent(string name, int level, DataTable data);

        /**
         * Fires context event.
         * 
         * @return Event object or null if event was suppressed by context.
         */
        Event fireEvent(string name, int level, CallerController<CallerData> caller, DataTable data);

        /**
         * Fires context event.
         * 
         * @return Event object or null if event was suppressed by context.
         */
        Event fireEvent(string name, params object[] data);

        /**
         * Fires context event.
         * 
         * @return Event object or null if event was suppressed by context.
         */
        Event fireEvent(string name, DataTable data, int? level, long? id, DateTime? creationtime, int? listener, CallerController<CallerData> caller, FireEventRequestController request);

        /**
         * Add a new action definition to the context.
         * 
         * @param def
         *          ActionDefinition to add
         */
        //void addActionDefinition(ActionDefinition def);

        /**
         * Remove an action definition from the context.
         * 
         * @param name
         *          Name of action to remove
         */
        //void removeActionDefinition(string name);

        /**
         * Returns action definition by name.
         * 
         * @param name
         *          Name of action
         */
        //ActionDefinition getActionDefinition(string name);

        /**
         * Returns action definition by name.
         * 
         * @param name
         *          Name of action
         * @param caller
         *          Caller controller
         */
        //ActionDefinition getActionDefinition(string name, CallerController caller);

        /**
         * Returns default action definition or NULL if there is no default action or it's not available to the caller.
         * 
         * @param caller
         *          Caller controller
         */
        //ActionDefinition getDefaultActionDefinition(CallerController caller);

        /**
         * Returns action definitions.
         */
        //List<ActionDefinition> getActionDefinitions();

        /**
         * Returns action definitions that are accessible for the caller.
         * 
         * @param caller
         *          Caller controller
         */
        //List<ActionDefinition> getActionDefinitions(CallerController caller);

        /**
         * Returns action definitions.
         */
        //List<ActionDefinition> getActionDefinitions(CallerController caller, bool includeHidden);

        /**
         * Returns context permissions.
         */
        Permissions getPermissions();

        /**
         * Returns permissions required to access children of this context.
         */
        Permissions getChildrenViewPermissions();

        /**
         * Adds listener of event with specified name.
         */
        bool addEventListener(string name, ContextEventListener listener);

        /**
         * Adds listener of event with specified name. This method allows to add auto-cleaned listeners by setting weak flag to true.
         */
        bool addEventListener(string name, ContextEventListener listener, bool weak);

        /**
         * Removes listener of event with specified name.
         */
        bool removeEventListener(string name, ContextEventListener listener);

        /**
         * Returns in-memory event history.
         */
        //List<Event> getEventHistory(string name);

        /**
         * Accepts context visitor, i.e. calls visitor.visit(this).
         */
        void accept(ContextVisitor<Context> visitor);


        void reinitialize();

        DataTable decodeRemoteDataTable(TableFormat format, String encodedReply);

    }
}