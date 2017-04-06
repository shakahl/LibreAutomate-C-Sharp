 /exe
out

 WebBrowserControlDisableIE7Emulation
opt waitmsg 1
int minLength=500
HtmlDoc d
d.SetOptions(2)
 g1
d.InitFromWeb("http://www.imea.com.br/imea-site/indicador-milho")

rep 20 ;;wait for the javascript to finish
	0.5
	str s=d.GetHtml("tbody" "body-milho-disponivel")
	out s.len ;;335 when empty, 5308 when full
	if(s.len>=minLength) break
	out "waiting"
if(s.len<minLength)
	out "RETRY"
	goto g1

out s

 BEGIN PROJECT
 main_function  Macro2753
 exe_file  $my qm$\Macro2753.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 flags  6
 guid  {F426B90A-BAF8-4698-82E9-63A67A491DE0}
 END PROJECT
