function `hwnd

 Gets root object of current web page in Firefox.


#if QMVER<0x2030400
Find(hwnd "" "" "" 0x3000 0 0 "parent")
#else
Find(hwnd "" "" "" 0x3000)
#endif

err+ end _error
