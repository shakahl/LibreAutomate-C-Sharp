 /C header files
function ~file str&sout
 Extracts #define macros (except function-like) from C header file
 and converts to const constants.


str s ss sl s1 s2; lpstr d1 d2; int l1 l2 i j k n
ss.flags=3; sl.flags=1; s1.flags=1; s2.flags=1

file.searchpath; if(file.len=0) end "file not found"
s.getfile(file); if(s.len=0) ret

CH_Prepare &s
rep
	 get line
	if(l2>=s.len) break
	l2=find(s "[]" l1); if(l2<0) l2=s.len
	sl.get(s l1 l2-l1); sl.trim; l1=l2+2
	if(sl.beg("#define "))
		n=tok(sl+8 &s1 2 " 	" 2 &d1); if(n<2) continue
		if(findc(s1 '(')>=0) continue;;skip macros
		if(s2.len=s1.len+1 and s2.beg(s1) and (s2[s2.len-1]='A' or s2[s2.len-1]='W')) continue;; skip A/W functions
		 format
		ss.formata("const %s = %s;[]" s1 s2)

CH_Compact(ss &sout)
