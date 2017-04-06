 /
function# ~&st ~vdOld ~vdNew

 Updates variable names in st after changing variable declarations vdOld to vdNew.
 Returns the number of variables with changed names.

 st - whole text, in/out.
 vdOld - old variable declarations.
 vdNew - new variable declarations.

 The declarations can be like:
   str controls="..."
   str e3 c4Nam ...
 or just the second line.


vdOld.getl(vdOld iif(vdOld.beg("str controls") 1 0))
vdOld.gett(vdOld 1 "" 2) ;;remove type
vdNew.getl(vdNew iif(vdNew.beg("str controls") 1 0))

str s
ARRAY(str) a
tok(vdNew a)

int i n
for i 1 a.len ;;for each new variable
	if(findrx(a[i] "(?<=\w)\d+" 1 0 s)<0) continue ;;get id from the new variable name, eg cb5Abc -> 5
	if(findrx(vdOld F"[A-Z_]+{s}[A-Z_]*" 0 3 s)<0 or s=a[i]) continue ;;in old declarations find variable with that id; ignore if same name
	st.findreplace(s a[i] 2)
	n+1

ret n

 note: fbc public.
