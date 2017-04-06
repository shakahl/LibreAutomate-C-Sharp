function# lineindex [nlines]

 Removes specified line(s).
 Returns index of first character of lineindex-th line, or -1 if lineindex is too big.

 lineindex - zero-based line index.
 nlines - number of lines to remove. Default or 0: 1 line.

 EXAMPLE
 str s="zero[]one[]two"
 s.RemoveLineN(1)
 out s


if(nlines<1) nlines=1
int i=findl(this lineindex)
if(i>=0) this.remove(i findl(this+i nlines))
ret i
