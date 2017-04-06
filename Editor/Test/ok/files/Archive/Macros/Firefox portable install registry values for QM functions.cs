 /exe 2

 Acc.FindFF and some other QM functions don't work with portable Firefox, unless non-portable version is installed.
 To fix it, need to set some registry values that don't exist if Firefox is not installed. This macro does it.
 Note: in portable QM this macro probably will need UAC consent, because it must run as Administrator.

 Registry keys
str k1="Interface\{1814CEEB-49E2-407F-AF99-FA755A7D2607}\ProxyStubClsid32"
str k2="CLSID\{0D68D6D0-D93D-4D08-A30D-F00DD1F45B24}\InProcServer32"

 Is Firefox installed?
if rget(_s "" k1 HKEY_CLASSES_ROOT) and rget(_s "" k2 HKEY_CLASSES_ROOT) and dir(_s)
	end "The Firefox registry key already exists and QM Firefox functions should work. This macro could damage existing settings."

 We need AccessibleMarshal.dll path. It's in Firefox folder.
str ff="D:\PortableApps\FirefoxPortable\App\Firefox\AccessibleMarshal.dll"
int drive found; for(drive 'D' 'Z'+1) ff[0]=drive; if(dir(ff)) found=1; break
if(!found) end "Portable Firefox not found. Please edit the 'str ff...' line in this macro."

 Create Firefox registry keys and set values
if !rset("{0D68D6D0-D93D-4D08-A30D-F00DD1F45B24}" "" k1 HKEY_CLASSES_ROOT) or !rset(ff "" k2 HKEY_CLASSES_ROOT)
	end iif(IsUserAdmin ERR_FAILED ERR_ADMIN)
rset "Both" "ThreadingModel" k2
out "Fixed. QM Firefox functions now should work."

 BEGIN PROJECT
 main_function  Firefox portable install registry values for QM functions
 exe_file  $my qm$\FirefoxRegFix.qmm
 flags  6
 guid  {706CD11E-878C-453E-B1CD-69078872E126}
 END PROJECT
