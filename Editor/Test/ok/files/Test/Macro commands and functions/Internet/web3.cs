 web "http://"; err ret
 web "C:\Documents and Settings\a\Favorites\T A K A S - DSL portalas.url" 0x1
 web "http://" 0x11 0 "*"; err ErrMsg(2)
 web "http://www.t54545as.lt/dsl/apie_dsl.php?section=info&sub=2" 0x1 0 "" _s
 out _s

 int h=win("Internet Explorer")
 int h=win("MyIE2")
 int h=win("Slim")
int h=win("Avant")

str s
web "" 0x0 h "" s
out s
