 /
function $printerName

 Sets default printer.

 REMARKS
 Calls Windows API function <help>SetDefaultPrinterW</help>. Error if fails.


if(!SetDefaultPrinterW(@printerName)) end "" 16
