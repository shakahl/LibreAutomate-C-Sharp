out
int w1=act(win("Quick Macros - ok - [Macro1886]" "QM_Editor"))

 wait 30 WT w1 "Quick Macros - ok - [ICsv ToArray]"
 wait 30 WT w1 "[ICsv ToArray]"
 wait 30 WT w1 "[iCsv ToArray]" 4

 wait 30 WT w1 "*[ICsv ToArray]" 1
 wait 30 WT w1 "*[iCsv ToArray]" 1|4

 wait 30 WT w1 ".+ICsv ToArray\]$" 2
wait 30 WT w1 ".+iCsv ToArray\]$" 2|4


 wait 30 -WT w1 "[Macro1886]"

out 1
