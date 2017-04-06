function !save what ;;save: 0 load, 1 save;  what: 1 help, 2 tools, 3 functions, 4 tips

IStringMap& m
ARRAY(___CHI_FILE)& a
lpstr sf
sel what
	case 1 &m=m_mw_help; &a=m_af_help; sf="help"
	case 2 &m=m_mw_tools; &a=m_af_tools; sf="tools"
	case 3 &m=m_mw_func; &a=m_af_func; sf="func"
	case 4 &m=m_mw_tips; &a=m_af_tips; sf="tips"

int i; str s ss sff sfw

sff.from(m_dir "\" sf "_files.txt")
sfw.from(m_dir "\" sf "_words.txt")

if(save)
	 list of files or items
	for i 0 a.len
		___CHI_FILE& f=a[i]
		s.formata("''%s'' ''%s''[]" f.filename f.title)
	 out s
	s.setfile(sff)
	
	 list of words
	m.GetList(s "")
	s.setfile(sfw)
else
	 list of files or items
	s.getfile(sff)
	a.create(numlines(s))
	foreach ss s
		&f=a[i]
		tok(ss &f.filename 2 " ''" 4)
		 out "%s       %s" f.filename f.title
		i+1
	
	 list of words
	s.getfile(sfw)
	m.AddList(s "")
	err
		out "failed to load list of words from %s" sfw
