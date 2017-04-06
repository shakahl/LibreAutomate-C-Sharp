 Select recorded mou "..." command in QM editor, and run this macro.
 It replaces the selection to multiple mou- x y commands.
 Can be selected more than one line. Can contain other commands. For example you can select whole macro.


str s1.getsel s2 so
ARRAY(str) a
int r
foreach s2 s1
	if(findrx(s2 "^([[9],]*)mou ''(.+?)''$" 0 0 a)>=0)
		ConvertRecordedMouString a[2] so a[1]; err mes- "failed to convert" "" "x"
		r=1
	else
		so.addline(s2)

if(r) so.setsel
else mes- "There are no  mou ''...''  commands in the selected text." "" "!"
