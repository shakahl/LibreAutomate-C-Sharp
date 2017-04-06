 /dlg_html_editor
function! action ;;action: -1 init, 0 new, 1 open, 2 save, 3 save as, 4 exit

DHEDATA- t
str-- openfile ;;current file
int-- crc ;;CRC of saved HTML. Used to get dirty.
str sf sd

sel action
	case [0,1,4]
	if(DHE_GetHtml!=crc) ;;dirty
		sel mes("Save changes?" "" "YNC?")
			case 'Y'
			if(!DHE_OpenSave(2)) ret
			case 'C' ret
	case 2 if(!openfile.len) action=3

sel(action)
	case [1,3] if(!OpenSaveDialog(action=3 sf "Html files[]*.htm;*.html[]All files[]*" "htm")) ret
	case 4 ret 1

sel action
	case 0 ;;new
	sd="<HTML></HTML>"; sd.setwintext(t.hwb)
	openfile.all
	
	case 1 ;;open
	sd.getfile(sf); sd.setwintext(t.hwb)
	openfile=sf
	
	case [2,3] ;;save
	MSHTML.IHTMLDocument3 doc3=+t.doc
	sd=doc3.documentElement.outerHTML
	sd.setfile(iif(action=2 openfile sf))
	if(action=3) openfile=sf
	

crc=DHE_GetHtml

sel(action) case [-1,0,1,3] _s.from("QM HTML Editor - " openfile) _s.setwintext(t.hDlg)

sel action
	case [1,3] ;;add to recent
	 if(rget(sf

ret 1
err+ end _error
