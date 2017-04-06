out
 IRegex x=CreateRegex("(?<=.)" PCRE2_UTF)
IRegex x=CreateRegex("." PCRE2_UTF)
str s="a ąčę"
 out x.Replace(s ".")
 out s

ARRAY(str) a; int i
out x.MatchAll(s a)
out a
 for(i 0 a.len) out a[i]

 ARRAY(POINT) a; int i
 out x.MatchAll(s 0 a)
 for(i 0 a.len) out "%i %i" a[i].x a[i].y

 s="a ąčę"
 class Regex -pcre2_code*_p -pcre2_match_data*_m -pcre2_match_context*_mc -*_v -_vCount
 Regex y.Compile("(?<=.)" PCRE2_UTF)
 out y.ReplaceP(s ",")
 out s
