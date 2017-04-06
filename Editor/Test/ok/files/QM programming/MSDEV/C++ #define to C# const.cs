 Convers multiple selected #define lines to C# const.


 act "Visual Studio"
 out

str s.getsel
if(!s.len)
	 ge
	mes- "Select one or more #define lines."

str sType; sel(ListDialog("int[]uint" "type")) case 1 sType="int"; case 2 sType="uint"; case else ret

s.replacerx("[ \t]*//[^\r\n]*") ;;remove comments

 if(!s.replacerx(F"(?m)^#define\s+(\w+)\s+([^\r\n]+)" F"[9]public const {sType} $1 = $2;")) goto ge
ARRAY(str) a; int i
if(!findrx(s F"(?m)^#define\s+(\w+)\s+([^\r\n]+)" 0 4 a)) goto ge
s=""
for i 0 a.len
	str t=a[2 i]
	t.trim(" [9]()L")
	s+F"[9]public const {sType} {a[1 i]} = {t};[]"

 out s
s.setclip
OnScreenDisplay "Stored in clipboard"
