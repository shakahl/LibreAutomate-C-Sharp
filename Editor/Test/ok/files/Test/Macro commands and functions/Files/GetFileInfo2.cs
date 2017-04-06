SYSTEMTIME st
int attr size
lpstr s=GetFileInfo("$qm$\winapiQM.txt" 0 &attr &size &st 0 0 1)
if(s) out "%s attr=0x%X size=%u date=%u/%u/%u time=%u:%u:%u" s attr size st.wMonth st.wDay st.wYear st.wHour st.wMinute st.wSecond

DATE d
GetFileInfo "$qm$\ok.qml" 0 0 0 &d 0 0 2
out d
