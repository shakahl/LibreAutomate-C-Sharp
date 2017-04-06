;Get the filename from the Password dialog
Acc a=acc("" "STATICTEXT" win("Password" "bosa_sdm_msword") "bosa_sdm_msword" "" 0x1001)
out a.Name
 int w=win("Password" "bosa_sdm_Microsoft Office Word 11.0")
 Acc a1=acc("" "STATICTEXT" w "bosa_sdm_Microsoft Office Word 11.0" "" 0x1001)
 out a1.Name
