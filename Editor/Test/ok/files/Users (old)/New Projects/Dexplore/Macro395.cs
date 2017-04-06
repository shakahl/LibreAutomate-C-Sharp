
str namesp="ms.vistasdk.en"
str chdir="F:\Program Files\Microsoft SDKs\Windows\v6.0\Help\"
str hxcfile="WinSDK.hxc"
str descr="Vista SDK"

ChDir chdir
str cl.format("-n %s -c %s -d ''%s''" namesp hxcfile descr)
out run("C:\Program Files\Microsoft Help 2.0 SDK\hxreg.exe" cl "" "" 0x400)


