 /
function# str&s [$text] [$caption] [$default] [x] [y]

 Similar to inp, but you can set position.


str controls = "0 3 4 5 6"
str d0 st3 e4 e5 e6
d0=iif(len(caption) caption "QM Input")
st3=text
e5=x
e6=y
if(!ShowDialog("inp2_dialog" &inp2_dialog &controls))
	s=default
	ret
s=e4
ret 1
