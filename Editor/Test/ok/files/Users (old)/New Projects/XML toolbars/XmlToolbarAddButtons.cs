 /Macro885
function# htb $xml $tbpath

IXml _xml=CreateXml
ARRAY(TBBUTTON) a
str si st; lpstr s
IXmlNode xt x
int i il ilold niold hi

if(empty(xml)) ret
if(xml[0]='<') _xml.FromString(xml); else _xml.FromFile(xml)
err+ ret

xt=_xml.Path(tbpath); if(!xt) ret

 if already has imagelist, add to it
ilold=SendMessage(htb TB_GETIMAGELIST 0 0)
if(ilold) niold=ImageList_GetImageCount(ilold)

x=xt.FirstChild
rep
	if(!x) break
	TBBUTTON& b=a[]
	sel(x.Name)
		case "b"
		b.idCommand=val(x.AttributeValue("id"))
		s=x.AttributeValue("state"); b.fsState=iif(s val(s) TBSTATE_ENABLED)
		s=x.AttributeValue("style"); if(s) b.fsStyle=val(s)
		 s=x.Value; if(s) st.fromn(st st.len s len(s)+1); b.iString=1
		case "sep"
		b.fsStyle=TBSTYLE_SEP
	s=x.AttributeValue("i"); b.iBitmap=iif(s val(s)+niold I_IMAGENONE)
	 next
	x=x.Next

s=xt.AttributeValue("imagelist")
if(s)
	x=_xml.Path(s); if(!x) ret
	si=x.Value
	il=si.ToImageList
	if(!ilold) SendMessage(htb TB_SETIMAGELIST 0 il)
	else
		for i 0 ImageList_GetImageCount(il)
			hi=ImageList_GetIcon(il i 0)
			ImageList_ReplaceIcon ilold -1 hi
			DestroyIcon hi
		 note: this is slow. How to append imagelist to imagelist faster?

s=xt.AttributeValue("style"); if(s) SetWinStyle(htb val(s))
s=xt.AttributeValue("exstyle"); if(s) SendMessage(htb TB_SETEXTENDEDSTYLE 0 val(s))

 if(st.len) ;;convert strings to Unicode
	 st.unicode(st _unicode st.len)
	 word* w=st
	 for i 0 a.len
		 if(!a[i].iString) continue
		 a[i].iString=w; w+(len(w)+1)*2

SendMessage(htb TB_BUTTONSTRUCTSIZE sizeof(TBBUTTON) 0)
SendMessage(htb TB_ADDBUTTONSW a.len &a[0])
 SetWindowPos(SendMessage(htb TB_GETTOOLTIPS 0 0) HWND_TOPMOST 0 0 0 0 SWP_NOSIZE|SWP_NOMOVE|SWP_NOACTIVATE|SWP_NOOWNERZORDER)

ret 1
//notes:
//Before destroying the toolbar, the caller must call ImageList_Destroy(SendMessage(htb, TB_SETIMAGELIST, 0, 0)). Not necessary if the process then exits.
