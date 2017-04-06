SystemParametersInfo(SPI_GETMENUSHOWDELAY 0 &_i 0)
out _i

SystemParametersInfo(SPI_SETMENUSHOWDELAY 400 0 SPIF_SENDCHANGE)

SystemParametersInfo(SPI_GETMENUSHOWDELAY 0 &_i 0)
out _i
