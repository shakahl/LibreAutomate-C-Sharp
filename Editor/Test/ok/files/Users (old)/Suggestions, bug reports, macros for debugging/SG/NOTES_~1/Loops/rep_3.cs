 100% processor usage with certain rep loops
 ===========================================
 I dont know if this should concern you, but a block
  such as the following uses 100% of processer time, on my P1 & P2 computers

 Possible scenero:
   someone, instead of using "wait for key" command, may use this instead:

 
rep
	ifk(F2) out "ok"
	
	
 Also: What practical use is there of ifk()?? Need some solid examples

From Qm documentation:
ifk(F2) break                if key F2 pressed, exit for or rep loop
ifk(K 1) key K               if key CapsLock toggled, press CapsLock

5) You cannot stop standalone running function manually, unless function contains special code, e.g. ifk(C.) break (if Ctrl+. pressed, exit for or rep loop).

 Update: Ignore this question for now, i have a good example, and many notes regarding wait, ifk etc.
 Will send you as soon as i get them analyzed & organized 