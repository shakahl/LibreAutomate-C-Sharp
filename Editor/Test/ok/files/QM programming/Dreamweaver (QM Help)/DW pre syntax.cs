str s
DW_SelectSource s

s.trim
if(!s.begi("<pre>")) mes- "selected text must begin with <pre>"

s.replacerx("<span\s+class=''blue''>([^<]*)</span>" "<b>$1</b>" 1)
s.replacerx("<span\s+class=''silver''>([^<]*)</span>" "<i>$1</i>" 1)
s.replacerx("<span\s+class=''courier''>([^<]*)</span>" "$1" 1)
s.insert(" class=syntax" 4)

s.setsel
key C`
