function $action

 Executes a Java object's action.

 action - action name. You can see action names in "Find accessible object" dialog -> "Properties".

 REMARKS
 Most accessible objects can have only 1 action. DoDefaultAction executes it.
 In Java windows, some objects have multiple actions. For example, text objects. This function can execute any of them. DoDefaultAction executes the first.

 Added in: QM 2.4.2.


if(!a) end ERR_INIT
a.DoDefaultAction(action); err end _error
