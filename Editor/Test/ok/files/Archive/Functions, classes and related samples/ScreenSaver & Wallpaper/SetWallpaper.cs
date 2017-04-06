 /
function $wpFile [style] ;;style: 0 center, 1 tile, 2 stretch, 3 fit (Win7), 4 fill (Win7)

 Sets desktop wallpaper.

 wpFile - bmp file.
   Error if does not exist or is not bmp (Vista/7 also supports jpg).
   Use "" to remove wallpaper.


if(!empty(wpFile))
	wpFile=_s.searchpath(wpFile)
	if(!wpFile) end ERR_FILE

lpstr rk="control panel\desktop"

 get current values to restore if fails
str _wp _tw _ws
rget _wp "wallpaper" rk; rget _tw "tilewallpaper" rk; rget _ws "wallpaperstyle" rk

 set style
lpstr rstyle("0") rtile("0")
sel style&255
	case 1 rtile="1"
	case 2 rstyle="2"
	case 3 if(_winver>=0x601) rstyle="6"
	case 4 if(_winver>=0x601) rstyle="10"
rset rstyle "wallpaperstyle" rk; rset rtile "tilewallpaper" rk

 set file
if !SystemParametersInfo(SPI_SETDESKWALLPAPER 0 wpFile 3)
	 when SPI fails, it removes wallpaper. Restore.
	rset _tw "tilewallpaper" rk; rset _ws "wallpaperstyle" rk
	SystemParametersInfo(SPI_SETDESKWALLPAPER 0 _wp 3)
	end ERR_FAILED
