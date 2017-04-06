 Shows drop-down list and gets the selected item index.
str csv=
 ,$qm$\il_qm.bmp,0|1
 one,1
 two,2
 three

int iSelected
int R=ShowDropdownListSimple(csv iSelected)
out F"R=0x{R}, selected={iSelected}"

