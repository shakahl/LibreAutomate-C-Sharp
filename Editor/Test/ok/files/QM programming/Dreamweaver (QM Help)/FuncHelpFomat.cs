 /
function str&s

str ss sss

 out s
if(findrx(s "<span class=fd>(\w+)</span>" 0 0 ss 1)<0) ret ;;get dll function name
sss.format("<a name=''%s''></a>" ss)
if(!s.findreplace("<span class=i>&#9;</span>" sss 4)) ret ;;replace comma to anchor
 s.replacerx("\[(.+?)\]" "<span class=silver>[</span>$1<span class=silver>]</span>") ;;make [optarg] gray. Now we don't use it.
 out s
