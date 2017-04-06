 This macro will be displayed when you click this class name in code editor and press F1.
 Add class help here. Add code examples below EXAMPLES.

 EXAMPLES

#compile "__StaticMember"
StaticMember x
x.Public
 x.Protected
 x.Private
out x.xPub
 out x.xProt
 out x.xPriv
StaticMember_Test x

StaticMemberInh y
y.Test

StaticMemberInh_Test x

 StaticMemberInh y
sub.StaticMember_One x
sub.StaticMember_Two y
 sub.StaticMemberInh_One x
 sub.StaticMemberInh_Two y


#sub  StaticMember_One
function StaticMember&x
x.Public
x.Protected
x.Private
out x.xPub
out x.xProt
out x.xPriv


#sub StaticMember_Two
function StaticMemberInh&y
y._.Public
y.Public
y.Protected
y.Private
out y.xPub
out y.xProt
out y.xPriv


#sub StaticMemberInh_One
function StaticMember&x
x.Public
x.Protected
 x.Private
out x.xPub
out x.xProt
 out x.xPriv


#sub StaticMemberInh_Two
function StaticMemberInh&y
y.Public
y.Protected
 y.Private
out y.xPub
out y.xProt
 out y.xPriv
