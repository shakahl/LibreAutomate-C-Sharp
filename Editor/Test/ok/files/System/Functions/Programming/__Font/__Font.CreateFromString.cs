function ~fontstring

 Creates font from string created by FontDialog.


#compile FontDialog
___LOGFONTQM f
fontstring.setstruct(f 3)
if(f.fontname.len) lstrcpynW &f.lf.lfFaceName @f.fontname LF_FACESIZE

if(handle) DeleteObject handle
handle=CreateFontIndirectW(&f.lf)
