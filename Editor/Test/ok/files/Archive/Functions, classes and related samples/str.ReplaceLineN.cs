function# $replacement lineindex [nlines]

 Replaces specified line(s).
 Returns index of first character of lineindex-th line, or -1 if lineindex is too big.

 replacement - replacement string.
 lineindex - zero-based line index.
 nlines - number of lines to replace. Default or 0: 1 line.

 EXAMPLE
 str s="zero[]one[]two"
 s.ReplaceLineN("ONE" 1)
 out s


if(nlines<1) nlines=1
int i=findl(this lineindex)
if(i>=0)
	int j=findl(this+i nlines)
	if(j>=0) j-2; if(j<i or this[i+j]!=13) j+1
	this.replace(replacement i j)
ret i
