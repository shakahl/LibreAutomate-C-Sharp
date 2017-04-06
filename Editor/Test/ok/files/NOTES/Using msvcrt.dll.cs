How to build projects with msvcrt.dll instead of msvcrxx.dll.

NOTE: don't do it where not necessary, because of AV FP.
   For example, Bitdefender and 2 related AV give FP for many such files, and don't when used static lib.
      These files were: qmrun, qmdd, qmserv, qmtul. Other files (no FP) where removed msvcrt: qmcl, setup.dll. Still using (no FP): qmexe, qmgrid.dll.

___________________________________________________________

METHOD 1

Works with VC9 but not with VC7 (2003).

Install Vista DDK (driver development kit). Install only if you don't have the required files (eg in DDK that is installed in other partition).
In VC lib folder, replace msvcrt.lib with the ddk msvcrt.lib. Or add to the project folder. It is in qmcore folder.
If need for 64-bit, replace msvcrt.lib in lib\amd64 subfolder with the ddk amd64 msvcrt.lib. Not tested, currently not using.
Copy msvcrt_win2000.obj from ddk lib folder to VC lib folder.

Use runtime library "Multithreaded DLL (/MD)".

Ignore libcmt.lib. Maybe the project does not use it, but libraries use.

For 64-bit projects, and other projects that run only on Vista and later, don't need more changes.

For other projects, add msvcrt_win2000.obj in additional dependencies.

___________________________________________________________

METHOD 2

Use project settings that currently are used by some app projects, eg qmcl.
   Particularly:
      In additional dependencies add msvcrt_vc6.lib (msvcrt.lib from VC6), and set to ignore specific library msvcrt.lib.
      Uncheck "buffer security check" and "enable C++ exceptions" (instead add /EHa in command line).
      Use Multithreaded library (not dll). Will be warning LNK4098. Without it, some intrinsic functions, eg __ftol, would be missing. If using msvcrt.dll from DDK, can use dll library, then links without the warning.
   Then don't need the DDK files.
   However this can be used only with simple 32-bit projects.
   With bigger projects may be error "this function already defined...".
   Then can try to remove libcmt.lib from default libs. But then may be missing some intrinsic functions.
   If project uses some library, eg qmcore for dll, may be linker error like "DllMain already defined". In additional dependencies the msvcrt libs must be after your lib.

___________________________________________________________

IN EXE PROJECT (VC7)

Cannot use method 1 because using VC7.

Use multithreaded dll. Also works with not dll, but _DLL is useful.

Add libraries: msvcrt_vc6.dll msvcrt.dll.
   The second is from DDK. Actually not necessary here if using multithreaded dll, but then would add two references to msvcrt.dll in exe, where part of functions is in one, part in other, don't know why.
   Without msvcrt_vc6.dll compiles, but adds two .CRT sections etc, and exe crashes, because ctors of globals not called.

Remove default library libcmt.lib.

Still adds two references to msvcrt.dll. The second contains just single function _ftol2. To solve it, add msvcrt.c to the project (not to qmcore); there is my version of the function. The function is available in msvcrt.dll on Windows 7, but not on XP.

Problems:
1. Missing function for double in output.c. Solution: we use sprintf from msvcrt.dll and ensure that decimal is '.'.
2. strtod depends on locale. Maybe there are more such functions. Solution: we use own strtod function.
___________________________________________________________

IN EXE PROJECT (VC9)

Use method 1.

Don't add msvcrt.c. Need it only for method 2 (VC7).
