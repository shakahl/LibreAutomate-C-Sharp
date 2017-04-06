 /CtoQM
function $dest_file $ref_list

 Saves all to files.

str v so

m_s.fix(0)
if(!empty(ref_list)) m_s.addline(ref_list)

m_mall.EnumBegin
rep
	if(!m_mall.EnumNext(0 v)) break
	if(m_delayload and v.beg("dll ") and !v.mid("C_macro" 4)) v.insert("-" 3) ;;cannot do it earlier
	m_s.addline(v)

m_mo.EnumBegin
rep
	if(!m_mo.EnumNext(0 v)) break
	so.addline(v)

m_s.setfile(dest_file)
sub.SaveOtherFile(so "_other.txt")
sub.SaveOtherFile(m_sFuncDll "_fdn_missing.txt")
sub.SaveOtherFile(m_sFuncArgsWin "_fan_missing_win.txt")
sub.SaveOtherFile(m_sFuncArgsCrt "_fan_missing_crt.txt")


#sub SaveOtherFile c
function str&s $suffix

str fn.from(m_dest suffix)

if(s.len) s.setfile(fn)
else del- fn; err
