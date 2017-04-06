 Fails:
 collect2: cannot find `ld'
 Compilation failed: 1 error(s), 0 warning(s)
 error: cc exited with status 1

 Compilation is slow. Starts many processes with visible consoles.

out
 SetCurDir "q:\test"
SetCurDir "C:\vala-0.12.0\bin"
SetEnvVar "cc" "gcc"

str sf="q:\test\hello.vala"
str src=
 class Demo.HelloWorld : GLib.Object {
 
     public static int main(string[] args) {
 
         stdout.printf("Hello, World\n");
 
         return 0;
     }
 }
src.setfile(sf)

str valac="C:\vala-0.12.0\bin\valac.exe"

RunConsole2(F"{valac} {sf}" 0 "C:\vala-0.12.0\bin")
