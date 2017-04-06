 /dlg_apihook
function hdc CLogicalCoord&c

SetBkMode hdc TRANSPARENT
int LH=30*c.ey

str s2("abcd Ԏݭῼ")
 goto g1

DAH_UniscribeSSAnalyseSSOut hdc "CALL SSAnalyse/SSOut" c; c.Offset(200 LH)
 ret
DAH_UniscribeSSAnalyseSSOut hdc s2 c 2; c.Offset(-200 LH)

 ANSI
DAH_UniscribeSSAnalyseSSOut hdc "CALL SSA ANSI" c 1; c.Offset(0 LH)
str sa="αβγ"; sa.ConvertEncoding(CP_UTF8 1253)
DAH_UniscribeSSAnalyseSSOut hdc sa c 1 GREEK_CHARSET; c.Offset(0 LH)

 g1
DAH_UniscribeSItemizeSShapeSTextOut hdc "CALL SItemize/SShape/STextOut" c; c.Offset(200 LH)
DAH_UniscribeSItemizeSShapeSTextOut hdc s2 c; c.Offset(-200 LH)

DAH_UniscribeOpenType hdc "CALL OpenType" c; c.Offset(200 LH)
DAH_UniscribeOpenType hdc s2 c; c.Offset(-200 LH)
