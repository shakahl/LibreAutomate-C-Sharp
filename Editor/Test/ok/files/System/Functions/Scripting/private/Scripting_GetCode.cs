 /
function# $&code str&ss [flags] [stackLevel] ;;flags: 1 tcc

 If code looks like macro, gets code from macro (stores in ss) and returns QM item id. Else just returns 0.
 If code is empty, gets text of stackLevel-th caller, default 2.
 If code begins with "macro:", gets text of the macro.
 If flag 1, supports "*macro".

 Errors: <..>

opt noerrorshere 1

lpstr macro
if empty(code)
	macro=+getopt(itemid -iif(stackLevel stackLevel 2))
else if !StrCompareN(code "macro:" 6) and findc(code 10)<0
	macro=code+6
else if flags&1 and code[0]='*'
	macro=code+1
else
	ret

int iid=Scripting_GetCodeFromMacro(macro ss)
code=ss
ret iid
