 Shows drop-down list with check boxes; uses a 32-bit variable to get checked items.
str icons="$qm$\new.ico[]$qm$\copy.ico[]$system$\shell32.dll,4"
str csv=
F
 ,"{icons}",2
 one,1
 two,2
 three,,1

int checked
int R=ShowDropdownListSimple(csv checked)
out F"R=0x{R} checkBoxes=0x{checked}"

int i
for i 0 3
	if(checked>>i&1) out F"item {i} is checked"
