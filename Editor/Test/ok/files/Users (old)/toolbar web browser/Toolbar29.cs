 /hook ToolbarExProc_TWWB2 /siz 500 300 /set 2 
Favorites :TbExpFolderMenuOpenInWebBrowser "menu favorites"
 Up :SHDocVw.WebBrowser b._getcontrol(child("" "ActiveX" val(_command))); str s=b.LocationURL; if(s.begi("file:///")) s.get(s 8); s.findreplace("/" "\"); s.getpath; out s; WebInQmActiveX s 0 val(_command)
Up :SHDocVw.WebBrowser b._getcontrol(child("" "ActiveX" val(_command))); str s=b.LocationURL; if(s.begi("file:///")) s.get(s 8); s.findreplace("/" "\"); s.getpath; out s; b.Navigate(s)
 Back :web "Back" 0 TriggerWindow; err **
 Back :WebInQmActiveX "Back" 0 val(_command);;often does not work, and QM may hang
Back :SHDocVw.WebBrowser b._getcontrol(child("" "ActiveX" val(_command))); b.GoBack
