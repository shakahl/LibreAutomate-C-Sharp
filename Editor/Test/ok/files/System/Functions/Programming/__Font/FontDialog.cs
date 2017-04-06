function# str&fontString [hwndOwner] [CF_flags]

 Shows Font dialog and stores font properties into a string.
 The string later can be used with __Font.CreateFromString.
 Returns 1 on OK, 0 on Cancel.

 fontString - str variable that receives font string. On input - initializes dialog controls.
 hwndOwner - handle of owner window.
 CF_flags - additional <google>CHOOSEFONT</google> flags.


type ___LOGFONTQM :LOGFONTW'lf str'fontname

___LOGFONTQM f
CHOOSEFONTW cf.lStructSize=sizeof(cf)
cf.hwndOwner=hwndOwner
cf.lpLogFont=&f
cf.Flags=CF_SCREENFONTS|CF_flags
if fontString.len
	cf.Flags|=CF_INITTOLOGFONTSTRUCT
	fontString.setstruct(f 3)
	if(f.fontname.len) lstrcpynW &f.lf.lfFaceName @f.fontname LF_FACESIZE

if(!ChooseFontW(&cf)) ret

f.fontname.ansi(&f.lf.lfFaceName)
fontString.getstruct(f 1)
fontString.replacerx("(?s)lf\.lfFaceName.+(?=fontname)")

ret 1
