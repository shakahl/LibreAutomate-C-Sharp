 /
function hDlg $itemsHide iShow $it0 [$it1] [$it2] [$it3] [$it4] [$it5] [$it6] [$it7] [$it8] [$it9] [$it10] [$it11] [$it12] [$it13] [$it14] [$it15]

 Hides controls itemsHide. Then shows controls it[iShow], unless iShow is invalid, eg <0.
 items format - see <help>__IdStringParser.Parse</help>.

TO_Show hDlg itemsHide 0
if(iShow<0 or iShow>=getopt(nargs)-3) ret
lpstr* p=&it0
lpstr k=p[iShow]
if(!empty(k)) TO_Show hDlg k 1
