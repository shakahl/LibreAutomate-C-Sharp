function$

 Replaces CSV ',' with '=' and calls SF(2).
 Returns this.

if(!s.len) s="''''"; ret s
ICsv c._create
c.FromString(s)
c.Separator="="
c.ToString(s)
s.rtrim("[]")
SF(2)
ret s
