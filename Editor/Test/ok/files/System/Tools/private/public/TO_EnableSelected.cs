 /
function hDlg $itemsDisable iEnable $it0 [$it1] [$it2] [$it3] [$it4] [$it5] [$it6] [$it7] [$it8] [$it9] [$it10] [$it11] [$it12] [$it13] [$it14] [$it15]

 Disables controls itemsDisable. Then enables controls it[iEnable], unless iEnable is invalid, eg <0.
 items format - see <help>__IdStringParser.Parse</help>.

TO_Enable hDlg itemsDisable 0
if(iEnable<0 or iEnable>=getopt(nargs)-3) ret
lpstr* p=&it0
lpstr k=p[iEnable]
if(!empty(k)) TO_Enable hDlg k 1
