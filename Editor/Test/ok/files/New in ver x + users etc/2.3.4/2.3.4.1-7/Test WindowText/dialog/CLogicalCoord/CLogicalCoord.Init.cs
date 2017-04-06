function hdc X Y CX CY

 Sets rect of this. X Y CX CY are device coord.
 x y r b will be logical coord + getviewportorgex.
 ex ey will be getviewportextex.

m_hdc=hdc
SetRect &m_rd X Y X+CX Y+CY
Refresh
