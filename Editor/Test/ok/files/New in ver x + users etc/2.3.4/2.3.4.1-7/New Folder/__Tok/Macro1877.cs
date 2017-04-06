out
str s=
 a b "string" 'c' a[2] func(args) 9+a(j)+k a[8 func("oo)]kk")] F"a'' {`b''c`}" {window} _s.all
 int'arg ARRAY(str)arg ARRAY(str)'arg [attr]something [optional] function'int function[c]int function[c]'int
 1'b' ;;err
 1(2)
 "one""two"
 (1)(2)
 5"two"
 (5)"two"
 "two"5
 "two"(5)
  _hwndqm(2) ;;err
  _hwndqm"two" ;;err
 fff(1)fff(2)
  'a''b' ;;err
  'b'2 ;;err
 1,2
 1,,2
 , 1, 2
 (,1,2,)
 @"one"

str ss
foreach ss s
	out "---- %s" ss
	
	ARRAY(str) a; int i n
	n=__Tok(ss &a)
	for i 0 a.len
		out "<><z 0xc0ffff>%s</z>" a[i]
