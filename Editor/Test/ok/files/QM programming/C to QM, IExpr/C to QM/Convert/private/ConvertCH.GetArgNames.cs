 /CtoQM
function# ~fn ARRAY(str)&a

 Gets missing argument names from dll_an_file (see ConvertCtoQM). If it is not specified or the function in the file does not exist, adds fn to dest_file_fan_missing_win.txt or dest_file_fan_missing_crt.txt.
 FuncA and FuncW in the file must be Func.

if(fn.end("A") or fn.end("W")) fn.fix(fn.len-1)

str s

if(m_mfan.Get2(fn s))
	  this code was for functions with ..., which now supported by qm
	 int i=find(s ";;")
	 if(i>=0)
		 _s.from(" " s+i)
		 s.fix(i); s.rtrim
		 m_mcomm.Add(fn _s); err
	tok s a
	ret 1

if(IsFuncNotUseful(fn)) ret
if(m_crt) m_sFuncArgsCrt.formata("%s[]" fn)
else m_sFuncArgsWin.formata("%s[]" fn)
