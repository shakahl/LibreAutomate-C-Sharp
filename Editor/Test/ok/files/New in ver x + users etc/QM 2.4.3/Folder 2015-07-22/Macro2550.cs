 Shows drop-down list and gets the selected item index.
str imagelist=":5 $qm$\il_qm.bmp" ;;tip: to see images in QM code editor, check toolbar button "Images in code editor"
str csv=
F
 ,{imagelist}
 one,1
 two,2
 three

int iSelected
int R=ShowDropdownListSimple(csv iSelected)
out F"R=0x{R}, selected={iSelected}"
