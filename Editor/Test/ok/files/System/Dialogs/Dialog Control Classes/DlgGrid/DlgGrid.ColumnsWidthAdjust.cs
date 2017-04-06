function [ncolumns] [rightMargin]

 Proportionally adjusts widths of columns to fit in client area.
 Use to autosize columns, or to avoid adding horizontal scrollbar.

 nColumns - number of columns that must fit in client area. If omitted or 0, adjusts for all columns.
 rightMargin - number of pixels to reserve in the right side.


int i w ww; double fa; ARRAY(int) a; RECT r

if(ncolumns<1) ncolumns=ColumnsCountGet

for i 0 ncolumns
	w=Send(LVM_GETCOLUMNWIDTH i 0)
	ww+=w
	a[]=w

GetClientRect(h &r)
r.right-rightMargin
if(!ww or ww=r.right or r.right<0) ret
fa=1.0*r.right/ww
for(i 0 ncolumns) Send(LVM_SETCOLUMNWIDTH i a[i]*fa)
