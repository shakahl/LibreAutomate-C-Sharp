BSTR s="q:\app\menu.ico"
 s="c:\windows\system32"
s="c:\windows\system32\notepad.exe"
s="shell:AppsFolder\Microsoft.WindowsCalculator_8wekyb3d8bbwe!App"
 s="q:\app\qm.exe"; _i=2
 s="q:\app\qm.exe"
 s="q:\app\no.exe"
 s="what?"
 s="q:\app\Lock.cpp"
s="q:\app\MakeExe.cpp"
 s="q:\app2\MakeExe.cpp"
PF
int i=Shell_GetCachedImageIndex(s _i 0)
PN;PO
out i

IImageList il
if(SHGetImageList(SHIL_SMALL IID_IImageList &il)) ret

__Hicon hi=ImageList_GetIcon(il i 0)
out hi
