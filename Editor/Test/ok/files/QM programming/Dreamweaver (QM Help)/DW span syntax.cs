str s.getsel
s.trim

s.replacerx("<span\s+class=''(?:courier-)?blue''>([^<]*)</span>" "<b>$1</b>" 1)
s.replacerx("<span\s+class=''(?:courier-)?silver''>([^<]*)</span>" "<i>$1</i>" 1)
s.replacerx("<span\s+class=''(?:courier-)?green''>([^<]*)</span>" "<s>$1</s>" 1)
s.replacerx("<span\s+class=''courier''>([^<]*)</span>" "$1" 1)
s-"<span class=syntax>"; s+"</span>"

s.setsel
key C`
