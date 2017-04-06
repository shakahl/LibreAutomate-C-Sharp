function $rangeFrom $rangeTo [flags] ;;flags: 1 move (cut)

 Copies or moves from one range to other. Works like Excel Copy/Paste.

 rangeFrom - cell, row, column or other range to copy from. <help>Excel range strings</help>.
 rangeTo - range to copy to. Can be of different size than rangeFrom.

 Added in: QM 2.3.3.
 Errors: Excel errors


WS

Excel.Range rf rt
rf=__Range(rangeFrom)
rt=__Range(rangeTo)

if(flags&1) rf.Cut(rt)
else rf.Copy(rt)

err+ E
