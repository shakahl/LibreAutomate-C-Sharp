 note: run as administrator.

if(RunConsole2("netsh interface set interface ''Ethernet 3'' admin=disable")) end "failed"
mes "Now should be disabled. Click OK to enable again."
if(RunConsole2("netsh interface set interface ''Ethernet 3'' admin=enable")) end "failed"
