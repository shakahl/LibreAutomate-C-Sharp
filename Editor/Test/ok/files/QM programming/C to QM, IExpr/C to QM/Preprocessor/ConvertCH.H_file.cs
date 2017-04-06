 /CtoQM
function $shf [$shfparent] [level] [issys]

 Preprocesses header file, and recursively calls itself to preprocess #include'd files.
 Appends all included/preprocessed stuff to m_s.

str s ss shfi shf2
int ok si skip hr db once
str stack.all(100)

if(shf[0]!='$' and shf[0]!='%' and shf[1]!=':') ;;not full path
	if(!issys) shf2.getpath(shfparent); shf2+shf
	if(issys or !dir(shf2))
		foreach(shf2 m_sInclDir)
			if(!shf2.end("\")) shf2+"\"
			shf2+shf
			if(dir(shf2)) ok=1; break
		if(!ok) out "Warning in %s: cannot open:[][9]%s" _s.getfilename(shfparent) shf; ret
	shf=shf2
else if(findc(shf '*')>=0) ;;eg "c:\x\*.h". Include all matching
	H_file_wildcard(shf level issys)
	ret

ss.all(level 2 32)
if(m_outIncl) out "%s%s" ss shf

m_mh.Add(shf "")
err ;;already included
	ret
	 once=1
	 sel(shf 3) case ["*initguid.h","*guiddef.h"] ret
	 out "Info: already included: %s,  size=%i" shf GetFileOrFolderSize(shf)

ss.getfile(shf); err out "Warning in %s: cannot open[][9]%s" _s.getfilename(shfparent) shf; ret
H_tidy(shf ss)
 out ss
 if(once)
	 if(ss.len>512) ret
	 out "Info: already included: %s,  size=%i" shf ss.len
m_file=shf; m_s.formata("$file''%s'';[]" shf)

lpstr s1(ss) s2(ss+ss.len) s3 s0
for s1 s1 s2 2
	hr=0; db=0
	if(s1[0]='#')
		s0=s1
		s.getl(s1 0); s1+s.len
		int lens=s.len
		if(s[1]=32) s.replacerx("^# +" "#" 4)
		
		 skip: 0 include, 1 skip, 2 include all (if/elif/else), 3 skip all
		sel(s 2)
			case "#if*"
			 out s
			stack[si]=skip; si+1
			if(skip&1) skip=3
			else skip=_if(s)
			if(db) out "%s, skip=%i" s skip
			
			case "#elif*"
			if(si<1) end "unmatched #elif. File %s" 1 shf
			if(skip&2=0)
				if(skip=0) skip=3
				else skip=_if(s)
			if(db) out "%s, skip=%i" s skip
			
			case "#else*"
			if(si<1) end "unmatched #else. File %s" 1 shf
			if(skip&2=0) skip^1
			if(db) out "%s, skip=%i" s skip
			
			case "#endif*"
			if(si<1) end "unmatched #endif. File %s" 1 shf
			si-1; skip=stack[si]
			if(db) out "%s, skip=%i" s skip
			
			case else
			if(skip&1) continue
			sel s 2
				case "#define *"
				if(find(s "__STDC__")>0) out s
				hr=_define(s)
				
				case "#undef *"
				s.gett(s 1)
				 out shf; out s
				m_mc.Remove(s); if(_hresult) m_mcf.Remove(s)
				
				case "#include*"
				if(findrx(s+8 "^ *([<''].+?)[>'']" 0 0 shfi 1)<0) end "err" 1
				shfi.findreplace("/" "\")
				sel shfi 3 ;;don't process "pack" files, because the converter may include them once, making packing invalid
					case "*pshpack1.h" m_s.addline("$pack(push,1);"); continue
					case "*pshpack2.h" m_s.addline("$pack(push,2);"); continue
					case "*pshpack4.h" m_s.addline("$pack(push,4);"); continue
					case "*pshpack8.h" m_s.addline("$pack(push,8);"); continue
					case "*poppack.h" m_s.addline("$pack(pop);"); continue
				H_file(shfi+1 shf level+1 shfi.beg("<"))
				m_file=shf; m_s.formata("$file''%s'';[]" shf)
				
				case "#pragma *"
				sel SelStr(32 s+8 "^ *once$")
					case 1 if(once) ret
				
				case "#error *"
				out "#error in %s: %s" shf s+6; ret
				
				case "#"
				case else hr=-1
	else
		s3=strstr(s1 "[]#"); if(!s3) s3=s2
		if(skip&1) s1=s3; continue
#opt nowarnings 1
		if(s3=s1) continue
		s.left(s1 s3-s1); s1=s3
		 out s
		
		ExpandMacros(s)
		m_s.formata("%s[]" s)
	
	if(hr) out "%i %s" hr s
