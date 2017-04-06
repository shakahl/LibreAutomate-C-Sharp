function$ hwnd [$prefix]

 Formats string with window handle, class and text (first line).

 Added in: QM 2.3.4.


if(!hwnd) this.format("%s0" prefix); ret this
if(!IsWindow(hwnd)) this.format("%s%i <invalid handle>" prefix hwnd); ret this
str s.getwinclass(hwnd); err
str ss.getwintext(hwnd); err
ss.trim; ss.getl(ss 0)
this.format("%s%i %s ''%s''" prefix hwnd s ss)
ret this
