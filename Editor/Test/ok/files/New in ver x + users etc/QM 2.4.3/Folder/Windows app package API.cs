int w=win("Calculator" "ApplicationFrameWindow")
w=child("Calculator" "Windows.UI.Core.CoreWindow" w) ;; 'Calculator'

out _s.getwinexe(w 1)

type PACKAGE_ID reserved processorArchitecture %version @*name @*publisher @*resourceId @*publisherId
dll- kernel32 #GetPackageId hProcess *bufferLength !*pBuffer
 dll- kernel32 #GetPackagePath PACKAGE_ID*packageId reserved *pathLength @*path
dll- kernel32 #GetApplicationUserModelId hProcess *applicationUserModelIdLength @*applicationUserModelId



int pid; GetWindowThreadProcessId(w &pid)
 out pid
__Handle hp=OpenProcess(PROCESS_QUERY_LIMITED_INFORMATION 0 pid)
 out hp

str _packID.all(1000); _i=_packID.nc
if(GetPackageId(hp &_i _packID)) ret
_packID.fix(_i)
PACKAGE_ID* p=_packID
 out "%S[]%S[]%S[]%S" p.name p.publisher p.resourceId p.publisherId

 _s.all(1000); _i=_s.nc
 if(GetPackagePath(p 0 &_i +_s)) ret
 _s.ansi(_s -1 _i)
 out _s

_s.all(1000); _i=_s.nc
if(GetApplicationUserModelId(hp &_i +_s)) ret
_s.ansi(_s -1 _i)
out _s



 __Hicon h=GetWindowIcon


 out
 EnumPropsEx w &sub.EnumProc 0
 
 
 #sub EnumProc
 function# hwnd $lpszString hData x
 
 if(lpszString<=0xffff) ret 1 ;;atom
 out lpszString
 ret 1

 out FindProp(w "DelayServiceAvailabilityChangedOperationId" _s)
 out _s
