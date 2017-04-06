 typelib RC4Lib {186DB40A-BD43-11D1-94B1-0000B43369D3} 3.0
typelib RC4Lib "%com%\noUI\RC4Lib (crypt, dll)\RC4.dll"

 dll "C:\Documents and Settings\a\Desktop\New Components\RC4Lib (crypt, dll)\RC4.dll" #DllRegisterServer
 dll "C:\Documents and Settings\a\Desktop\New Components\RC4Lib (crypt, dll)\RC4.dll" #DllUnregisterServer
 out DllRegisterServer
 out DllUnregisterServer

RC4Lib.RC4 r._create
BSTR s("texttext") se sk("key") sd
se=r.AscEncrypt(&s &sk)
out se
sd=r.AscDecrypt(&se &sk)
out sd
