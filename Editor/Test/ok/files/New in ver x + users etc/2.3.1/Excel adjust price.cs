 /exe 1

 The /exe 1 tells to run in separate process. It is needed on Vista/7, if UAC is on.
 Warning: When Excel sheet changed in this way, you cannot Undo. You should backup your workbook file or sheet.


 define prices
type PRICERANGE ^maxPrice ^percentAdd
ARRAY(PRICERANGE) ap; PRICERANGE& p
&p=ap[]; p.maxPrice=2.5; p.percentAdd=20 ;;0 - 2.5
&p=ap[]; p.maxPrice=10; p.percentAdd=18 ;;2.51 - 10
&p=ap[]; p.maxPrice=20; p.percentAdd=15 ;;10.01 - 20
&p=ap[]; p.maxPrice=0; p.percentAdd=11 ;;20.01 - oo
 You can change the above values.
 You can add more price ranges, or delete some. Prices must be in ascending order. The last price is not used and can be 0.

 connect to current Excel sheet and get all cells
ExcelSheet es.Init
ARRAY(str) a
es.GetCells(a)

 enumerate cells
int c r i
for r 0 a.len(2) ;;for each row
	for c 0 a.len(1) ;;for each column
		 here you can insert code to skip some columns. Example: sel(c) case [1,2] case else continue ;;skips columns other than B and C
		str& s=a[c r] ;;cell
		 out s
		if(findrx(s "^[\d,]+(\.\d{1,3})?$")<0) continue ;;is it currency format?
		double d=val(s 2) ;;need to convert to a numeric type to do calculations
		 out d
		for(i 0 ap.len-1) if(d<=ap[i].maxPrice) break ;;find range
		d+d*ap[i].percentAdd/100 ;;add %
		d=Round(d 2) ;;max 2 digits after .
		es.SetCell(d c+1 r+1) ;;replace cell

 BEGIN PROJECT
 main_function  Macro1278
 exe_file  $my qm$\Macro1277.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {19D2B851-FC63-4787-8D28-2883B22566DB}
 END PROJECT
