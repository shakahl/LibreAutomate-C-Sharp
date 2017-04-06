 /C header files
function ~file ~includedir [flags] [str*files] ;;flags: 1 convert #include files (files is required, it will receive list of converted files), 2 include macros, 4 include all directives
 Extracts #define constants from C header file,
 converts to QM def constants and saves to txt file
 in Desktop\include.
 Properly converts only quite simple constants.
 Ignores directives.


if(file.len=0 or includedir.len=0) ret

str file1.format("%s\%s" includedir file)

if(flags&1 and files)
	if(files.len and find(*files file1 0 1)>=0) ret
	*files+file1; *files+"[]";

str file2.format("$Desktop$\include\%s.txt" file)

str s ss sl s1 s2; lpstr d1 d2; int l0 l1 l2 i j k n qu
ss.flags=3; sl.flags=1; s1.flags=1; s2.flags=1
 out file1
s.getfile(file1); err out "can't open %s" file1
if(s.len=0) ret
CH_Prepare &s
rep
	 get line
	if(l2>=s.len) break
	l2=find(s "[]" l1); if(l2<0) l2=s.len
	sl.get(s l1 l2-l1); sl.trim; l0=l1; l1=l2+2
	if(sl.beg("#define "))
		n=tok(sl+7 &s1 2 " 	" 2 &d1); if(n<2) if(n=1) s2="1"; goto format; else continue
		if(flags&2=0 and findc(s1 '(')>=0) continue;;skip macros
		if(s2.len=s1.len+1 and s2.beg(s1) and (s2[s2.len-1]='A' or s2[s2.len-1]='W')) continue;; skip A/W functions
		qu=0
		for(i 0 1000000000)
			sel(s2[i])
				case 0 break
				case '/' if(s2[i+1]='/' or s2[i+1]='*' and qu=0) s2.fix(i); break
				case 34 if(i=0 or s2[i-1]!'\') qu^1
				case 'L'
				if(qu=0 and i and (s2[i+1]<33 or __iscsym(s2[i+1])=0))
					for(k i-1 -1 -1) if(isxdigit(s2[k])=0) break
					if(k<0 or ((s2[k]='x' or s2[k]='X') and k and s2[k-1]='0' and (k=1 or __iscsym(s2[k-2])=0)) or __iscsym(s2[k])=0) s2[i]=32
		 format
		ss.formata("def %s %s[]" s1 s2)
	else if(sl.beg("include "))
		if(flags&1 and tok(sl+8 &s1 1 " 	''<>") and files)
			CH_Convert(s1 includedir flags files)
		if(flags&4) ss.formata("%s[]" sl)
	 else if(sl.beg("enum ") or sl.beg("typedef enum "))
		  enum
		 i=findc(s '{' l0); if(i<0) break
		 j=findc(s '}' l0); if(j<i) break
		 l2=find(s "[]" j); if(l2<0) l2=s.len
		 l1=l2+2
		 s[j]=0
		 ss.get(s i+1 j-i-1)
		 str* arr=StringToArray(s+i+1 "," 0 &j)
		 for i 0 j
			 arr[i].trim
			 if(findc(arr[i] '(')>=0) continue
			 out arr[i]
		 arr=SA_Free(arr)
		
	

CH_Compact(ss &s)
s.setfile(file2)
