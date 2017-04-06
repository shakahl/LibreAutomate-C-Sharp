str s.getsel
if(s.len)
	s.wrap(60 "" "" 1)
	s.findreplace("[]" "[] ")
	s.findreplace("  " " ")
	s.rtrim
	s+"[]"
	s.setsel
