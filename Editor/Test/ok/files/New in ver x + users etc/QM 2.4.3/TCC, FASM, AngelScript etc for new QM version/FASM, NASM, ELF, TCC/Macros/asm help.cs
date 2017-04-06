 Double click a CPU instruction, and this macro will open its reference page at www.felixcloutier.com.
 Note: the reference includes instructions for the laters Intel CPUs. I would need only those that can be used with Pentium MMX.

dou
str s.getsel; err
s.trim
if(!s.len) ret
if(findrx(s "\W")>=0) ret
s.ucase

sel s 2
	case ["CBW","CWDE","CDQE"] s="CBW:CWDE:CDQE"
	case ["CMPSD","CMPSS"]
	case "CMPS*" s="CMPS:CMPSB:CMPSW:CMPSD:CMPSQ"
	case ["CWD","CDQ","CQO"] s="CWD:CDQ:CQO"
	 case "F...
	case "INT*" s="INT n:INTO:INT 3"
	case "JMP"
	case "J*" s="Jcc"
	case "L?S" s="LDS:LES:LFS:LGS:LSS"
	case "LODS*" s="LODS:LODSB:LODSW:LODSD:LODSQ"
	case "LOOP*" s="LOOP:LOOPcc"
	case ["MOVD","MOVQ"] s="MOVD:MOVQ"
	case ["MOVS","MOVSB","MOVSW","MOVSD","MOVSQ"] s="MOVS:MOVSB:MOVSW:MOVSD:MOVSQ"
	case "POPA*" s="POPA:POPAD"
	case "PUSHA*" s="PUSHA:PUSHD"
	case ["RCL","RCR","ROL","ROR-"] s="RCL:RCR:ROL:ROR-"
	case "REP*" s="REP:REPE:REPZ:REPNE:REPNZ"
	case ["SAL","SAR","SHL","SHR"] s="SAL:SAR:SHL:SHR"
	case ["SARX","SHLX","SHRX"] s="SARX:SHLX:SHRX"
	case "SET*" s="SETcc"
	case "STOS*" s="STOS:STOSB:STOSW:STOSD:STOSQ"
	 case "" s=""
	 case "" s=""
	 case "" s=""
	 case "" s=""
	 case "" s=""
	 case "" s=""
	 case [""] s=""
	 case [""] s=""
	 case [""] s=""
	 case [""] s=""
	 case [""] s=""
	 case [""] s=""
	 case [""] s=""
	 case [""] s=""

s-"http://www.felixcloutier.com/x86/"; s+".html"
run s
