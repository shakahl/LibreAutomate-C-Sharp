 Can be used to see invalid declarations in a reference file.
 Creates "Check ref" folder and several macros that contain all declarations from
 reffile in form refname.identifier. When you open such a macro, all declarations
 are extracted. Declarations with errors or unknown identifiers are black.


 str reffile="$qm$\winapi.txt"
 str refname="WINAPI"
 str reffile="$qm$\winapiv.txt"
 str refname="WINAPIV"
 str reffile="$qm$\winapi7.txt"
 str refname="WINAPI7"
 str reffile="$qm$\sqlite.txt"
 str refname="__sqlite"
 str reffile="$desktop$\xdiff.txt"
 str refname="__xdiff"
str reffile="$qm$\pcre2.txt"
str refname="__pcre2"


out
str s ss sss sf.getfile(reffile) af at ai ac
int retry
foreach s sf
	if(s.beg(" ") or s.beg("[9]")) continue ;;comments or member
	if(s.beg("dll"))
		if(findrx(s "^dll-? (?:''.+?''|\S+) \S*\b(\w+)(?= |)" 0 0 ss 1)<0) continue
	else ss.gett(s 1)
	 out ss
	str& r
	sel s 2
		case "def*" &r=ac
		case "dll*" &r=af
		case "type*" &r=at
		case "interface*" &r=ai
		case else continue
	r.formata("%s.%s[]" refname ss)

sf.from(at ai af ac)
foreach s sf
	sss.formata("%s[]" s)
	if(sss.len>32000)
		 g1
		newitem("" sss "" "" "\Check ref" 16|128|4)
		sss.all
if(!retry) retry=1; goto g1
