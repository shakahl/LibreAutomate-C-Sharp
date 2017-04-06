 Registers custom URL protocol to run QM macros.
 Then you can run QM macros from:
   Windows 'Run' dialog and some programs:  qm:macro name
   Hyperlinks:  <a href="qm:macro name">link text</a>


if(!IsUserAdmin) mes- "QM must be running as administrator." "" "x"
rset "URL:QM Protocol" "" "qm" HKEY_CLASSES_ROOT
rset "" "URL Protocol" "qm" HKEY_CLASSES_ROOT
rset "qmcl.exe M ''qm_protocol_handler'' C %1" "" "qm\shell\open\command" HKEY_CLASSES_ROOT
