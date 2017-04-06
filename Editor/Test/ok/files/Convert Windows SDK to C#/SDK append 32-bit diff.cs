 Compares converter output files (64-bit and 32-bit) and saves in final .cs file with added differences.

out
str s64.getfile("Q:\app\Catkeys\Api\Api-64.cs")
str s32.getfile("Q:\app\Catkeys\Api\Api-32.cs")
str diff

IStringMap m._create
IStringMap m32._create

sub.ProcessCsFile(0 s64 diff m m32)
sub.ProcessCsFile(1 s32 diff m m32)
 out diff

str k
m32.EnumBegin
rep
	if(!m32.EnumNext(k)) break
	diff.findreplace(k F"{k}__32" 2)

str commentsEtc=
 // 32-BIT
 // Declarations that are different (or exist only) when the process is 32-bit.
;
s64.fix(findcr(s64 '}'))
s64+commentsEtc
s64+diff
s64+"[]}[]"
s64.setfile("Q:\app\Catkeys\Api\Api.cs")
out F"DONE, {m.Count} declarations, size = {s64.len/1024} KB, API32 size = {diff.len/1024} KB"


#sub ProcessCsFile
function !is32bit str&s str&diff IStringMap'm IStringMap'm32

str sType sFunc sInterf sVar sConst sOther
int i
i=findrx(s "(?ms)^// TYPE[](.+?)^[]//" i 0 sType 1); if(i<0) end "failed" 1
i=findrx(s "(?ms)^// FUNCTION[](.+?)^[]//" i 0 sFunc 1); if(i<0) end "failed" 1
i=findrx(s "(?ms)^// INTERFACE[](.+?)^[]//" i 0 sInterf 1); if(i<0) end "failed" 1
i=findrx(s "(?ms)^// VARIABLE[](.+?)^[]//" i 0 sVar 1); if(i<0) end "failed" 1
i=findrx(s "(?ms)^// CONSTANT[](.+?)^[]//" i 0 sConst 1); if(i<0) end "failed" 1
i=findrx(s "(?ms)^// CANNOT CONVERT[](.+?)^[][]\}" i 0 sOther 1); if(i<0) end "failed" 1

ARRAY(str) a

str rxType="(?ms)^\r\n(?:\[[^\r\n]+\r\n)*internal (?:struct|enum|interface|class) (\w+)[^\r\n\{]+\{(?:\}$|.+?^\})"
str rxFunc="(?m)^\r\n(?:\[[^\r\n]+\r\n)*internal (?:static extern|delegate) \w+\** (\w+)\(.+;$"
str rxVar="(?m)^internal static \w+ (\w+) =.+;$"
str rxConst="(?m)^internal (?:const|readonly) \w+ (\w+) =.+;$"

 TYPE
if(!findrx(sType rxType 0 4 a)) end "failed" 1 ;;struct, enum
sub.AddToMap is32bit a diff m m32 "struct/enum"
if(!findrx(sType rxFunc 0 4 a)) end "failed" 1 ;;delegate
sub.AddToMap is32bit a diff m m32 "delegate"
 FUNCTION
if(!findrx(sFunc rxFunc 0 4 a)) end "failed" 1
sub.AddToMap is32bit a diff m m32 "function"
 INTERFACE
if(!findrx(sInterf rxType 0 4 a)) end "failed" 1
sub.AddToMap is32bit a diff m m32 "interface"
 VARIABLE
if(!findrx(sVar rxVar 0 4 a)) end "failed" 1
sub.AddToMap is32bit a diff m m32 "variable"
if(diff.len) diff+"[][]"
 CONST
if(!findrx(sConst rxConst 0 4 a)) end "failed" 1
sub.AddToMap is32bit a diff m m32 "const"
if(diff.len) diff+"[]"
 CANNOT CONVERT
if(!findrx(sOther rxConst 0 4 a)) end "failed" 1
sub.AddToMap is32bit a diff m m32 "other"


#sub AddToMap
function !is32bit ARRAY(str)&a str&diff IStringMap'm IStringMap'm32 $debugType

int i
for i 0 a.len
	str& name(a[1 i]) value(a[0 i])
	if !is32bit
		 out F"{debugType} {name}"
		m.Add(name value)
		err
			out F"<><c 0xff>duplicate: {name} ({debugType}, 64-bit)</c>" ;;0 in SDK 
			continue
	else
		lpstr v=m.Get(name)
		if(v and value=v) continue
		
		 out "--------------------------------"
		 out F"64-bit:  {v}"
		 out F"<><c 0xff0000>32-bit:  {value}</c>"
		
		diff.addline(value)
		m32.Add(name)
		err
			out F"<><c 0xff>duplicate: {name} ({debugType}, 32-bit)</c>" ;;0 in SDK 
			continue
