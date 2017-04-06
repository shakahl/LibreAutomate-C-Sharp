str s.getsel; if(!s.len) ret

 str s=
  		s2.Format(
  "<><macro \"VS_goto /$\">$
  </macro>$", &s1.p, s3.p, b+i);

REPLACERX r.frepl=&Callback_str_replacerx3
if(s.replacerx("(?s)\bFormat\(\s*''.+''" r 4)<0) ret
 out s
s.setsel
