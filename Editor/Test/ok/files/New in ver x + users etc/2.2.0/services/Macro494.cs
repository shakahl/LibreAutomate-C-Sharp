dll "qm.exe" #StartStopService start $name
 dll "qm.exe" #ControlQmService action ;;action: 1 install, 2 uninstall, 3 start, 4 stop

out
out StartStopService(0 "qmphook");;works only if not using process triggers

 StartStopService(0 "quickmacros2");;don't call from nonadmin
 StartStopService(1 "quickmacros2")

 out StartStopService(1 "seclogon")

 if(!StartStopService(1 "RegSrvc")) out _s.dllerror
