out

str s="a '' ' \ [] [8][9][11][12] ąﻟ"
out s
out "---------"
s.EscapeJavascript(2)
out s
out "---------"

s.UnescapeJavascript
out s
