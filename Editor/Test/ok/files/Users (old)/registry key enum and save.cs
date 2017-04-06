ARRAY(str) a
RegGetValues(a "software\gindi\qm2\settings" 0 1)
int i; str s
for i 0 a.len
	s.formata("%s=%s[]" a[0 i] a[1 i])
s.setfile("$desktop$\test.txt")
run "$desktop$\test.txt"
