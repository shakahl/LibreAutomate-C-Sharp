str filesToPrint=
 $qm$\HTMLHelp\Functions\IDP_CALL.html
 http://www.quickmacros.com/
 http://www.quickmacros.com/features.html
 http://www.yahoo.com

foreach _s filesToPrint
	SendHtmlToPrinter _s 2
