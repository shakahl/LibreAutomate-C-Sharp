 /
function activate [temporary] ;;activate: 0 deactivate, 1 activate

 Activates or deactivates screen saver.


SystemParametersInfo(SPI_SETSCREENSAVEACTIVE activate 0 iif(temporary 0 3))
