 /
function# $dd str&so [cbFunc] [cbParam]

 Takes text that contains dialog definition, and returns binary dialog template in so.
 Returns nonzero if successful.


if(findrx(dd __S_RX_DD 0 0 _s 1)<0) ret
ret __CompileDialogDefinition(_s &so cbFunc cbParam)

 this was public, don't know why.
