function~ [flags] ;;flags: 0 inner, 1 outer

 Gets HTML.

 REMARKS
 HTML is available only for 'element' nodes (node type 1). For other node types ('text', 'comment') use Text instead.
 Does not work in Chrome (returns empty string).


if(!node) end ERR_INIT

str s=node.innerHTML; err

if flags&1
	str tag=Tag
	ARRAY(str) a; int i
	AttributesAll(a)
	
	_s.from("<" tag)
	for(i 0 a.len) _s.formata(" %s=''%s''" a[0 i] a[1 i])
	_s.formata(">%s</%s>" s tag)
	s.swap(_s)

ret s

err+
