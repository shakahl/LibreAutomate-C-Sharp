 removes y pixels from the top of the work area. Run with y=0 to restore.

int y=60

 ---------

key Wd
1
RECT r
SystemParametersInfo(SPI_GETWORKAREA 0 &r 0)
r.top=y
SystemParametersInfo(SPI_SETWORKAREA 0 &r 2)
1
key Wd
