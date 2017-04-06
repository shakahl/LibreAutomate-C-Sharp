out
SetEnvVar "GOPATH" "q:\test\go"
 SetCurDir "q:\test\go"
str go="C:\Go\bin\go.exe"
str sf="q:\test\go\src\test\hello.go"
str s=
 package main
 
 import "fmt"
 
 func main() {
     fmt.Printf("hello, world\n")
 }
s.setfile(sf)
PF
 RunConsole2(F"{go} run {sf}" 0 0 1)
 RunConsole2(F"{go} tool 8g {sf}" 0 0 0)
RunConsole2(F"{go} install test" 0 0 0)
 PN
 RunConsole2(F"{go} tool 8l q:\test\hello.8" 0 0 0)
 RunConsole2(F"{go} tool 8l hello.8" 0 0 0)
PN
PO

 Speed:
   Just to compile: 120 ms. With -N same. Can show assembler text, but the syntax is strange.
   Full (make exe): 650 without Avast, >=750 with Avast.
