 /
function^

 Returns screen saver timeout, in minutes.


if(SystemParametersInfo(SPI_GETSCREENSAVETIMEOUT 0 &_i 0))
	ret _i/60.0
