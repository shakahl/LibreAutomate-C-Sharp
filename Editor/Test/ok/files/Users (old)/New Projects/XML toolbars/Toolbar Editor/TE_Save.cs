 /ToolbarEditor

int- htb hed
str s ss
ss.getwintext(hed)
if(!ss.len) ret
IXml xml=CreateXml
IXmlNode xt=xml.Add("toolbars").Add("tb1")
IXmlNode ne

 for(i SendMessage(htb TB_BUTTONCOUNT 0 0)-1 -1 -1) SendMessage htb TB_DELETEBUTTON i 0

int i
foreach s ss
	if(s="-")
		xt.Add("sep")
	else
		ne=xt.Add("b")
		ne.SetAttribute("id" _s.from(i+101))
	i+1

xml.ToString(s)
out s

int il=SendMessage(htb TB_GETIMAGELIST 0 0)
s.FromImageList(il)
s.setfile("$desktop$\il.txt")

 IMAGEINFO ii
 ImageList_GetImageInfo il 0 &ii
 SaveBitmap ii.hbmImage "$desktop$\il.bmp"
