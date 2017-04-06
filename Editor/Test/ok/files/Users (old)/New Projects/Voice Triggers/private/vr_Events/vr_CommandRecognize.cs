 /VR_Main
function ID BSTR'CmdName Flags BSTR'Action NumLists BSTR'ListValues BSTR'cmd
str s=cmd
 out s
mac s
err out "Voice triggers: %s: %s" _error.description s
