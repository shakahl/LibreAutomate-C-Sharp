 This example works with QM Find dialog. Repeats while value of edit control text is less than 10.

int i h; str s

h=id(1127 "Find")
s.getwintext(h)
i=val(s)

for i i+1 10
	s=i
	s.setwintext(h)
	0.2

 If edit control is in web page, this will not work. Then you have to use html or accessible object (QM 2.1.2 beta), or copy/paste edit control contents, as in example below.

 act "Find"
 
 int i; str s
 
 key H SE ;;select
 s.getsel
 i=val(s)
 
 for i i+1 10
	 s=i
	 s.setsel
	 key H SE
	 0.2