str programs=
 $program files$\FineEditor\FineEditor.exe
 $program files$\Windows NT\Accessories\wordpad.exe
 and so on
ARRAY(str) ap=programs

str windows=
 Fine Editor
 WordPad
 and so on
ARRAY(str) aw=windows

int+ g_ed; g_ed+1; if(g_ed>ap.len) g_ed=1

run ap[g_ed-1]
wait 10 aw[g_ed-1]
key Cv
