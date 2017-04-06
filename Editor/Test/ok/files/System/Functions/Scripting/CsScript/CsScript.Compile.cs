function $code $outputFile [flags] ;;flags: 1 classless function, 0x100 exe, 0x200 console exe, 0x300 dll, 0x1000 don't recompile always

 Compiles C# or VB.NET source code to a dll or exe file.

 code - source code. Can be string, file or macro, like with <help>CsScript.AddCode</help>.
 outputFile - file to create.
 flags:
   1, 0x10000 - same as with <help>CsScript.AddCode</help>.
   File type can be optionally specified: 
     0x100 - Windows exe.
     0x200 - console exe.
     0x300 - dll.
   If flags 0x100-0x300 not used, creates Windows exe if outputFile ends with .exe, else creates dll.
   0x1000 don't recompile if not outdated. Recompiles only if code or options changed. Note: appends some data to the assembly file.

 REMARKS
 Use this function when you need assembly file (.dll or .exe). If just need to run script, instead use AddCode etc.
 Unlike AddCode, this function does not load the assembly.
 By default compiles as C#. For VB call SetOptions("language=VB") before; then cannot be classless function, and flags 1 and 0x10000 are ignored.

 Errors: <.>

 See also: <CsScript.Load>


#exe addtextof "<script>"
opt noerrorshere 1
opt nowarningshere 1

Init

str ss saf sourceFile
code=_GetCode(code ss sourceFile flags)
x.Compile(code saf.expandpath(outputFile) flags sourceFile)
