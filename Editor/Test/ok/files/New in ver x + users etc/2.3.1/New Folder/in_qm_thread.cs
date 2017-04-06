function a b
 opt waitmsg 1
 10
mes "%i %i" "" "" a b

if(a=1) SendMessage _hwndqm WM_SETTEXT 3 "M ''in_qm_thread'' A 3 4"
