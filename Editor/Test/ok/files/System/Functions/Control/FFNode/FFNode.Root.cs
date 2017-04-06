function'FFNode [Acc&a]

 Returns root node. Optionally gets its accessible object.
 It is root of web page, frame, UI, or an UI part, depending on where this node is.

 a - variable that receives accessible object.


if(!node) end ERR_INIT

FFDOM.ISimpleDOMNode p(node) p2
rep
	p2=p.parentNode; err break
	if(!p2) break
	p=p2

if(&a) a.elem=0; a.a=+p; err end ERR_OBJECTGET
ret p
