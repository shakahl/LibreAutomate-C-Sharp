 /
function WTI*a n [~title]

 Shows text capturing results in QM output.


if(n<1) ret

if(title.len) out "<><Z 0xe080>%s</Z>" title

int i
for i 0 n
	WTI& t=a[i]
	str sc.getwinclass(t.hwnd) st.getwintext(t.hwnd) sw.format("%i %s ''%.15s''" t.hwnd sc st)
	out "<><c %i>%i. '%s'  rt={%i %i %i %i, FH=%i}   hwnd=[%s]   flags=0x%X   api=%i(0x%X)   bkColor=0x%X</c>" t.color i t.txt t.rt.left t.rt.top t.rt.right t.rt.bottom t.fontHeight sw t.flags t.api t.apiFlags t.bkColor
