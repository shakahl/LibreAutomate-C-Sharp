 str sd.expandpath("$my qm$\test")
str sd.expandpath("$temp$\test")

 CreateSymLink F"{sd}\sym" "$my qm$\qm icon" 1
 CreateSymLink "$my qm$\qm icon\sym" "$my qm$\test" 1
CreateSymLink F"{sd}\sym" "$my qm$\qm icon" 1

str cmd=
F
 mklink /J "{sd}\junction" "q:\my qm\Aspell"
system cmd

 if(!CreateHardLink(F"{sd}\qm.exe" "q:\app\qm.exe" 0)) out _s.dllerror

