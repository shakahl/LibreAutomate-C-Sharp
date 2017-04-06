 /
function# $icons [imageSize]

 Creates imagelist and adds icons.
 Returns imagelist handle. Later delete it with ImageList_Destroy (don't need if assigned to an __ImageList variable).

 icons - list of icons. Example: "icon1.ico[]icons.dll,5".
 imageSize - size of images. Default 16.


if(!imageSize) imageSize=16
int hi il=ImageList_Create(imageSize imageSize ILC_MASK|ILC_COLOR32 0 10)
imageSize<<16
foreach _s icons
	hi=iif(_s.len GetFileIcon(_s 0 imageSize) 0)
	ImageList_ReplaceIcon il -1 iif(hi hi _dialogicon) ;;TODO: add empty icon
	if(hi) DestroyIcon hi
ret il
