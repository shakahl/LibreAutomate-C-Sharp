 /exe

 sends command line to QM to run process_files_543

str s1 s2
s1.RandomString(3 3 "abc")
s2.RandomString(3 3 "xyz")
s1.addline(s2)

str cl=
F
 M "process_files_543" A "{s1}"

run "Q:\app\qmcl.exe" cl

 BEGIN PROJECT
 main_function  Macro2208
 exe_file  $my qm$\Macro2208.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 flags  6
 guid  {ED9E922C-2B22-49BF-8905-5FADF04B8B5D}
 END PROJECT
