out
str prog="Q:\Downloads\objconv\objconv.exe"

 RunConsole2 F"{prog} -felf -nr:__imp__OutputDebugStringA@4:OutputDebugStringA -nr:__imp__wvsprintfA@12:wvsprintfA -nu- Q:\app\qmcore\pcre\Release\pcre.obj Q:\Downloads\objconv\pcre.o"
RunConsole2 F"{prog} -felf -xp -nu- Q:\app\qmcore\pcre\Release\pcre.obj Q:\Downloads\objconv\pcre.o"
 RunConsole2 F"{prog} -felf -nu- Q:\app\qmcore\pcre\Release\maketables.obj Q:\Downloads\objconv\maketables.o"

 str s.getfile("Q:\Downloads\objconv\pcre.s")
 out s.replacerx("\$(\w)" "_1_$1")
 out s.replacerx("\?\?(\w)" "_2_$1")
 out s.replacerx("(\w)@" "$1_3_")
 out s.replacerx("(\w)\?" "$1_4_")
 s.setfile("Q:\Downloads\objconv\pcre.s")


 RunConsole2 F"{prog} -felf -nu- Q:\app\Release\TestTccLink.obj Q:\Downloads\objconv\TestTccLink.o"
