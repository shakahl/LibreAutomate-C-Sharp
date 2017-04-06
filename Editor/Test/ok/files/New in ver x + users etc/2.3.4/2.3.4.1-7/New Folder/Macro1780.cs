str s1 s2 s3 s4 s5 ;;must be local variables, like here; then they are allocated as array

str* p=&s1; int i
for i 0 5
	p[i].from("var" i)

for i 0 5
	out p[i]
