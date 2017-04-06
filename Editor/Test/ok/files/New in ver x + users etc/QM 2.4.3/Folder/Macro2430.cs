 /exe
out
 #exe addactivex "SHDocVw.WebBrowser"

 str s=
 <html><head></head><body></body

int w=win("Quick Macros - automation software for Windows. Macro program. Keyboard, mouse, record, toolbar - Internet Explorer" "IEFrame")
HtmlDoc d.SetOptions(2)
d.InitFromInternetExplorer(w)
out d.GetText

 BEGIN PROJECT
 main_function  Macro2430
 exe_file  $my qm$\Macro2430.qmm
 flags  6
 guid  {0CD19C2E-DFCA-4E9D-80A7-C44763301D48}
 END PROJECT
