function! Excel.Worksheet&wIn $range `&what ARRAY(Excel.Range)&aFound flags

int lookIn lookAt searchOrder
sel(flags&0x300) case 0 lookIn=Excel.xlFormulas; case 0x100 lookIn=Excel.xlValues; case 0x200 lookIn=Excel.xlComments; case else end ERR_BADARG
lookAt=iif(flags&2 Excel.xlWhole Excel.xlPart)
searchOrder=iif(flags&16 Excel.xlByColumns Excel.xlByRows)

Excel.Range r rIn=__Range(range wIn)

r=rIn.Find(what @ lookIn lookAt searchOrder Excel.xlNext flags&1 0)
if(!r) ret

if(flags&4=0) aFound[]=r; ret 1

BSTR addr=r.Address(@ @ 1)
rep
	aFound[]=r
	 r=rIn.FindNext(r) ;;slower
	r=rIn.Find(what r lookIn lookAt searchOrder Excel.xlNext flags&1 0)
	if(!r or addr.cmp(r.Address(@ @ 1))=0) break

ret 1

err+ end _error

 notes:
 The loop will not end if contents of the first found cell changed while searching. But MSDN recommends this way.
 The SearchFormat parameter can be:
   VARIANT(VT_BOOL)=True - applies format defined in the Find Format dialog.
   Excel.CellFormat f=ws.Application.FindFormat - the same (containing properties defined in the dialog), but we can change properties. All properties must match, therefore we cannot set eg just number format.
 Don't need a Replace function. Too limited and rarely used.
