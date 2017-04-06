function# $_file

 Loads imagelist from file.
 Returns imagelist handle, or 0 if failed.

 _file - .bmp file saved by the ImageList Editor.
   Supports <help #IDP_RESOURCES>macro resources</help> (QM 2.4.1) and exe resources.

 REMARKS
 The __ImageList variable can be used as standard Windows imagelist handle.
 The variable destroys the imagelist when dying.
 If don't need to auto-destroy, use function __ImageListLoad instead of __ImageList variable.

 See also: <__ImageList.Create>.


if(handle) ImageList_Destroy(handle)
handle=__ImageListLoad(_file)
ret handle
