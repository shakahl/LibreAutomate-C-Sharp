str s="required_email=gindi@takas.lt&action=subscribe&beta=yes"
str sp
IntPost("http://www.quickmacros.com/cgi-bin/cgiemail/templ1.txt" s sp)
out sp
	