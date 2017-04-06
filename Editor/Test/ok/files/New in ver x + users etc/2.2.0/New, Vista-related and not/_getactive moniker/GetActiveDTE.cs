function'IUnknown [hwnd]
 
if(!hwnd) hwnd=win
if( !hwnd ) ret

int dwProcID
GetWindowThreadProcessId(hwnd &dwProcID)

IRunningObjectTable pROT
IEnumMoniker pEnumMonikers
IBindCtx pBindCtx
IMoniker mon
IUnknown punkDTE

GetRunningObjectTable(0 &pROT)
pROT.EnumRunning(&pEnumMonikers)
pEnumMonikers.Reset()

CreateBindCtx(0 &pBindCtx)

int nNumElements
ARRAY(str) rgMatches

rep
	mon=0
	pEnumMonikers.Next(1 &mon &nNumElements)
	if(0==nNumElements)
		break
	
	word* olestrDisplayName
	mon.GetDisplayName(pBindCtx 0 &olestrDisplayName)
	str sAnsiDisplayName.ansi(olestrDisplayName)
	CoTaskMemFree(olestrDisplayName)
	out sAnsiDisplayName
 	
	if(findrx(sAnsiDisplayName "!VisualStudio\.DTE\.\d+\.\d+:(\d+)" 0 (1|4) rgMatches) > 0)
		int nCurrPID = val(rgMatches[1 0])
		if(nCurrPID==dwProcID)
			pROT.GetObject(mon &punkDTE)
			break

err+
	mes "GetActiveDTE: ERROR"
	punkDTE=0

ret punkDTE 

