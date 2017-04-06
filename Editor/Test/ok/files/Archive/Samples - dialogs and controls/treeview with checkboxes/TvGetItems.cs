 /
function htv ARRAY(int)&ahi [ARRAY(int)&apar] [flags] ;;flags: 1 only checked

 Gets handles and/or lParams of tree view items, only those at root.

if(&ahi) ahi.redim
if(&apar) apar.redim
int hi=SendMessage(htv TVM_GETNEXTITEM TVGN_ROOT 0)
rep
	if(!hi) break
	if(flags&1 and !TvIsChecked(htv hi)) goto next
	if(&ahi) ahi[]=hi
	if(&apar) apar[]=TvGetParam(htv hi)
	 next
	hi=SendMessage(htv TVM_GETNEXTITEM TVGN_NEXT hi)
