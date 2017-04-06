str s ss sss
rep 100000
	ss.RandomString(1 2); ss.replacerx("[;'']" "k")
	sss.RandomString(1 2); sss.replacerx("[;'']" "k")
	s.formata("%s; %s[]" ss sss)
 out s
s.setfile("$desktop$\data.csv")

s=""
rep 1000
	ss.RandomString(1 2)
	sss.RandomString(1 2)
	s.formata("%s %s[]" ss sss)
s.setfile("$desktop$\map.txt")
