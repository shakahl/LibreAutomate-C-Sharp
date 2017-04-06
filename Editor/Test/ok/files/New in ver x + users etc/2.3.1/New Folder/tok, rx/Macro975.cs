out
str subject="''pns1'', ''pn, s2'': goto xxx"
out subject
str pattern="^(''(?C)(?:[^'']+)''[\s,]*)+"
ARRAY(str) a

FINDRX r.fcallout=&callout
r.paramc=&a
int i=findrx(subject pattern &r)

 REPLACERX r.fcallout=&callout
 r.paramr=&a
 subject.replacerx(pattern r)
