 Converts Win32Api.txt file (Win32 API Declarations for Visual Basic) to QM format.
 Converted "winapiQM.txt" file will be placed in QM folder.
 You can select it as reference file.


ClearOutput
int file1 = fopen("e:\Doc\References\WIN32API2.TXT" "r")
if(file1=0) mes "Can't find or open source file." "Error" "x"; ret
 str dest.expandpath("$qm$\winapiQM2.txt")
str dest.expandpath("$desktop$\winapiQM2.txt")
int file2 = fopen(dest "w")
if(file2=0) mes "Can't open destination file:[]%s" "Error" "x" dest; ret

int i j nt na lens Type Enum ntypes nfunctions nconstants nerrors byref
str s.all(1000) sv st; s.flags=1; sv.flags=1; st.flags=1
lpstr del1=" 	="
type TOKLPSTR50 $s[50]
type TOKSTR50 ~ss[50]
TOKLPSTR50 t ta
TOKSTR50 tt
ARRAY(str) aa af

str rxf rxa1 rxa2
findrx("" "^ *(?:Private +|Public +|)+Declare (?:Function|Sub) +(\w+) +Lib +''(.+?)'' +(?:Alias +''(.+?)'' +)?\((.*)\)(?: +As +(\w+))? *$" 0 1|128 rxf)
findrx("" "^ *(?:Optional +|)(ByVal +|ByRef +|)(\w+)(?:\(\))? +As +(\w+)" 0 1|128 rxa1)
findrx("" "^ *(?:Optional +|)(ByVal +|ByRef +|)(\w+)([\&\$\%\#\!\@])" 0 1|128 rxa2)

rep
	 get one line from file1
	if(fgets(s 1000 file1)) s.fix; else break
	 remove comments and empty lines
	if(s.len<2 or s[0]=''') continue
	lens=s.len;
	i=findc(s '''); if(i>=0) s.fix(i)
	 g1
	s.trim; if(s.len=0) continue
	
	if s.begi("Type")
 TYPE
		ntypes+1; Type=1; s[0]='t'
		fwrite(s 1 s.len file2)
	else if Type
		if(s.begi("End")) fputc(10 file2); Type=0; continue
 TYPE MEMBER
		nt=tok(s &t 5 "" 61 &tt); if(nt<3) goto error
		i=0; sv=t[0]
		 if array, get number of elements
		if(tt[1][0]=')')
			if(nt<4) goto error
			i+1; sv+"["
			if(_strnicmp(t[i] "1 To " 5)) sv + t[i]; else sv + (t[i]+5)
			sv+"]"
		i+1; if(_stricmp(t[i] "As")) goto error
		i+1
		 convert type string
		if(WA_ConvertTypeName(st t[i] 0 0))
			 if fixed length string, get number of elements
			i+1; if(i<nt) sv+"["; sv+t[i]; sv+"]"; sv-" !"; else goto error 
		else sv.from(" " st sv)	
		fwrite sv 1 sv.len file2
	else if s.begi("Declare")
 FUNCTION
		nfunctions+1
		if(findrx(s rxf 0 1 af)) goto error
		 out "%s %s %s %s %s" af[1] af[2] af[3] af[4] af[5]
		                      func  dll   alias args  type
		if(af[2].endi(".dll")) af[2].fix((af[2].len)-4)
		if(findc(af[2] 32)>=0) af[2].from("''" af[2] "''")
		if(af[3].len) ;;alias
			i=af[1].len
			if(!af[3].beg(af[1]) or af[3][i]!='A' or af[3][i+1]!=0)
				 out "%s %s %s" af[1] af[3] af[2]
				af[1]=af[3]
		 convert function type
		if(af[5].len) if(WA_ConvertTypeName(st af[5] 0 1)) goto error
		else st=""
		fprintf file2 "dll %s %s%s" af[2] st af[1]
		 convert arguments
		na=tok(af[4] &ta 50 "," 1)
		for j 0 na
			if(findrx(ta[j] rxa1 0 1 aa) and findrx(ta[j] rxa2 0 1 aa)) goto error
			if(WA_ConvertTypeName(st aa[3] !aa[1].begi("ByVal ") 2)) goto error
			fprintf file2 " %s%s" st aa[2]
		fputc 10 file2	
	else if s.begi("Const")
 CONSTANT
		nconstants + 1
		sv.gett(s 1 del1)
		 g2
		if(st.gett(s -1 del1 2) < 0) out 1; goto error
		if(st.begi("As "))
			if(st.gett(st 2 del1 2)<0) out 2; goto error
		if st[0]!34 and st[1]!34
			i=0
			rep
				i=findc(st '&' i); if(i<0) break
				if(st[i+1]='H' or st[i+1]='h') st.set("0x" i 2); i+2
				else if(st[i-1]!=32) st.remove(i 1)
				else i+1
			j=findcs(st " 	+*")+1
			if j>0
				i=j
				rep
					i=findw(st "Or" i " 	()" 1)
					if(i<0) break; else st.set("| " i 2); i+2
				i=j
				rep
					i=findw(st "And" i "" 1)
					if(i<0) break; else st.set(" & " i 3); i+3
				i=j
				rep
					i=findw(st "Not" i "" 1)
					if(i<0) break; else st.set(" ~ " i 3); i+3
				if(st[0]!'(') st-"("; st+")"
		fprintf file2 "def %-15s  %s[10]" sv st
	else if s.begi("Enum")
 ENUM
		Enum=1
		fprintf file2 " %s[10]" s
	else if Enum
		if(s.begi("End")) Enum=0; fputc(10 file2); continue
 ENUM MEMBER
		sv.gett(s 0 del1)
		goto g2
	else if(s.begi("Public")) s.get(s 7); goto g1
	else if(s.begi("Private")) s.get(s 8); goto g1
	else goto error
	continue
 error
	nerrors+1
	fseek file1 -(lens+1) 1; fgets(s 1000 file1)
	fprintf file2 "[10] Error: %s[10]" s
	s.rtrim; out s

fgetpos(file2 &i)
fclose(file1); fclose(file2)
if(nerrors) out "Cannot convert %i lines;" nerrors
else out "Converted successfully:"
out "Found %i types, %i functions and %i constants;[]File size is %i.%i KB." ntypes nfunctions nconstants (i/1024) (i%1024)
