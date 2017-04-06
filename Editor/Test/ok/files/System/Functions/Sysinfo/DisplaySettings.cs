function! [colorbits] [xpixels] [ypixels]

 Changes display color depth and/or resolution.
 Returns: 1 success, 0 failed.

 colorbits - bits/pixel. Can be 4, 8, 16, 24 or 32.
 xpixels, ypixels - width and height.

 REMARKS
 You can set only some arguments, others set to 0.
 If all arguments are 0, then only shows available display settings in QM output.
 If needs to restart computer, shows message box and returns 0.

 EXAMPLES
 DisplaySettings ;;show available settings

 DisplaySettings(0 1024 768) ;;change display resolution

 DisplaySettings(8) ;;change color depth


DEVMODE d.dmSize=sizeof(d)
int i
for i 0 1000
	if(EnumDisplaySettings(0 i &d)=0) break
	if(colorbits=0 and xpixels=0 and ypixels=0)
		out "colorbits=%i  xpixels=%i  ypixels=%i" d.dmBitsPerPel d.dmPelsWidth d.dmPelsHeight
		continue
	if((colorbits=0 or colorbits=d.dmBitsPerPel) and (xpixels=0 or xpixels=d.dmPelsWidth) and (ypixels=0 or ypixels=d.dmPelsHeight))
		d.dmFields=0
		if(colorbits) d.dmFields|DM_BITSPERPEL
		if(xpixels) d.dmFields|DM_PELSWIDTH
		if(ypixels) d.dmFields|DM_PELSHEIGHT
		i=ChangeDisplaySettings(&d CDS_UPDATEREGISTRY)
		if(i=DISP_CHANGE_SUCCESSFUL) ret 1
		if(i=DISP_CHANGE_RESTART) mes "Display settings will be changed after you restart computer." "Display Settings" "i"
		ret
