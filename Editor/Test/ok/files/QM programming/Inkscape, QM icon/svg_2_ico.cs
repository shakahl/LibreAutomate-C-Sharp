 /
function ~svgFile ~icoFile $sizes [flags] ;;flags: 1 use Iconscape command line

icoFile.expandpath
svgFile.expandpath
str pngFile.expandpath("$temp qm$\icon-%s.png")
str convertExe.expandpath("$program files$\ImageMagick-6.8.9-Q16\convert.exe")

if flags&1
	str inkscapeExe.expandpath("$program files$\Inkscape\inkscape.exe")
else ;;faster, and don't need to save
	spe 10
	int w1=act(win(" - Inkscape" "gdkWindowToplevel"))
	key CSe ;;Export Bitmap...
	int w2=wait(5 win("Export Bitmap (Shift+Ctrl+E)" "gdkWindowToplevel"))
	key Ap ;;Page

str size inkscapeCL convertCL=F"''{convertExe}''"
foreach size sizes
	_s.format(pngFile size)
	convertCL+F" ''{_s}''"
	
	if flags&1
		inkscapeCL=F"''{inkscapeExe}'' -z -e ''{_s}'' -w {size} -h {size} ''{svgFile}''"
		 out inkscapeCL
		RunConsole2 inkscapeCL _s ;;out _s
	else
		del- _s; err
		key Aw (size) ;;Width
		key Af (_s) ;;Filename
		key Ae ;;Export
		act w2

if(flags&1=0) clo w2

convertCL+F" ''{icoFile}''"
 out convertCL
RunConsole2 convertCL
