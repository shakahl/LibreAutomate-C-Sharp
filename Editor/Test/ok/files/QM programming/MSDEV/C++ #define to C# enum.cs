 Convers multiple selected #define lines to C# enum.
 Uses prefix (eg WM_) as enum name and removes from member names. If no such prefix, enum name is ENUM.


 act "Visual Studio"
 out

str s.getsel
if(!s.len)
	 ge
	mes- "Select one or more #define lines."

s.replacerx("[ \t]*//[^\r\n]*") ;;remove comments

str prefix; findrx(s "^#define\s+(\w+?_)" 0 0 prefix 1)

if(!s.replacerx(F"(?m)^#define\s+{prefix}(\w+)\s+([^\r\n]+)" "[9]$1 = $2,")) goto ge

if(!prefix.len) prefix="ENUM"
s=F"//[Flags][]enum {prefix}[]{{[]{s}}[]"

 out s
s.setclip
OnScreenDisplay "Stored in clipboard"
