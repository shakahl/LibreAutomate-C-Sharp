function [x] [y] [width] [height] [RECT&r]

 Sets rectangle to be used with other functions.
 The functions will get text only from the rectangle, not from whole target window.
 The rectangle must be in target window client area coordinates.
 If called without arguments, sets to not use a rectangle (default), ie get text from whole target window.
 You can specify the rectangle either with the first 4 arguments, or pass a RECT variable as 5-th argument.


if getopt(nargs)=0
	m_rp=0
else
	if(&r) m_r=r; else SetRect &m_r x y x+width y+height
	if(IsRectEmpty(&m_r)) end ERR_BADARG
	m_rp=&m_r
