 /
function $menuname [$tbname]

 Shows a menu and opens the selected file in web browser on a QM toolbar.
 The menu must have these properties (checked in Properties): 'don't run ...', 'expand file folders'.

 menuname - menu name.
 tbname - toolbar name. Can be omitted when this function is called from the toolbar.

 EXAMPLE toolbar
  /hook ToolbarExProc_TWWB /siz 500 300 /set 2 
 Favorites :TbExpFolderMenuOpenInWebBrowser "menu favorites"

 EXAMPLE menu ("menu favorites")
  /dontrun /expandfolders
 fav "$favorites$"


int h; str s
 get toolbar hwnd
if(len(tbname)) s=tbname; h=win(s.ucase "QM_toolbar")
else h=val(_command)
if(!h) ret
 show the menu and wait
if(!mac(menuname)) ret
 get selected path
qm.GetLastSelectedMenuItem 0 &s 0
 open in toolbar web browser
WebInQmActiveX s 0 h
