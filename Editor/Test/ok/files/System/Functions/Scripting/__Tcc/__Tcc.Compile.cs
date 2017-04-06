function'int* $code $func [flags] [$libList] [$qmFuncList] [int*qmFuncAddrArr] [$sysIncludePaths] [$includePaths] [$libPaths] [$preprocDef] ;;flags: 0-3 output format (0 memory, 1 exe, 2 dll, 3 o), 8 store function addresses in tail, 16 no warnings, 32 compile once

 Compiles C code.
 Returns array of function addresses (read below).
 Error if fails.

 code - one of:
    1. String containing C source code.
    2. Name of QM item (macro etc) that contains C source code.
       Syntax:
          "*MacroName" or "macro:MacroName" (QM 2.3.5) - compile text of the macro.
          "" - compile text of the caller.
       If the text contains line '#ret', compiles only what is after.
    3. List of C source files and/or other TCC-supported files, like ":$myqm$\c\f1.c[]$myqm$\c\f2.o". Compiles and links all.
 func - one of:
    1. List of C functions whose addresses you need (multiline).
       After compiling, address of the first specified function will be in member f.
       Addresses of all specified functions will be in an internal array. Returns address of its first element.
    2. If flags 1-3 used, func must be the output file (.exe, .dll or .o). Then Compile creates the file and returns 0. Should be full path.
 flags:
    0-3 - compiled code format:
       0 (default) - in memory.
       1, 2, 3 - .exe, .dll or .o (ELF object) file. Saves in file specified by func.
    8 - store function addresses (specified in func) in int variables that in memory are immediately after the __Tcc variable. <open "C example - multiple functions">Example</open>. 
    16 - don't show errors and warnings while compiling. On error, error description will be available.
    32 - if already compiled, don't recompile. Although you can use a global variable to check if already compiled, add this flag anyway if need to make thread-safe.
 libList - list of libraries and other files, like "user32[]mydll[]$my qm$\f1.c[]$my qm$\f2.o".
    QM 2.4.2. Can be any files supported by TCC. Previously could be just filenames of library files.
    Supported libraries: .def (contain names of dll functions that you can use), .dll, .rsrc, .a (ELF library). Can be filename or full path.
    Other supported files: .c, .s, .S, .o. They will be compiled/linked. Should be full path.
    Always adds these libraries: msvcrt, kernel32, libtcc1. For GUI exe and dll also adds user32 and gdi32. These files are in $qm\tcc\lib.
 qmFuncList - list (multiline) of user-defined functions that will be called from C code as callback functions. Must have __cdecl calling convention (function[c]). <open "C example - call QM functions">Example</open>.
 qmFuncAddrArr - array of addresses of functions specified in qmFuncList. Must be address of first element of a caller-allocated array.
 sysIncludePaths - additional folders for #include <...>. Multiline list. Always added: "$qm$\tcc\include", "$qm$\tcc\include\winapi".
 includePaths - folders for #include "...". Multiline list.
 libPaths - folders for files specified in libList. Multiline list. Always added: "$qm$\tcc\lib".
 preprocDef - list of preprocessor definitions. Each line contains name, and optionally value after space. Example: "_DEBUG[]WINVER 0x0601"

 REMARKS
 The compiled code is in memory. The __Tcc variable manages the memory. The code becomes invalid after destroying the variable.
 To call functions, you can use <help>call</help>. See also <help>dll</help>.
 When testing created dll files, may be useful <help>UnloadDll</help>.

 Errors: <Scripting_GetCodeFromMacro>, <__Tcc.__Compile>

 <open "C example - C code in macro">More examples</open>

 EXAMPLE
 lpstr c=
  int add(int a, int b)
  {
  printf("a=%i, b=%i", a, b);
  return a+b;
  }
 
 __Tcc x.Compile(c "add")
 int r=call(x.f 1 2)
 out r


#exe addtextof "<script>"

lock tcc
if(flags&32 and m_code) ret m_code
__Clear

flags&0xffff; m_flags=flags

m_iid=Scripting_GetCode(code _s 1)

ret __Compile(code func libList qmFuncList qmFuncAddrArr sysIncludePaths includePaths libPaths preprocDef)

err+
	if(m_flags&16 and m_ew.len)
		m_ew.rtrim
		m_ew.findreplace("[]" "[][9]")
		_error.description.formata("[][9]%s" m_ew)
	end _error
