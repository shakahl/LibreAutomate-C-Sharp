 /GetFuncDllFromDoc
function $_file str&sout

 Appends to sout.

typelib VsHelp {83285928-227C-11D3-B870-00C04F79F802} 1.0
VsHelp.DExploreAppObj dex._getactive

dex.Filter="Platform SDK"; err

str s ss.getfile(_file) fn
int i n=numlines(ss)
foreach fn ss
	if(GetFuncDllFromDoc3(dex fn s))
		out s
		sout.formata("%s[]" s)
	else
		out "? %s" fn
	i+1
	_s.format("%i from %i" i n)
	_s.setwintext(id(2204 _hwndqm))

dex.Filter="(no filter)"; err
