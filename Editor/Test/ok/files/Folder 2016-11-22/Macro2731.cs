SystemParametersInfo(SPI_GETMOUSEHOVERTIME 0 &_i 0)
out _i

SystemParametersInfo(SPI_SETMOUSEHOVERTIME 400 0 SPIF_SENDCHANGE)

SystemParametersInfo(SPI_GETMOUSEHOVERTIME 0 &_i 0)
out _i
