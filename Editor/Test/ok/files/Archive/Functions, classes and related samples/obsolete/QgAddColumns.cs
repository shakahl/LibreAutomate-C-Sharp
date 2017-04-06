 /
function hgrid $ctext [$cwidth] [$ctype] ;;ctype: 0 edit (def), 1 combo, 2 check, 7 read-only, 8 multiline text, 9 sorted combo, + 16 to add button

 Adds columns to QM_Grid control.

 ctext - list of labels.
 cwidth - list of widths.
 ctype - list of column type flags. Must be literal numbers, without operators. For example, 17 is combo with button. For edit can be empty line.


ref GRID

int i w(100)
ARRAY(str) at(ctext) aw(cwidth) ac(ctype)

for i 0 at.len
	if(i<aw.len) w=val(aw[i])
	LvAddCol2(hgrid i at[i] w)

for i 0 at.len
	if(i=ac.len) break
	w=val(ac[i])
	if(w) SendMessage hgrid LVM_QG_SETCOLUMNTYPE i w
