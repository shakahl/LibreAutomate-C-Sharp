out
opt waitmsg 1
ARRAY(int) a
a[]=mac("Function268" "" 3)
a[]=mac("Function268" "" 1)
a[]=mac("Function268" "" 9)
a[]=mac("Function268" "" 2)
 PostMessage 0 WM_APP 0 0
 out MsgWaitForMultipleObjectsEx(a.len &a[0] 2000 QS_ALLINPUT|QS_ALLPOSTMESSAGE MWMO_ALERTABLE|MWMO_WAITALL|MWMO_INPUTAVAILABLE)

 out WaitForMultipleObjectsEx(a.len &a[0] 1 3000 1)
 out wait(0 HM a)
out wait(0 HMA a)
 out wait(0 HMA a[0] a[1])
out "ok"
