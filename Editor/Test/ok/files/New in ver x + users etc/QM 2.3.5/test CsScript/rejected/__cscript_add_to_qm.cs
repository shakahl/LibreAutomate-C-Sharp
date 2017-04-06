 /
function! $_exeFile ;;_exeFile: qm or qmmacro

int h=BeginUpdateResource(_s.expandpath(_exeFile) 0)
if(!UpdateResource(h +