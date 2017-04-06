QM description 45 :outp sub.Get(45)
QM description 80 :outp sub.Get(80)
QM description 250 :outp sub.Get(250)
QM description 450 :outp sub.Get(450)
QM description 750 (584) :outp sub.Get(750)
QM description 1000 :outp sub.Get(1000)
QM description 2000 (1117) :outp sub.Get(2000)
-
Old 750 :outp sub.Get(751)
-
Keywords :"macros, macro, macro recorder, automate, automation, toolbar, record, key, keyboard, text, mouse, run, launch, window, file, schedule, menu, system, utility, programming, function, variable, exe"


#sub Get
function~ n

 Gets text of n characters from "QM description2".


str s1 s2
s1.getmacro("QM description2")
if(findrx(s1 F"(?ms)^\[{n}[](.+?)[]\]" 0 0 s2 1)<0) end "failed"
ret s2
