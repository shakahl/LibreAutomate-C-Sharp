 Gintaras: I don't use this. Or, would use Needs_WM_VSCROLL instead, but so far did not encountered windows that don't support WM_MOUSEWHEEL (XP).


 Needs_WM_MOUSEWHEEL:
    Determines whether the given windoow (w) needs a WM_MOUSEWHEEL message instead of WM_VSCROLL
 	- Mostly for Microsoft office programs
 	- Add more programs to the array as you need to
function w
int i mx(10)
ARRAY(str) name_ class_ prg_; name_.create(mx); class_.create(mx); prg_.create(mx)
 Tested & needed for:

name_[0]="Microsoft Excel -"; class_[0]="XLMAIN"; prg_[0]="EXCEL"		;; Word 2000 & 2002
name_[1]="- Microsoft Word"; class_[1]="OpusApp"; prg_[1]="WINWORD"		;; Excel 2000 & 2002
name_[2]="Microsoft PowerPoint -"; prg_[2]="POWERPNT"					;; PP2000: +PP9FrameClass, PP2002: +PP10FrameClass
name_[3]="- Microsoft Publisher"; class_[3]="MSWinPub"; prg_[3]="MSPUB"	;; Publisher 2000

 Tested & not needed for:
     Microsoft Access 2002, Outlook Express

for i 0 mx
	if(!(name_[i].len)) ret 0
	if(wintest(w name_[i] class_[i] prg_[i])) ret 1
