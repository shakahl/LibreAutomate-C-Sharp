 Finds differences between two strings or files.
 Patches a string or file.
 Merges 3 strings or files.
 Supports text (unified) and binary diff.

 Based on LibXDiff: <link>http://www.xmailserver.org/xdiff-lib.html</link>
 Setup: move xdiff.dll to qm folder.
 What is a unified diff: <link>http://en.wikipedia.org/wiki/Diff</link>

 EXAMPLES

#compile "__Xdiff"
Xdiff x
str s1="one[]two[]three[]four[]five[]six[]eight[]"
str s2="one[]twoo[]thre[]four[]six[]seven[]eight[]"
str sd
x.DiffText(s1 s2 sd 0)
out "---- diff ----"
out sd
out "---- patched ----"
str s2Patched
x.PatchText(s1 sd s2Patched)
out s2Patched
