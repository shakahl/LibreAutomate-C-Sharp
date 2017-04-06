Single line :mac "VS make single line"
x <code> :mac "XML comments example code"
x <see/> :sub.Surround("see" "cref" 1)
x <see>text</see> :sub.Surround("see" "cref")
x <msdn> :sub.Surround("msdn")
x <b> :sub.Surround("b")
x <i> :sub.Surround("i")
CatkeysHelp :run "$qm$\Catkeys\Help\CatkeysHelp\Help\CatkeysHelp.chm"

 !a"Microsoft Visual Studio" "HwndWrapper[DefaultDomain;;*" /DEVENV

#sub Surround
function $tag [$attribute] [flags] ;;1 <tag attr="..."/>
spe 10
key A0 ;;activate document

str s.getsel prefix
if(s.len=0 or s.end("[10]")) mes- "Select the API name or other text to google in MSDN."
if(s.beg("Api.")) s.remove(0 4); prefix="API "
if(s.end("()")) s.remove(s.len-2)
s.escape(11)

if(empty(attribute)) s=F"{prefix}<{tag}>{s}</{tag}>"
else if(flags&1) s=F"{prefix}<{tag} {attribute}=''{s}''/>"
else s=F"{prefix}<{tag} {attribute}=''{s}''>{s}</{tag}>"

paste s
