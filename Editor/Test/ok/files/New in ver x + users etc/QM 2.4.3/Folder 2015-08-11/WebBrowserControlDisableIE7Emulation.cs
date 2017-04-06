 /

 Ensures that web browser controls of this program (QM or your QM-created exe) can use features of current Internet Explorer version (CSS3, HTML5 etc).

 REMARKS
 By default, web browser controls work in compatibility mode with Internet Explorer 7. IE 8/9/10/11 features are unavailable.
 This function sets this registry value, if it is missing or different:
   HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION:<ThisProgram.exe>=<CurrentInternetExplorerVersion>
 Error if this process is not running as administrator.
 You can call this function before ShowDialog if the dialog contains an ActiveX control SHDocVw.WebBrowser. Or call it once.


if(_iever<8) ret
str e.getfilename(ExeFullPath 1)
 out e
int t v=_iever>>8*1000
 out v
lpstr k="SOFTWARE\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION"
if(rget(t e k HKEY_LOCAL_MACHINE) and t=v) ret
 out t
if(!rset(v e k HKEY_LOCAL_MACHINE)) end ERR_ADMIN
