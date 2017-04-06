out
act id(2210 _hwndqm)
key CE
ScreenSaverRun "" 1
bee 200, 100
1
ifa("Quick") ret
3
 SystemParametersInfo(SPI_GETSCREENSAVERRUNNING, 0, &_i, 0); out _i
 zw win
bee 500, 100
if(!EnsureLoggedOn(1)) ret
bee 1000, 100
 key C
key ab
 SystemParametersInfo(SPI_GETSCREENSAVERRUNNING, 0, &_i, 0); out _i

#if 0

