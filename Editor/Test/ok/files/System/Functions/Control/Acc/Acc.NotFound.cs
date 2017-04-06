function!

 Returns 1 if this variable is empty, 0 if not.
 Can be used for example after Find, when called without flag 0x1000 (error if not found).

 Added in: QM 2.4.1.

 EXAMPLES
 Acc a.Find(w "...")
 if(a.NotFound)
 	out "not found"

  the same
 Acc a.Find(w "...")
 if(!a.a)
 	out "not found"

  example with flag 0x1000
 Acc a.Find(w "..." "" "" 0x1000)
 err
 	out "not found"


ret !a
