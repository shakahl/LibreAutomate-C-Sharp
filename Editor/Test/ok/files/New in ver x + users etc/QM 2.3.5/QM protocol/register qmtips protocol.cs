if(!IsUserAdmin) mes- "QM must be running as administrator." "" "x"
rset "URL:QM Tips Protocol" "" "qmtips" HKEY_CLASSES_ROOT
rset "" "URL Protocol" "qmtips" HKEY_CLASSES_ROOT
rset "q:\app\qmcl.exe M ''qmtips_protocol_handler'' C %1" "" "qmtips\shell\open\command" HKEY_CLASSES_ROOT
