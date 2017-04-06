 \
function# $text [$tempitemname] [hwndowner] [$folder]

 Shows toolbar created at run time.

 text - menu text. Same as in usual popup menus.
 tempitemname - name of temporary toolbar. Default: "temp_toolbar". 
 	If toolbar with this name already exists, it is closed.
 hwndowner - owner window handle.
 folder - if set toolbar will be saved in custom folder

 Returns toolbar window handle.

 EXAMPLE
 *note:
  you need a vaild folder path

 int hwnd=win(mouse)
 int icon=GetWindowIcon(hwnd)
 str code
 code.format(" /mov %i %i /siz 44 44 /isiz 32 32 /hook _thumb_min /ini ''$my qm$\Toolbars\thumb_min.ini''" xm ym)
 code.formata("[]huhu :out ''huhu'' *%i" icon)
 DynamicToolbar22(code "thumb_min" 0 "\mouse\rightclick\functions\tmp\")
 DestroyIcon(icon)

if(!len(tempitemname)) tempitemname="temp_toolbar"
if(!len(folder)) folder="\User\Temp"
str cmd; if(hwndowner) cmd=hwndowner
ret mac(newitem(tempitemname text "Toolbar" "" folder 1) cmd)
