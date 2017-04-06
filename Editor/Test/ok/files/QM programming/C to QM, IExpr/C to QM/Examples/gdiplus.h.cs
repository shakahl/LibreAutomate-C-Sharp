out
str h="C:\Program Files\Microsoft SDKs\Windows\v7.0\Include\gdiplus.h"
str incl="C:\Program Files\Microsoft SDKs\Windows\v7.0\Include"
str t="$qm$\gdiplus.txt"
 str t="$desktop$\gdiplus.txt"
ConvertCtoQM h t incl "" 4|64|128 "$qm$\winapiqmaz_fdn.txt" "$qm$\winapiqmaz_fan.txt" "$qm$\winapiv_pch.txt"
 1
 run t

 dll 7061, type 2527, interface 569, def 29921, guid 972, typedef 4554, callback 278, added 1624
