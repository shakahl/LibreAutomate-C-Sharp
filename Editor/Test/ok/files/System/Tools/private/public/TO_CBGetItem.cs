 /
function# str&s

 Parses combo box selected item string set by ShowDialog.
 s must be in format "index string". Gets string and returns index.

int i=val(s 0 _i)
if(_i<s.len) s.get(s _i+1); else s=""
ret i
