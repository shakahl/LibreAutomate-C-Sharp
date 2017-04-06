 /CtoQM
function# $fn str&dln str&truename

 Gets dll name for dll function declaration.
 Searches in dll_dn_file (see ConvertCtoQM). If it is not specified or the function in the file does not exist, adds fn to dest_file_fdn_missing.txt.
 FuncA and FuncW in the file must not be Func.

int i

for i 0 m_adll.len
	if(GetProcAddress(m_adll[i].h fn)) dln=m_adll[i].name; ret 1

if(m_mfdn.Get2(fn dln))
else if(m_crt)
	truename.from("_" fn)
	if(!m_mfdn.Get2(truename dln)) truename.all

if(dln.len)
	if(dln.end("]"))
		ARRAY(str) a
		if(findrx(dln "^(.+?) \[(.+?)\]$" 0 0 a)<0) end dln 1
		dln=a[1]; truename=a[2]
		 out "%s %s %s" fn dln truename
	 out "%s %s" fn dln
	ret 1
else
	if(IsFuncNotUseful(fn)) ret
	m_sFuncDll.formata("%s[]" fn)
	 out fn
