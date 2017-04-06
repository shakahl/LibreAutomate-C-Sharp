if(!IsUserAdmin) mes- "QM must be running as administrator." "" "x"
rset "URL:QM Help Protocol" "" "qmhelp" HKEY_CLASSES_ROOT
rset "" "URL Protocol" "qmhelp" HKEY_CLASSES_ROOT
rset "q:\app\qmcl.exe M ''qmhelp_protocol_handler'' C %1" "" "qmhelp\shell\open\command" HKEY_CLASSES_ROOT
