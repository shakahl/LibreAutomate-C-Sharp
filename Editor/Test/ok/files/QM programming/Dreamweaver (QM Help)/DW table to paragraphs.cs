 out
str s.getsel

if(s.replacerx("(?s)</?table\b.*?>")!=2) mes- "please select table in source code"
s.replacerx("<tr>\s*<td>" "<p>")
s.replacerx("</td>\s*</tr>" "</p>")
s.replacerx("</td>\s*<td>" " - ")
s.replacerx("<col\b.*?>")

 out s
s.setsel
key C`
