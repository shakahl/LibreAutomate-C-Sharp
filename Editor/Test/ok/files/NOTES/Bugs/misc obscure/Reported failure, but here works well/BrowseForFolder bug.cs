 http://www.quickmacros.com/forum/viewtopic.php?f=12&t=3116&start=0&st=0&sk=t&sd=a&hilit=BrowseForFolder
 QM2.3.0 crashes with BrowseForFolder flag 4 and PSHotFolders (http://www.pssoftlab.com/).
 Works well here. (On XP. On Vista the program does not work.)

if(!BrowseForFolder(_s "" 4)) ret
 if(!BrowseForFolder(_s "$program files$" 4)) ret
out _s

 with this also crashes:
BROWSEINFOW b
b.ulFlags=BIF_USENEWUI|BIF_RETURNONLYFSDIRS
SHBrowseForFolderW(&b)
