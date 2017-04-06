function# [$icons] [imageSize]

 Creates imagelist and optionally adds icons.
 Returns imagelist handle, or 0 if failed.

 icons - list of icon files. Example: "icon1.ico[]icons.dll,5".
 imageSize - size of images. Default 16.

 REMARKS
 The __ImageList variable can be used as standard Windows imagelist handle.
 The variable destroys the imagelist when dying.
 If don't need to auto-destroy, use function __ImageListFromIcons instead of __ImageList variable. Or __ImageListLoad with flag 2 (QM 2.4.3).

 Added in: QM 2.4.1.


if(handle) ImageList_Destroy(handle)
handle=__ImageListLoad(icons MakeInt(2 imageSize))
ret handle
