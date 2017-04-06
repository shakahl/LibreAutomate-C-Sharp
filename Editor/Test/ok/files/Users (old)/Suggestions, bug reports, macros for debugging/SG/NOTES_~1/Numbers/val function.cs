 Expressions inside val() not allowed?
 =====================================
str w="123,456"
str v

 out val( (v.gett(w -1 ",")) )
 - wont work this way

 - although this will:

v.gett(w -1 ",")
out val(v)
