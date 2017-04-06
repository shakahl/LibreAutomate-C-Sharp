rset 1 "qm.exe" "Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_WEBSOCKET"
rset 1 "qm.exe" "Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_WEBSOCKET" HKEY_LOCAL_MACHINE

str wow64=iif(_win64 "\Wow6432Node" "")
rset 1 "qm.exe" F"Software{wow64}\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_WEBSOCKET"
rset 1 "qm.exe" F"Software{wow64}\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_WEBSOCKET" HKEY_LOCAL_MACHINE
