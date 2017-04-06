function! iSub [flags] ;;flags: 1 events (find sel message (must have) and sel wParam, and store in e)

__Sub& r=a[iSub]
FINDRX k.ifrom=r.subOffset; k.ito=k.ifrom+r.subLen
if(findrx(sText "^function# +\[?hDlg\]? +\[?message\]? +\[?wParam\]? +\[?lParam\]?" k 9)<0) ret
if flags&1
	int i j
	i=findrx(sText "^sel message\b.*[]((?:[\t,# ;\\/].*[]|[])+)" k 8 j 1); if(i<0) ret
	e.wmBegin=i; e.wmEnd=i+j
	k.ifrom=e.wmEnd
	i=findrx(sText "^sel wParam\b.*[]((?:[\t,# ;\\/].*[]|[])+)" k 8 j 1)
	if(i<0) i=0; j=0
	e.cmdBegin=i; e.cmdEnd=i+j
ret 1
