 /CtoQM
function# $s ;;s begins after (

 info: if error, ret 8x.
 note: when adding multiple files, don't add the pshpack/pop files. info: in WINAPI2 they are excluded.

int pack
if(s[0]=')') pack=8
else if(isdigit(s[0])) pack=val(s)
else
	ARRAY(str) ap
	if(findrx(s "^(?:push|pop)(,[A-Za-z_]\w*)?(,\d+)?\);$" 0 0 ap)<0) ret 80
	pack=iif(ap[2].len val(ap[2]+1) 0)
	sel ap[0][1]
		case 'u'
		if(ap[1].len) out "Warning in %s: $pack(%s: does not support named pack. Will ignore the name." _s.getfilename(m_file 1) s ;;tested: Win7 SDK contains several such pragmas, but the name is not useful and can be ignored
		m_ps.fromn(m_ps m_ps.len &m_pack 1)
		if(!pack) ret ;;only push
		 
		case 'o'
		if(!m_ps.len) ret 81 ;;pop without pack
		m_ps.len-1; _i=m_ps[m_ps.len]
		if(!pack) pack=_i ;;specifying new pack value also possible (pop,newvalue)

sel pack
	case [1,2,4,8] m_pack=pack
	case else ret 82
