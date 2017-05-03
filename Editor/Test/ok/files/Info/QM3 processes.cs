PROCESSES

CatkeysEditor.exe - editor, manager. Also can run QM2 macros. Native, 32-bit. Initially use qm.exe of QM2; just add ability to edit and launch QM3 C# scripts.
CatkeysTasks.exe - appdomains for running scripts. Managed, anyCPU. Any UAC IL (set in Options, default Admin; or use multiple instances when need, for each IL and 32/64 bit). The C# compiler also is loaded here, and unloaded when don't need.
ScriptName.exe - scripts running in separate process. Also scripts running on computers without QM. Can be anyCPU or 32-bit or 64-bit. Any UAC IL (set in Properties). Option to have multiple appdomains like CatkeysTasks.exe.

TRIGGERS

CatkeysTriggers.exe - triggers. Native, 32-bit. Used by CatkeysTasks.exe and ScriptName.exe, also on computers without QM. Normally UAC Admin or uiAccess.

Or, don't use CatkeysTriggers.exe. Instead, triggers are in managed dlls that are loaded in CatkeysTasks.exe and optionally can be loaded in ScriptName.exe. Then need to use common trigger engine/tables.

Or, triggers are in CatkeysTasks.exe. Then ScriptName.exe that need triggers will need CatkeysTasks.exe, either as separate process or as a loaded managed or unmanaged dll.

COMMUNICATION

CatkeysTasks on trigger (or on command from CatkeysEditor) looks whether the script is compiled.
If not compiled, loads compiler in separate domain (if not loaded), and sends command to compile the script.
Compiler, if compiled successfully, sends command to CatkeysTasks main domain to execute the assembly.
If there are errors or warnings, compiler sends command to CatkeysEditor to display the errors/warnings. Can run CatkeysEditor if not running.
