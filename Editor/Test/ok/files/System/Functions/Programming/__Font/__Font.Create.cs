function $name [size] [style] [angle] [templateFont] [mask] ;;style: 1 bold, 2 italic, 4 underline, 8 strikeout.  templateFont: handle or: 1 dialog, 2 icon, 3 menu, 4 statusbar/tooltip, 5 messagebox, 6 caption, 7 smallcaption.  mask: 1 name, 2 size, 4 style, 8 angle, 0x102 size%.

 Creates font.

 name - font name. If empty, uses default dialog font name.
 size - font size. Can be 0 to use default size.
 style - font style flags.
 angle (QM 2.3.2) - angle, degrees. To draw text with angle, use function TextOutW. For example, if 90 or 270, draws text vertically.
 templateFont (QM 2.3.2) - font to be used as template. One of:
   Font handle. Can be another __Font variable.
   QM 2.3.3. One of standard Windows fonts: 1 dialog, 2 icon, 3 menu, 4 statusbar/tooltip, 5 messagebox, 6 caption, 7 smallcaption.
 mask (QM 2.3.2) - flags that specify what properties of the template font to change.

 See also: <DT_SetTextColor>.


LOGFONTW lf
int t=templateFont
if t
	if t=2
		SystemParametersInfoW(SPI_GETICONTITLELOGFONT sizeof(lf) &lf 0)
	else if t>=3 && t<=7
		NONCLIENTMETRICSW m.cbSize=sizeof(m)
		SystemParametersInfoW(SPI_GETNONCLIENTMETRICS m.cbSize &m 0)
		LOGFONTW* p
		sel(t) case 3 p=&m.lfMenuFont; case 4 p=&m.lfStatusFont; case 5 p=&m.lfMessageFont; case 6 p=&m.lfCaptionFont; case 7 p=&m.lfSmCaptionFont
		lf=*p
	else
		if(t=1) templateFont=_hfont
		GetObjectW(templateFont sizeof(lf) &lf)
else
	mask=0xff
	lf.lfCharSet=DEFAULT_CHARSET

if mask&1
	if(empty(name)) lstrcpyW &lf.lfFaceName L"MS Shell Dlg 2"
	else lstrcpynW &lf.lfFaceName @name LF_FACESIZE
if mask&2
	if size
		if(mask&0x100) lf.lfHeight=MulDiv(lf.lfHeight size 100)
		else int hdc=GetDC(0); lf.lfHeight=-MulDiv(size GetDeviceCaps(hdc LOGPIXELSY) 72); ReleaseDC 0 hdc
	else lf.lfHeight=0
if mask&4
	lf.lfWeight=iif(style&1 FW_BOLD FW_NORMAL) 
	lf.lfItalic=style&2!0
	lf.lfUnderline=style&4!0
	lf.lfStrikeOut=style&8!0
if mask&8
	lf.lfEscapement=angle*10
	lf.lfOrientation=lf.lfEscapement

if(handle) DeleteObject handle
handle=CreateFontIndirectW(&lf)
