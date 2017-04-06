str s="a ?*<>''/\\| [1] [31] b"
 s="a."
 s=" ab "
 s=".txt"
 s="CON"
 s="CON-"
 s="CON.txt"
s.ReplaceInvalidFilenameCharacters("-")
out s
