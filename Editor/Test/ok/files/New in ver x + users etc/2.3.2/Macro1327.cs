int i=50
str s="stringvar"
double d=3.1415926535897932384626433832795
str sv=F"variables: i={i}, s=''{s}'', d={d%%.5G}, expression={i+d}, function={_s.from(s i)}"
out sv ;;variables: i=50, s="stringvar", d=3.1416, expression=53.1415926535898, function=stringvar50

 also can use F"string" with key
key TT F"variables: 0x{i} {s%%20s} {d}" Y
