function!

 Returns 1 if this variable is empty, 0 if not.
 Can be used for example after htm, when called without flag 32 (error if not found).

 Added in: QM 2.4.1.

 EXAMPLES
 Htm e=htm("..." "..." "" w "0")
 if(e.NotFound)
	 out "not found"

  the same
 Htm e=htm("..." "..." "" w "0")
 if(!e)
	 out "not found"

  example with flag 32
 Htm e=htm("..." "..." "" w "0" 0 0x20)
 err
	 out "not found"


ret !el
