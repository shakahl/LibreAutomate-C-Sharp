 /
function# x y width height [$saveToBmpFile] [int&getHbitmap]

 Captures a rectangle region on the screen.
 Returns: 1 success, 0 failed.

 x, y, width, height - rectangle coordinates.
 saveToBmpFile - will save to this bmp file. Use "" if don't want to save.
 getHbitmap - int variable that receives bitmap handle. Use 0 if not needed. Later call DeleteObject.

 REMARKS
 Unlike <help>CaptureImageOrColor</help>, this function immediately captures the specified rectangle and does not interact with the user.
 Can save the captured image to a bmp file, or get bitmap handle, or store in the clipboard.
 If saveToBmpFile and getHbitmap not used, stores in the clipboard.

 EXAMPLE
 CaptureImageOnScreen 100 100 200 200


__MemBmp b
if(!b.Create(width height 1 x y)) ret

int r(1) action

if(!empty(saveToBmpFile)) ;;save
	action|1
	r=SaveBitmap(b.bm saveToBmpFile)
	
if(&getHbitmap and r) ;;get HBITMAP
	action|2
	getHbitmap=b.Detach
	
if(!action) ;;store to the clipboard
	r=OpenClipboard(_hwndqm)
	if(r) EmptyClipboard; r=SetClipboardData(CF_BITMAP b.Detach)!0; CloseClipboard

ret r
