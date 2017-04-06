 Converts QM-created exe from GUI to console.
 Use this function in 'Make exe' dialog, 'After' field.

 Example: <open>QM-console</open>.


str exe=_command
if(exe.endi(".qmm")) out "Function MakeExe_Console cannot be used with .qmm, must be .exe."; ret
_s="[3]" ;;IMAGE_OPTIONAL_HEADER::Subsystem = IMAGE_SUBSYSTEM_WINDOWS_CUI
_s.setfile(exe 0x16c 1)
ret 1
