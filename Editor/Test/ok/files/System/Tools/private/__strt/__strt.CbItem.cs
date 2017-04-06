function# [flags] ;;flags: 1 call S()

 Use with combobox or singlesel listbox variable. Gets text and returns index.

int i=val(s 0 _i)
if(_i<s.len) s.get(s _i+1); else s=""
if(flags&1) S
ret i
