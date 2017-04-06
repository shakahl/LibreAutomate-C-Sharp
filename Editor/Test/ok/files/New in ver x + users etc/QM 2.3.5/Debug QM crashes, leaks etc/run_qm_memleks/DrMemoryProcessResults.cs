 /Macro2001
function str&s nSysFunc

 Removes Dr. Memory error report useless entries.
 1. Single-line entries (no stack).
 2. Where stack begins with nSysFunc lines without a source file, ie the error is deep in system dlls.
 3. Containing one of functions from the list sList here.


str sList=
 USER32.dll!DdeEnableCallback
 CTypes::AddIntrinsicTypes
 CStaticVariables::AddDefaultVariables
 util::LoadExeResourceUnmix
 NtUserFindWindowEx
 NtUserRealChildWindowFromPoint
 NtUserWindowFromPoint
 NtUserGetForegroundWindow
 NtUserSetParent
 NtUserGetSystemMenu
 NtUserGetAncestor
 NtUserSetCursor
 !LoadAccelerators
 OutputDebugString
 GetScrollBarInfo
 SetScrollInfo
 GetDateFormat
 GetTimeFormat
 $CreateMutexA.+\n\# 3 Init1
 $CreateWindowHidden.+\n\# 4 ThreadFF
 $CreateWindowHidden.+\n\# 4 tbGetRealOwner
 SurfaceImpl::InitPixMap
 ScintillaWin::RealizeWindowPalette
 $NtUserBeginPaint.+\n\Q# 1 qmsci.dll!ScintillaWin::WndProc\E
 CToolTip::CreateTT
 # 3 _lock
 FormatKeyString
 NtGdiCreateSolidBrush
 $CBrushQm::Create.+\n\Q# 3 tbStartupOptions\E
 $NtUserSetWinEventHook.+\n\Q# 1 snxhk.dll\E
 # 0 system call NtUserSetFocus
 WS2_32.dll!socket
 $NtUserSetWinEventHook.*\n\Q# 1 qmhook32.dll!HookUnhook\E

str rx sSummary
s.replacerx("^~~[^~]+~~ " "" 8)
sSummary.get(s find(s "ERRORS FOUND:"))

ARRAY(str) a aList
int i j
if(!findrx(s "(?sm)^.+?[][]" 0 4 a)) end "bad regex"
s.flags=1; s.fix(0)
aList=sList

for i 1 a.len
	str& r=a[0 i]
	
	if(numlines(r)=1) continue
	
	if(findrx(r F"^.+[](.+[^\]][]){{{nSysFunc}}")>=0) continue
	
	for(j 0 aList.len)
		lpstr k=aList[j]
		if(k[0]='$') if(findrx(r k+1)>=0) break
		else if(find(r k)>=0) break
	if(j<aList.len) continue
	
	s+r

s+sSummary

 CTypes::AddDefaultTypes
