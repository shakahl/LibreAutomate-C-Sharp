 For C/C++ programmers.

 Quickly compiles C source code so that C functions can be immediately used in QM macros.
 C source code can be in a string, macro or c files. Compiled code is in memory.
 Also can create dll and exe files (console and GUI).

 Uses Tiny C Compiler library (TCC): <link>http://bellard.org/tcc/</link>

 WHERE IT CAN BE USED
 Faster low level data processing. Large strings, arrays, images, etc. Faster sorting.
    In loops with character or array element access and calculations and without calling slow functions:
       TCC compiled code is 15-50 times faster than QM.
       Almost same speed as MSVC code without optimizations.
       2-5 times slower than MSVC optimized code or assembler.
 Using existing C source code without converting to QM.
    However can use only C code, not C++. No classes, etc.
 Creating tiny dlls and exes.
 Global in-process hooks. Injecting dlls into other processes.
 Using assembler.

 REMARKS
 TCC C extensions are not compatible with MSVC extensions. You can read more about TCC in its website.
 Only 32-bit exes and dlls can be created. The dlls cannot be injected in 64-bit processes.
 Compiled in-memory code generally cannot be copied to other place and executed there, unless it does not call external functions.
 C functions are called through QM call() function. It does not allow them to return values of types bigger than 4 bytes. But you can use pointer parameters for it.
 Or you can declare and call C functions like dll functions. See <help>dll</help>.
 __Tcc in exe:
    Use __Tcc in exe only if you really need to generate code at run time. Otherwise create and use dll instead. 
    If you use __Tcc in exe, take $qm$\tcc folder together.
    Also add macros containing C source code (use <help>#exe</help> addtextof).
 Warning: Many antivirus programs detect false positives in exe and dll files created by TCC, because they are very small and compiled with a nonstandard compiler.

 <open "C example - C code in macro">More examples</open>

 Added in: QM 2.3.2.

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
