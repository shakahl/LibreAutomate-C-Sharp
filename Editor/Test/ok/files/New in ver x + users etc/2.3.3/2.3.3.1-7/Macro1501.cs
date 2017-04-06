out
str s.getclip

 s=
 qmitem
 	int qmitem([name|iid] [flags] [QMITEM&qi] [mask])
 	 ;;flags: 1 skipfolders, 2 skipshared, 4 skipencrypted, 8 skipdisabled, 16 skipwithouttrigger, 32 skipmemberf, 64 skiplink;  mask: 1 name, 2 trigger, 4 programs, 8 text, 16 folderid, 32 filter, 64 descr, 128 datemod
 
 getopt
 	int getopt(option [context])
 	 ;;option: speed, itemid, nargs, nthreads, hidden, err, waitmsg, end, clip, waitcpu, slowmouse, slowkeys, keymark, keysync, hungwindow, nowarnings;   context: 0 this, 1 caller, 2 callback|thread entry, 3 thread entry, 4 global, 5 any
 

ARRAY(str) a; int i
if(!findrx(s "^.+[](\t.+[])+" 0 4|8|16 a)) ret
a.sort
s=a
s.setclip

