 Acc a=acc("Text" "PUSHBUTTON" win("QM TOOLBAR" "QM_toolbar") "ToolbarWindow32" "" 0x1001)
 Acc a=acc("Katalogas" "LINK" win("Google - Mozilla Firefox" "MozillaUIWindowClass") "MozillaUIWindowClass" "" 0x1001)
Acc a=acc("Pagalba" "TEXT" win("Teksto vertimas - Windows Internet Explorer" "IEFrame") "Internet Explorer_Server" "" 0x1801 0x40 0x20000040)

typelib Accessibility {1EA4DBF0-3C3B-11CF-810C-00AA00389B71} 1.1
Accessibility.IAccIdentity ai=+a.a
lpstr m
ai.GetIdentityString(1 &m &_i)
outb m _i 1
out _i
CoTaskMemFree m

 01 00 00 80 CE 01 02 00 FC FF FF FF 01 00 00 00 

 The string is not constant, and therefore cannot be used in QM. Tested with a toolbar button. The string probably includes HWND.
 Objects in web pages don't support IAccIdentity. Tested with FF and IE.

