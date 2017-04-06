 CsScript x.Load("$my qm$\test\ClassLibrary1\ClassLibrary1\bin\x86\Debug\ClassLibrary1.dll") ;;platform "x86", works on Win7 32 and 64 bit
CsScript x.Load("$my qm$\test\ClassLibrary1\ClassLibrary1\bin\x64\Debug\ClassLibrary1.dll") ;;platform "x64", error on Win7 32 and 64 bit: "An attempt was made to load a program with an incorrect format"
 CsScript x.Load("$my qm$\test\ClassLibrary1\ClassLibrary1\bin\Debug\ClassLibrary1.dll") ;;platform "Any CPU", works on Win7 32 and 64 bit
out x.Call("ClassLibrary1.Class1.Test")

 note: restart QM when switching platform, or may use the already loaded assembly instead
