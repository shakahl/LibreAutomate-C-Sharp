function! ARRAY(str)&a $names [$defLbSel] [flags] ;;flags: 1 separator is newline (default space)

 Use with a multiplesel listbox variable to get names for selected items.
 Eg if this is "1010" and names is "A B C D", creates array where a[0]="A", a[2]="C" and other elements empty.
 If defLbSel used, uses it if there are no '1' in this.
 Always creates array of number of tokens in names.
 Returns 1 if there was '1' or used defLbSel.

int is=findc(s '1')>=0
if(!is and !empty(defLbSel)) s=defLbSel; is=1
int i n=tok(names a -1 iif(flags&1 "[]" " "))
for(i 0 n) if(s[i]!'1') a[i].all
ret is
