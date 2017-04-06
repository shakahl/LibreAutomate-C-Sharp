sub.test("", "", 1);
sub.test("*", "", 1);
sub.test("*", "A", 1);
sub.test("", "A", 0);
sub.test("A*", "", 0);
sub.test("A*", "AAB", 1);
sub.test("A*", "BAA", 0);
sub.test("A*", "A", 1);
sub.test("A*B", "", 0);
sub.test("A*B", "AAB", 1);
sub.test("A*B", "AB", 1);
 sub.test("A*B", "AABA", 0);
sub.test("A*B", "ABAB", 1);
sub.test("A*B", "ABBBB", 1);
sub.test("A*B*C", "", 0);
sub.test("A*B*C", "ABC", 1);
sub.test("A*B*C", "ABCC", 1);
sub.test("A*B*C", "ABBBC", 1);
sub.test("A*B*C", "ABBBBCCCC", 1);
sub.test("A*B*C", "ABCBBBCBCCCBCBCCCC", 1);
sub.test("A*B*", "AB", 1);
sub.test("A*B*", "AABA", 1);
sub.test("A*B*", "ABAB", 1);
sub.test("A*B*", "ABBBB", 1);
sub.test("A*B*C*", "", 0);
sub.test("A*B*C*", "ABC", 1);
sub.test("A*B*C*", "ABCC", 1);
sub.test("A*B*C*", "ABBBC", 1);
sub.test("A*B*C*", "ABBBBCCCC", 1);
sub.test("A*B*C*", "ABCBBBCBCCCBCBCCCC", 1);
 sub.test("A?", "AAB", 0);
sub.test("A?B", "AAB", 1);
sub.test("A?*", "A", 0);
sub.test("A?*", "ABBCC", 1);
sub.test("A?*", "BAA", 0);
out "ok"

#sub test
function $p $s r
if(r!=matchw(s p)) mes F"{p}    {s}"
