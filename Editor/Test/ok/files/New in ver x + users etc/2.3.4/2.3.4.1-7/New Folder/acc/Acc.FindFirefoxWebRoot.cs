function hwnd

 Gets root object of current web page in Firefox.

FromWindow(hwnd OBJID_CLIENT)
VARIANT v=a.Navigate(0x1009 1)
a=v.pdispVal

err+ end _error
