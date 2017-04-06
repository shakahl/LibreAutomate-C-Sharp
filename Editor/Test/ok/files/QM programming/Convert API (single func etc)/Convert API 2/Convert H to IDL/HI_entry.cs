 Adds entry to dll function prototype.
 First of all, header must be preprocessed and all functio prototypes
 must begin:
 (space)
 TYPE
 FunctionNameA(...

 Move mouse to function name and press Ctrl+e.

dou
str s.getsel
str ss=s
ss.rtrim('A')
if(ss!=s) ss.setsel
ss.format("[entry(''%s'')][]" s)
key HU
ss.setsel


 dou
 str s.getsel
 str ss=s
 ss.rtrim('A')
 if(ss!=s) ss.setsel
 ss.format("[entry(''%s'')][]" s)
 wait 0 ML
 ss.setsel
