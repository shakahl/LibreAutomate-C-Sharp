if(!IsUserAdmin) mes- "QM must be running as administrator." "" "x"
rset "qmcl.exe M ''url_protocol_handler'' C %1" "" "http\shell\open\command" HKEY_CLASSES_ROOT
rset "qmcl.exe M ''url_protocol_handler'' C %1" "" "https\shell\open\command" HKEY_CLASSES_ROOT

 Replaces current handler command, which probably is
 "C:\Program Files (x86)\Mozilla Firefox\firefox.exe" -osint -url "%1"
