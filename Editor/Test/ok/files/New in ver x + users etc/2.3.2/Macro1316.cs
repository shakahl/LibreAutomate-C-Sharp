out
 out "---- RT ----"
int i=40
str s="TEST_STR"
lpstr ls="TEST_LPSTR"
byte b=10
word w=20
long lo=-1
double d=1.5
 double d=1.12345678901234567890
 double d=1500000000000000
int* p=+400
BSTR bs="BSTR"
POINT po

out "escape {{ %i %% eee"

out F"int={i} str=''{s}'' lpstr=''{ls}'' byte={b} word={w} long={lo} double={d} pointer={p}[][9]inthex=0x{i} longhex=0x{lo} custom={d%%e}[][9]func={pow(2.1 2)} elem={s[0]} char={'A'%%c} unsigned={-1U} unshex=0x{-1U}"

lpstr m=F"a{0%%c}bbbbbbbbbbbb{s}bbbbbbbbbbbbbbbbb"
out m
out m+2

def FSTR F"- {i} {s} -"
out FSTR

 ____________________________

 ERRORS

 out F"[100]error{unknown+3}......."
 out F"aaa {i+1"
 out F"aaa {i+(1}"

 out F"POINT={po}" ;;type mismatch at CT
 out F"BSTR={bs}" ;;type mismatch at RT (same as with str.format)

 lpstr m=F"a[0]bbbbbbbbbbbb{s}bbbbbbbbbbbbbbbbb"
