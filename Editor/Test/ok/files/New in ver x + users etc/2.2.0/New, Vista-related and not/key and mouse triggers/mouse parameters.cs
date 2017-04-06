 SystemParametersInfo SPI_SETDOUBLECLKWIDTH 4 0 3
 SystemParametersInfo SPI_SETDOUBLECLKHEIGHT 4 0 3

int x y t
Q &q
x=GetSystemMetrics(SM_CXDOUBLECLK)
y=GetSystemMetrics(SM_CYDOUBLECLK)
t=GetDoubleClickTime()
Q &qq
outq
out "%i %i %i" x y t
