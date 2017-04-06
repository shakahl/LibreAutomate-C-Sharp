 Regular expression functions that use <link "http://www.pcre.org/">PCRE2 library</link>, version 10.20, year 2015.
 QM functions <help>findrx</help> and <help>str.replacerx</help> use an old version of this library (PCRE1, version 4.4, year 2003).
 In this new PCRE version: Many new features in regular expressions. Better supports Unicode. Can JIT-compile regular expressions to native code. And more.
 Speed: In most cases the new version is about 50% slower, but with some regular expressions searches many times faster in long strings. Slow with flag PCRE2_UTF. JIT-compiling can make searching faster, if used wisely.
 The Regex class functions are low-level wrappers of PCRE2 API functions. To find, usually need to call 3 functions: Compile(), Match(), Get(). To replace, call 2 functions: Compile(), Replace(). Also there is a Split() function.
 The PCRE2 API are in file pcre2.dll. It must be in QM folder. In exe - in exe folder.
 QM functions findrx/replacerx will continue to use the old PCRE version. Use Regex class if you need the new features.
 Virtually all regular expressions that work with findrx/replacerx also work with Regex class functions. However there are some differences, including:
   Default new line and \R characters are: \n, \r or \r\n. With findrx/replacerx too, but there are some differences, for example . includes \r.
   Uses default PCRE2 tables. findrx/replacerx use tables created at run time, and there may be some differences.
   Can match empty string. findrx/replacerx don't try if the string is empty.
   Supports binary data, ie can find/replace in strings that contain 0 characters.
 PCRE2 regular expression reference: <link "http://www.pcre.org/current/doc/html/pcre2syntax.html">short</link>, <link "http://www.pcre.org/current/doc/html/pcre2pattern.html">long</link>.
 PCRE API reference: <link "http://www.pcre.org/current/doc/html/">contents</link>, <link "http://www.pcre.org/current/doc/html/pcre2api.html">most API info</link>.

 EXAMPLES

out
 WakeCPU
#compile "__Regex"

type pcre2_match_data __[4] $__s
class Regex -pcre2_code*_p -pcre2_match_data*_m -pcre2_match_context*_mc -*_v -_vCount
type REGEX_REPLACE_CALLBACK ~t $s POINT*v vLen Regex*x number $repl

str s rx; int fC fM R
s="aa 7word bb 8kk cc"
 rep(10) s+s
 out s.len
rx="\d(?<na>\w+)"
 rx="\d(?:(?<na>\w+)|(?<na>;+))"
 rx="\d(?:(?<na>;+)|(?<na>\w+))"
 rx="\d(?:(?<na>;+)|(?<na>,+))?"
 rx="\d(?<na>;+)?"
 rx="\d(*MARK:B)\w+(*MARK:C)"
 rx="a(?=bc\K)"; s=",,abcde"
 rx=",(a(?=bc\K))"; s=",,abcde"
 fC=PCRE2_DUPNAMES
 rx="\d(\d)\d\d\d"; s="aa 78"; fM=PCRE2_PARTIAL_SOFT
Regex x
PF
x.Compile(rx fC)
 x.Compile(rx fC 1)
 err out _error.code;ret
PN
 R=x.Match(s fM)
 PN;PO
 out "Match() returned %i" R
 
 out x.Get(0 _i); out _i
  out x.GetStr(1 _s); out _s
  out x.GetByName("na" _i); out _i
  out x.GetByNameStr("na" _s); out _s
 ret

 out pcre2_get_startchar(x.get_match_data)
 out x.GetMark

 ARRAY(POINT) a; int i subm=-1
 R=x.MatchAll(s a subm fM)
 PN;PO ;;speed: 17  1216  
 out "MatchAll() returned %i" R
 for(i 0 a.len) if(subm>=0) out "%i %i" a[i].x a[i].y; else out "%i %i   %i %i" a[0 i].x a[0 i].y a[1 i].x a[1 i].y

 ARRAY(str) a; int i subm=-1
 R=x.MatchAllStr(s a subm fM)
 PN;PO ;;speed: 17  1254  
 out "MatchAllStr() returned %i" R
 for(i 0 a.len) if(subm>=0) out a[i]; else out "%s  %s" a[0 i] a[1 i]

 out x.Replace(s "" 0x100)
 out x.Replace(s "REPL")
out x.Replace(s "REPL" 0x100)
 out x.Replace(s "REPL" 0x100 7)
 out x.Replace(s "REPL" 0x100 3 s.len-7)
out s


 out "---"
 out pcre2_get_startchar(x.get_match_data)
 if(!pcre2_substring_length_bynumber(x.get_match_data 0 &_i)) out _i
 lpstr k
 out pcre2_substring_get_bynumber(x.get_match_data 0 &k &_i)
 out k; out _i

 pcre2_match_data* d=x.get_match_data
 out "%i" s
 int* p=+d
 int i
 for i 0 10
	 out "%i %i" i p[i]
	 