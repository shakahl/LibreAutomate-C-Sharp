 Converting C header file to ODL format (for making type library).

 Collect #define macros (except function-like) from this h file, and covert
 them to const. Use HI_define to const macro.

 Preprocess cpp file which includes windows.h and this header (set /P /EP compiler options).
 Rename from .i to .h, add to project, and delete part before this header.

 Add idl file which has this:
 [uuid(guid)]
 library abc
 {
 #include "header.h"
 }
 Use HI_Create GUID macro to create guid.

 Prepare file with HI_PrepareFile (remove emty lines, __stdcall, etc).

 Collect (manually) all functions, place at end, remove W functions,
 remove callback functions, and make module:
 [dllname("hhh.dll")]
 {
 all function prototypes
 }

 Add entry attribute to each function. Use HI_entry macro.

 Structures and enumerations does not require manual converting.

 Debug.
