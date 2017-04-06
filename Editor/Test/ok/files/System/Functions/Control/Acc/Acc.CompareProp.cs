function! $role [$name] [$prop] [flags] [*cbFunc] [cbParam] ;;flags: 1 use *, 2 regexp, 4 use * in value/descr, 8 regexp value/descr, 16 +invisible, 32 +useless, 64 direct child

 Compares various properties of this object.
 Returns 1 if match, 0 if not.

 REMARKS
 This function is similar to <help>Acc.Find</help>. It calls <help>acctest</help>.

 Added in: QM 2.3.3.

 ERRORS
 ERR_INIT
 ERR_BADARG - prop CSV invalid; prop contains unknown properties.
 ERR_RX_PATTERN - invalid regular expression.
 ERR_FUNCADDRTYPE - cbFunc invalid.


if(!a) end ERR_INIT
flags&0x027F
flags|0x4100
if(cbFunc) flags|0x8000

if empty(prop)
	ret acctest(this name role 0 "" prop flags cbFunc cbParam)
else
	int hwnd=GetAncestor(child(a) 2) ;;need it when using class, id, xy
	ret acctest(this name role hwnd "" prop flags cbFunc cbParam)

err+
	if(_error.code!ERRC_HWND) end _error
