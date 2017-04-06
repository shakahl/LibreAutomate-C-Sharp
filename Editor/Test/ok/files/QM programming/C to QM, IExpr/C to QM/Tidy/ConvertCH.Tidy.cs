 /CtoQM

 Modifies preprocessed code so that extracting declarations would be easier.
 Joins lines, removes unnecessary spaces, removes some keywords, etc.

str s

m_s.replacerx("[]\s+" "[]") ;;remove empty lines and indentation
Blocks ;;remove extern "C"{, namespace X{, template ...{...}, join lines surrounded by {} and ()

m_s.replacerx("\b__?(stdcall|cdecl|w64)\b") ;;remove unnecessary keywords
m_s.replacerx("\b__?declspec *\( *\w+ *\)") ;;remove unnecessary keywords
m_s.replacerx("\b__?declspec *\( *''.*?'' *\)") ;;remove unnecessary keywords
m_s.replacerx("\b__declspec\(align\(4\)\)") ;;remove unnecessary keywords
m_s.replacerx("\bthrow(?: *\(.*?\))?") ;;remove unnecessary keywords
m_s.replacerx("\b__\$[a-z]{3}_\w+\(\w+(\( *\))?\)") ;;remove SAL macros containing $ in name. Normally adding them to specstrings_nothing.h would remove them, but somehow it does not work for some.

foreach(s "const[]unsigned[]signed[]volatile[]virtual[]static[]mutable[]WINAPI[]SEC_ENTRY[]WSPAPI") m_s.findreplace(s "" 2) ;;remove unnecessary keywords, including nonexpanded macros

m_s.replacerx("[]\s+" "[]") ;;remove indentation made when removing keywords, and possibly empty lines after Blocks
m_s.replacerx(" +(?=\W)"); m_s.replacerx("(?<=\W) +") ;;remove spaces

m_s.replacerx("^(.+[^;}])[]" "$1 " 8) ;;join lines
m_s.replacerx("^(typedef .+?[^;])[]" "$1 " 8) ;;join lines typedef ... {...}[]...;
m_s.replacerx("(?<=\W) "); m_s.replacerx(" (?=\W)") ;;remove spaces added when joining lines
SplitMultiStatementLines(m_s)
m_s.replacerx("^[\};]+[]" "" 8) ;;remove } left after removing extern "C" {, and empty ;

m_s.replacerx("^typedef (\w+\**)\((\w+)\)(\(.*?\);)[](?:.+?[])?\2 (\w+);" "$1 $4$3" 8) ;;typedef a(b)(c)[]b FunctionName;

m_s.replacerx("^.+?\b\w+?_(Proxy|Stub)\(.+;[]" "" 8) ;;remove proxy/stub functions that follow interfaces
m_s.replacerx("^(?:const )?\w+[ \*]\w+;[]" "" 8) ;;forward decl (struct)
m_s.replacerx("^typedef (?:struct|class|union) (\w+) \1;[]" "" 8) ;;typedef A A (forward decl)
m_s.replacerx("^extern [^\(\)]+?;[]" "" 8) ;;remove var, etc
m_s.replacerx("^extern " "" 8) ;;remove extern from func
m_s.replacerx("^typedef (__declspec\(uuid\(.+?\)\))(struct|class|enum|union) " "typedef $2 $1" 8) ;;uuid before struct
m_s.replacerx("^typedef __declspec\(uuid\(.+?\)\)" "typedef " 8) ;;eg typedef __declspec(uuid("..."))UINT OLE_HANDLE;

m_s.replacerx("enum __declspec\(uuid\(.+?\)\)" "enum " 8)
m_s.replacerx("(?<=[,\(])struct "); m_s.replacerx("(?<=[,\(])enum "); m_s.replacerx("(?<=[,\(])union "); m_s.replacerx("(?<=[,\(])class ") ;;remove struct etc prefixes in function args
m_s.replacerx("(?<=[;\{])struct (?=\w+[\* ]+\w+)"); m_s.replacerx("(?<=[;\{])enum (?=\w+[\* ]+\w+)"); m_s.replacerx("(?<=[;\{])union (?=\w+[\* ]+\w+)"); m_s.replacerx("(?<=[;\{])class (?=\w+[\* ]+\w+)") ;;remove struct etc prefixes from struct members, except if it is embedded struct
m_s.replacerx("\bshort int\b" "short"); m_s.replacerx("\blong int\b" "int"); m_s.replacerx("\blong double\b" "double")
m_s.replacerx("\)throw(?:\(\))?;$" ");" 8)
m_s.findreplace(";;" ";") ;;some macros expand so that two ; are at the end

 out m_s

 NOTES
 In regex, (word1|word2) is slow. Instead use two statements. Not slow if preceded by something, eg __(stdcall|cdecl).

 Some debugging code:

 ARRAY(str) a
 findrx(m_s "\b\w+\$\w+\b" 0 4 a)
 int i
 for(i 0 a.len) out a[0 i]

 int i=find(m_s "CoInternetGetSecurityUrlEx")
 if(i>0) out _s.get(m_s i 200)
