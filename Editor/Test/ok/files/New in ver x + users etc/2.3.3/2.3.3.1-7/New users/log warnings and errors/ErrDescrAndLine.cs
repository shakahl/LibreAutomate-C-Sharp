 /
function$ $description [lineOffset]

 Gets previous code line and appends to description. To be used with end().

 description - error description.
 lineOffset - line offset from the line that calls this function. If 0 or omitted, uses -1 (previous line).

 REMARKS
 Uses " // " as separator.

 EXAMPLE
 if mes("Error?" "" "YN")='Y'
	 end ErrDescrAndLine("Error description.")


#if EXE
ret description
#else
if(!lineOffset) lineOffset=-1
if(Statement(1 lineOffset &_s)<0) ret description
str-- s.from(description " // " _s)
ret s