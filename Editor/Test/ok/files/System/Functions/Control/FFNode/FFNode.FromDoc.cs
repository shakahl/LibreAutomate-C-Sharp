function hwnd [flags] ;;flags: 0x2000 window root, 0x10000 document always busy

 Gets root node of web page or window, and initializes this variable.

 hwnd - window handle.

 REMARKS
 Web page root matches DOCUMENT accessible object. Window root - APPLICATION accessible object.


if(!IsWindow(hwnd)) end ERR_HWND
Acc ad
if(flags&0x2000) AccessibleObjectFromWindow(hwnd OBJID_CLIENT uuidof(IAccessible) &ad.a)
else ad=acc("" "" hwnd "" "" flags&0x10000|0x2000)
if(!ad.a) end ERR_OBJECT
FromAcc(ad); err end _error
