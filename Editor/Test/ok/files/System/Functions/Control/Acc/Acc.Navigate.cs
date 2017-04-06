function $navig [Acc&result] ;;navig string components: Up, Down, LEft, Right, Next, PRevious, PArent, First, LAst, Child.

 Gets adjacent object.
 Stores result into this or other variable.

 navig - navigation string, as with <help>acc</help>.
 result - Acc variable that receives the object. Default: this variable (for which is called this function).

 REMARKS
 To create navig string, use 'Find accessible object' dialog.


if(!a) end ERR_INIT
if(&result)
	result=acc(this navig)
	if(!result.a) end ERR_OBJECT
else
	Acc a1=acc(this navig)
	if(!a1.a) end ERR_OBJECT
	this=a1
err+ end ERR_BADARG
