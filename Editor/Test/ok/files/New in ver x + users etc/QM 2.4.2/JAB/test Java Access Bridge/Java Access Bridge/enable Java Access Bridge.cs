
 Enables Java Access Bridge. It initially is disabled after installing Java.

 Change the path if jabswitch.exe is not there.
 Does not require QM to run as Administrator.

RunConsole2("$Program Files$\Java\jre7\bin\jabswitch -enable" _s); out _s.trim
out "[9]If enabled but still does not work, restart QM and Java apps."
out "[9]If does not work in OpenOffice, in OpenOffice check Options -> OpenOffice.org -> Accessibility -> Support assistive."
 RunConsole2("$Program Files$\Java\jre7\bin\jabswitch -disable")
