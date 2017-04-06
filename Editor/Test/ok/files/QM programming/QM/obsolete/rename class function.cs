function !setNewName

 Renames class function under mouse pointer in treeview. Also can replace first word in names of macros and folders.

str+ g_newClassName
if(setNewName or !g_newClassName.len)
	inp g_newClassName "Enter new name. Then, for each function, move mouse over, and press Ctrl+F3." "Rename class functions" g_newClassName
	ret
 __________________________________________

rig
key r
str s.getsel
if(!s.replacerx("^\w+(?=[\W_])|^\w+$" g_newClassName)) ret
 out s
s.setsel
key Y
