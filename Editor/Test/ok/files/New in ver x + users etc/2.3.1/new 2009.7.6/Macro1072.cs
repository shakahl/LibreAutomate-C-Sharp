type DOUS double'k
type DOUT double'k
type INTP int*p

dll "qm.exe"
	DOUS'DouS
	DOUT'DouT
	INTP'IntP

DOUS a=DouS
out a
DOUT b=DouT
out b
long c=IntP
out c
