 run "control.exe" "/name Microsoft.Personalization" ;;canonical name

 RunConsole2 "control.exe /?"

IOpenControlPanel o._create(CLSID_OpenControlPanel)
 o.Open(@"Microsoft.InternetOptions" @"Programs" 0) ;;does not select page
 o.Open(@"Microsoft.Keyboard" @"Hardware" 0) ;;does not select page
 o.Open(@"Microsoft.Keyboard" @"1" 0) ;;selects second page
 o.Open(@"Microsoft.InternetOptions" @"2" 0) ;;selects third page

BSTR b.alloc(3000)
 o.GetPath(@"Microsoft.InternetOptions" b.pstr 3000) ;;fails
o.GetPath(@"Microsoft.Keyboard" b.pstr 3000) ;;fails
 o.GetPath(0 b.pstr 3000) ;;::{26EE0668-A00A-44D7-9371-BEB064C98683}
out b
