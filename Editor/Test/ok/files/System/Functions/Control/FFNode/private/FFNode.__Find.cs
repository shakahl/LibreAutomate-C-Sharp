function! FFDOM.ISimpleDOMNode&x ___FFNODEFIND&e level

 GET NODE INFO

FFNODEINFO n
x.get_nodeInfo(n.bName n._NamespaceId n.bValue n.numChildren n._EventId n.nodeType)

if(e.flags&64 and !level) goto no ;;direct child; don't test self; need only n.numChildren

 outNodeInfo(x n level)

 TEST

if(e.nodeType and n.nodeType!e.nodeType) goto no

if e.tag.len
	 tag
	if(n.bName.len!e.tag.len or 1!VarBstrCmp(n.bName e.tag 0x409 NORM_IGNORECASE)) goto no
	
	 attributes
	int i j na=e.a.len
	if na
		x.get_attributesForNames(na &e.a1[0] &e.a3[0] &e.a2[0]); err goto no
		for(i 0 na) if(!e.a2[i].len) break
		if(i=na) for(i 0 na) _s=e.a2[i]; if(!__SelStr(e.cf2 _s e.a[i] 0)) break
		for(j 0 na) e.a2[j].free
		if(i<na) goto no
	
	 inner HTML
	if e.textOrHTML.len
		if(n.bValue.len) goto value ;;info: value is empty for element and most others, but can be eg for comment (tag="#comment")
		if(n.nodeType!FFDOM.NODETYPE_ELEMENT) goto no ;;info: HTML is only for element
		n.bValue=x.innerHTML; err
		goto value
else if(e.textOrHTML.len)
	 value
	if(!n.bValue.len) goto no
	_s=n.bValue
	if(!__SelStr(e.cf1 _s e.textOrHTML 0)) goto no

 callback function
if e.cbFunc
	sel call(e.cbFunc &x level e.cbParam &n)
		case 0 ;;yes
		case 1 goto no ;;no
		case 2 ret ;;no, skip children
		case else end "callback function must return 0, 1 or 2"

 match index
e.matchIndex-1; if(e.matchIndex) goto no
 out "found"
node=x
ret 1
 no

if(e.flags&64 and level=1) ret ;;direct child

 ENUM CHILDREN

if n.numChildren<1 ;;usually ~60%
	 is it frame?
	if(n.nodeType!FFDOM.NODETYPE_ELEMENT) ret ;;usually < 10%
	 gf
	sel n.bName.len
		case 5 if(1!VarBstrCmp(n.bName e.bFRAME 0x409 NORM_IGNORECASE)) ret
		case 6 if(1!VarBstrCmp(n.bName e.bIFRAME 0x409 NORM_IGNORECASE)) ret
		case else ret
	Acc af.a=+x; err ret
	af.Navigate("fi"); err ret
	FFNode nf.FromAcc(af); err ret
	ret __Find(nf e level+1)

FFDOM.ISimpleDOMNode y=iif(e.flags&128 x.lastChild x.firstChild)
rep
	if(__Find(y e level+1)) ret 1
	n.numChildren-1; if(!n.numChildren) goto gf
	y=iif(e.flags&128 y.previousSibling y.nextSibling)

err+

 _________________

 notes:

 innerHTML retrieves not exact source.
   Newlines \r\n replaced with \n. On XP, edit controls may display boxes. Tested: setwintext/getwintext ok.
   Entities &quot; replaced with "". Same in IE (htm dialog).

 Value is not exctly as displayed.
   Tabs not removed.
   Multiple spaces not replaced with single.
   Not trimmed spaces.

 tested: the above are the same in different FF versions.

 When searching in UI, does not find some, eg tab buttons.

 Does not go to frame/iframe content.
   We use acc to get there.
   Most frames have 0 children.
   Some have 1 (text "Your browser does not support frames..."), therefore goto gf after enum children.
   All this does not make slower.
   Firefox also has 1 iframe in UI, but cannot get acc.
