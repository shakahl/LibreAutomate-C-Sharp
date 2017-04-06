 /
function $wpFile [style] ;;style: 0 center, 1 tile, 2 stretch, 3 fit (Win7), 4 fill (Win7)

 Changes wallpaper.

 wpFile - image file.
   Tested only with .bmp.
   Empty string removes wallpaper.

 note: fails on some computers. Use SetWallpaper instead.


IActiveDesktop ad._create(CLSID_ActiveDesktop)
if(empty(wpFile)) _s=""; else _s.expandpath(wpFile)
ad.SetWallpaper(@_s 0)

style&0xff
if(style>=3 and _winver<0x601) style=0
WALLPAPEROPT o.dwSize=sizeof(o)
o.dwStyle=style
ad.SetWallpaperOptions(&o 0)

ad.ApplyChanges(AD_APPLY_ALL)

err+ end _error
