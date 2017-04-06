\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str ax3SHD
ax3SHD=
 <div style="text-align: center; margin-left: auto; visibility:visible; margin-right: auto; width:450px;"> <object width="435" height="550"> <param name="movie" value="http://www.myspaceplaylists.com/mc/SearchPlayerApp.swf"></param> <param name="flashvars" value="mywidth=435&amp;myheight=550&amp;playlist_baseurl=http%3A%2F%2Fwww.myspaceplaylists.com%2Floadplaylist.php%3Fe%3D1%26playlist%3D&amp;;playlist_id=404926232&amp;wid=vos"></param></param> <param name="wmode" value="transparent"></param> <embed style="width:435px; visibility:visible; height:550px;" src="http://www.myspaceplaylists.com/mc/SearchPlayerApp.swf" width="435" height="550" name="mp3player" wmode="transparent" type="application/x-shockwave-flash" border="0" flashvars="mywidth=435&amp;myheight=550&amp;playlist_baseurl=http%3A%2F%2Fwww.myspaceplaylists.com%2Floadplaylist.php%3Fe%3D1%26playlist%3D&amp;playlist_id=404926232&amp;wid=vos"/> </object> <br/> <a href="http://www.mysocialgroup.com/searchbeta/videos#video"><img src="http://www.myspaceplaylists.com/mc/images/create_video_black.jpg" border="0"></a> <a href="http://www.playlist.com/playlist/404926232/standalone" target="_blank"><img src="http://www.myspaceplaylists.com/mc/images/popout_black.jpg" border="0"></a> <a href="http://www.mysocialgroup.com"><img src="http://www.myspaceplaylists.com/mc/images/pl-logo-ico-black.jpg" border="0"></a> </div>
 <div><a href="about:blank">Stop</a></div>

if(!ShowDialog("dlg_playlists_com" &dlg_playlists_com &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 344 393 "Dialog"
 1 Button 0x54030001 0x4 118 378 48 14 "OK"
 2 Button 0x54030000 0x4 168 378 48 14 "Cancel"
 3 ActiveX 0x54030000 0x0 0 0 344 374 "SHDocVw.WebBrowser"
 4 Button 0x54032000 0x0 0 379 48 14 "Stop"
 END DIALOG
 DIALOG EDITOR: "" 0x2030109 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 4
	str emp.setwintext(id(3 hDlg))
	case IDOK
	case IDCANCEL
ret 1
