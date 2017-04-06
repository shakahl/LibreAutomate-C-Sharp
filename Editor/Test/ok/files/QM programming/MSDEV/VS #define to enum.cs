 Convers multiple selected #define lines to C++ enum.
 This is an older version.

str s.getsel; if(!s.len) mes- "Select multiple #define and run this macro"

s.replacerx("^#define\s+(\w+)\s+(\S+)" "[9]$1=$2," 8)
s-"enum ENUM_[]{[]"; s+"};"
if(mes("Make single line?" "" "YN2")='Y') s.findreplace("[]" " "); s.findreplace("[9]")
s+"[]"
paste s
