str folder="$desktop$\excel pictures"
mkdir folder

ExcelSheet es.Init
Excel.Picture p
foreach p es.ws.Pictures
	_s=p.Name ;;out _s
	p.CopyPicture(Excel.xlScreen Excel.xlBitmap) ;;copy to clipboard
	_s=F"{folder}\{_s}.bmp"
	_s.getclip(CF_BITMAP)

run folder
