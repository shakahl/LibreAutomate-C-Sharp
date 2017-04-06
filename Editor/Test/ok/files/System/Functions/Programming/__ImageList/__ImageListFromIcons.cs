 /
function# $icons [imageSize]

 Creates imagelist and adds icons.
 Returns imagelist handle. Later delete it with ImageList_Destroy (don't need if assigned to an __ImageList variable).

 icons - list of icons. Example: "icon1.ico[]icons.dll,5".
 imageSize - size of images. Default 16.


ret __ImageListLoad(icons MakeInt(2 imageSize))
