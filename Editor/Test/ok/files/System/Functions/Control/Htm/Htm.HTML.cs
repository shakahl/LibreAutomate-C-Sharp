function~ [flags] ;;flags: 0 outer, 1 inner

 Gets HTML.

 REMARKS
 flags added in QM 2.3.3.


if(!el) end ERR_INIT

if(flags&1) ret el.innerHTML
ret el.outerHTML

err+ end _error
