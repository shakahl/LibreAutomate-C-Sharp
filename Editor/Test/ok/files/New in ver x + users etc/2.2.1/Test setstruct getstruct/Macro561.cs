/exe
out
type T1 byte'x word'y str's
type T2 int'i str's T1'b double'd long'k @flags

T2 t tt
t.i=-5
t.s="''string''[]line2"
t.b.x=200
t.b.y=50000
t.b.s="  string2  "
t.d=1.55
t.k=0x100000000
t.flags=0x12

str s
s.getstruct(t 1)
out s
out "---"
s.setstruct(tt 1)

out tt.i
out tt.s
out tt.b.x
out tt.b.y
out tt.b.s
out tt.d
out tt.k
out tt.flags

 Output:
 i -5
 s "''string''[]line2"
 b.x 200
 b.y 50000
 b.s string2
 d 1.55
 k 4294967296
 
 ---
 -5
 "string"
 line2
 200
 50000
 string2
 1.55
 4294967296

 BEGIN PROJECT
 main_function  Macro561
 exe_file  $my qm$\Macro561.exe
 icon  $qm$\macro.ico
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {4EF13D64-23F8-4269-BD6B-43CD2E2F0922}
 END PROJECT
