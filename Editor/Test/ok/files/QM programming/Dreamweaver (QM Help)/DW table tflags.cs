str s
DW_SelectSource s

str sep
if(!inp(sep "Separator(s). It makes 2 columns. If empty - 1 column." "" "-:")) ret
 sep=":"

s.replacerx("<span\s+class=''courier''>([^<]*)</span>" "$1" 1)

int r
if(sep.len) r=s.replacerx(F"(?si)<p>\s*(.+?)\s*[\{sep}]\s*(.*?)\s*</p>" "<tr>[]<td>$1</td> <td>$2</td>[]</tr>")
else r=s.replacerx("(?si)<p>\s*(.+?)\s*</p>" "<tr><td>$1</td></tr>")
if(!r) mes- "Failed."

s=F"<table class=''tflags''><col>[]{s}[]</table>[]"
s.setsel
key C`
