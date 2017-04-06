 /
function$ MSScript.ScriptControl'c [iid]

MSScript.Error e=c.Error
if(!e.Number) ret _error.description
str ed(e.Description) et(e.Text)
int line(e.Line) column(e.Column+1)
str-- errstr
lpstr tab="[9]"
if iid ;;create link
	str macro.getmacro(iid 1)
	errstr=
	F
	 {ed}
	 {tab}<macro "Scripting_Link /{line} {column} 2 ''{macro}''">{macro}({line},{column})</macro>: {et}
else
	macro=c.Language
	errstr=
	F
	 {ed}
	 {tab}{macro}({line},{column}): {et}
e.Clear
ret errstr
