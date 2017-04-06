function! [flags] ;;flags: 1 get selection, 2 only fragment

 Gets HTML from clipboard or selection.
 Returns 1 if HTML found, 0 if not.

 See also: <Acc.WebPageProp>, <Htm.DocProp>, <Htm.HTML>, <FFNode.HTML>, <IntGetFile>, <HtmlDoc help>
 Added in: QM 2.3.3.


this.all
str s
if(flags&1) s.getsel(0 "HTML Format"); err ret
else s.getclip("HTML Format")

int i j
if flags&2
	i=find(s "StartFragment:"); if(i<0) ret
	j=find(s "EndFragment:"); if(j<0) ret
	i=val(s+i+14)
	j=val(s+j+12)
	this.get(s i j-i)
	this-"<html><body>[]"; this+"[]</body></html>"
else
	i=find(s "StartHTML:"); if(i<0) ret
	i=val(s+i+10)
	this.get(s i)

ret this.len!0
