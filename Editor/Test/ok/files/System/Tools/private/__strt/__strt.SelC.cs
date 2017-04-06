function$ $chars

 Gets character val(this) from chars. Empty if it is space or val(this) is invalid.
 Use with a combo/list box var, eg to append an option char, like lef+ or lef-.
 Returns this.

int i=val(s)
if(i<0 or i>=len(chars) or chars[i]=32) s=""; else s.get(chars i 1)
ret s
