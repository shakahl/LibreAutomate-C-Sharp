 /
function ^timeout [temporary]

 Sets screen saver timeout, in minutes.


SystemParametersInfo(SPI_SETSCREENSAVETIMEOUT timeout*60 0 iif(temporary 0 3))
ScreenSaverActivate 1 ;;on Vista, setting timeout deactivates screen saver
