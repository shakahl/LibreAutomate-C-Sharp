out
type LLP $s
LLP k="test"
lpstr s="kkk"

 sub.Test k

 ARRAY(LLP) a.create(2)
 sub.Test2 a

 sub.Test3 k ;;OK
sub.Test3 s ;;exception
 sub.Test3 "uuu"


#sub Test
function ANY'x


#sub Test2
function ARRAY(LLP)a
out a.len


#sub Test3
function LLP'x
 out x
out x.s
