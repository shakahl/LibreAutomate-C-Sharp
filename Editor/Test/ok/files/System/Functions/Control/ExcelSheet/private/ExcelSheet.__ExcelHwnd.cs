function# Excel.Application'a

IDispatch _a=a
ret _a.Hwnd; err

 Old Excel (<2002) does not have Hwnd().
 Change Excel caption, or may find wrong window. http://support.microsoft.com/kb/302295
lock excelHwnd "QM_mutex_ExcelSheet.__ExcelHwnd"
a.Caption=F"Microsoft Excel - QM {GetCurrentThreadId}"
_s=a.Caption
_i=win(_s "XLMAIN")
BSTR _b; a.Caption=_b ;;restores normal
ret _i

err+ end _error

 IOleWindow ow=+app ;;error no interface
 ow.GetWindow(_i)
