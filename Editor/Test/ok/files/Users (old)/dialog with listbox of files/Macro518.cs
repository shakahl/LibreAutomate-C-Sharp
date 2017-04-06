ARRAY(str)- t_af

str controls = "6 3 8"
str e6 lb3 e8
e6="$desktop$"
if(!ShowDialog("dlg_listbox_files" &dlg_listbox_files &controls)) ret

e6.expandpath
int i
for(i 0 t_af.len) t_af[i].from(e6 "\" t_af[i])
out t_af
