out
int w=win("dlg_apihook" "#32770")

typelib TextGRABSDKLib {CA755C94-1DB9-4540-8C4C-2F05CB8D4E36} 1.0
TextGRABSDKLib.TextGRABSDK x._create
x.SetLicense("" "")
 TextGRABSDKLib.

 BSTR b
 x.CaptureFromHWND(w b)
 out b

TextGRABSDKLib.TextSnapshot ts
x.CaptureTextSnapshot(w ts)
TextGRABSDKLib.ITextItem t
foreach t ts
	out t.text
	 t.CharactersWidth
	 outw t.Window
	tagRECT r=t.Bounds
	zRECT +&r
	