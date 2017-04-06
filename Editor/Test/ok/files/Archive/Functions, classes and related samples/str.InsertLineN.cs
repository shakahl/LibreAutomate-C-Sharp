function# $insertstring lineindex

 Inserts line.
 Returns index of first character of lineindex-th line, or -1 if lineindex is too big.

 insertstring - string to be inserted. Does not have to end with new line.
 lineindex - zero-based line index where to insert insertstring.

 EXAMPLE
 str s="zero[]one[]two"
 s.InsertLineN("" 1) ;;inserts empty line
 out s


int i=findl(this lineindex)
if(i<0 and numlines(this)=lineindex) this+"[]"; i=this.len
if(i>=0)
	str s.from(insertstring "[]")
	this.insert(s i s.len)
ret i
