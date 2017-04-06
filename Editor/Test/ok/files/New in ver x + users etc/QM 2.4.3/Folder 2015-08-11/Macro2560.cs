 outx _iever ;;0xB00 = 11000
if(!rset(11000 "qm.exe" "SOFTWARE\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION" HKEY_LOCAL_MACHINE)) end "failed, run QM as administrator"
