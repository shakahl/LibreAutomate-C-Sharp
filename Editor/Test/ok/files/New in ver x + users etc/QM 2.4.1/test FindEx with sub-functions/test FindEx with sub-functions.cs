OnScreenDisplay "test" ;;runs asynchronously, in thread "OSD_Main"

 int iid=qmitem("OnScreenDisplay:Main")
 int iid=qmitem("\System\Functions\Dialog\OnScreenDisplay:Main")
 out iid
 out qmitem("OnScreenDisplay")
 if(iid) out _s.getmacro(iid 1)

 out IsThreadRunning("OnScreenDisplay:Main")
 out EnumQmThreads(0 0 0 "OnScreenDisplay:Main")


 out IsThreadRunning("\System\Functions\Dialog\OnScreenDisplay:Main")
 WaitForThreads 10 "OnScreenDisplay:Main" ;;without this, exe would exit now, and you would not see the OSD
 1
 EndThread "OnScreenDisplay:Main"


 qm.