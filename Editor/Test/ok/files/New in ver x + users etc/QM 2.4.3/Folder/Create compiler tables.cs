out
int i
str s="#pragma region tables //Created by macro 'Create compiler tables'[]"

 Character class table
s+"const BYTE g_ccTable[ 256 ]={"
lpstr CC_IDENT="CC_IDENT"
lpstr CC_DIGIT="CC_DIGIT"
lpstr CC_SPACE="CC_SPACE"
lpstr CC_OPERATOR="CC_OPERATOR"
lpstr CC_OTHER="CC_OTHER"
lpstr CC_NO="CC_NO"
lpstr cc pcc
for i 0 256
	if((i>='A' and i<='Z') or (i>='a' and i<='z') or i='_') cc=CC_IDENT
	else if(i>='0' and i<='9') cc=CC_DIGIT
	else
		sel i
			case [0,10] cc=CC_OTHER
			case [32,9,13] cc=CC_SPACE
			case ['=','!','^','~','@','+','-','*','/','%','|','&','>','<'] cc=CC_OPERATOR
			case ['?','\','`'] cc=CC_NO
			case else cc=iif((i>=32 and i<=126) CC_OTHER CC_NO)
	if cc!=pcc
		pcc=cc
		s+"[]"
	s.formata("%s," cc)
s+"[]};[]#define CCLASS(ch) g_ccTable[(BYTE)(ch)][][]"

 Operator precedence table
s+"const BYTE g_opTable[ 256 ]={"
for i 0 127
	int p=0; lpstr sop=0
	sel i
		case 16 p=10; sop="&&"
		case 17 p=10; sop="||"
		case ['=','!','<','>'] p=20
		case 18 p=20; sop="<="
		case 19 p=20; sop=">="
		case ['|','&','^','~'] p=30
		case 20 p=30; sop="<<"
		case 21 p=30; sop=">>"
		case ['+','-'] p=40
		case ['*','/','%'] p=50
	if(p)
		if(sop) s.formata("[]%i, //%s" p sop)
		else s.formata("[]%i, //%c" p i)
	else
		if(!s.end("0,")) s+"[]"
		s+"0,"
s+"[]};[]#define OPPREC(opChar) g_opTable[(BYTE)(opChar)][]"
s+"//For 2-character operators we use:[]"
s+"enum ENUM_OPERATOR { OPER_LOGAND=16, OPER_LOGOR=17, OPER_LTE=18, OPER_GTE=19, OPER_LSHIFT=20, OPER_RSHIFT=21, };"
s+"[]#pragma endregion"

out s
s+"[]"
s.setclip
