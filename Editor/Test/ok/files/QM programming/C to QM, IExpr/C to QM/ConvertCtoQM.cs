 /CtoQM
function $src_file $dest_file [$include_dir] [$preproc_def] [flags] [$dll_dn_file] [$dll_an_file] [$pch_file] [$dll_list] [$ref_list] [$type_map]

 Converts C declarations to QM.
 Tested (and works well) mostly with Microsoft Platform SDK header files (windows.h, etc). Also can be used to convert other header files. Should not be used to convert MFC/ATL header files (afxwin.h, etc) and other files that contain mostly C++ declarations, such as classes (read below).
 Requires QM 2.2.0 or later. Also uses ctoqm.dll, which must be in qm folder.

 src_file - C source or header file containing declarations or/and #include directives.
 dest_file - text file where to save QM declarations.
 include_dir - list of directories containing system header (.h) files.
    Can be "" or omitted if system header files are not used.
 preproc_def - list of preprocessor definitions in form "NAME1 value1[]NAME2 value2".
    Defaults include WINVER 0x0600, _WIN32_WINNT 0x0600, NTDDI_VERSION 0x06000000, _WIN32_IE 0x0700.
    It is for Vista/IE7. You can override them to convert for other Windows/IE versions.
 flags -
    1 - display included files.
    2 - only preprocess and create dest_file_preprocessed.txt.
    4 - compact.
    8 - create dest_file_pch.txt which can be used later as pch_file.
    16 - don't show results.
    32 - dont add default definitions and types.
    64 - need classes (only member variables).
    128 - declare dlls as delayloaded (dll-).
 dll_dn_file - file that provides dll names for dll function declarations. Read more below.
 dll_an_file - file that provides argument names for dll function declarations. Read more below.
 pch_file - file that contains C macros, typedefs and other info previously extracted from system header files (flag 8). Use it if you don't have system header files or don't want to add declarations from them to dest_file. You can use winapiv_pch.txt, which was created with winapi.txt (WINAPI reference file).
 dll_list - list of dlls. Used to get dll names for dll function declarations. If a function is found in a dll, the name of the dll is used in function's declaration. Searched before dll_dn_file.
 ref_list - list of external ref names, eg "ref REF1[]ref REF2". The list will be added at the beginning of the file.
 type_map - list that maps additional C types to QM types (can be type characters). Example: "Ctype1 QMtype1[]Ctype2 QMtype2".

 Processes #if, #include and other directives, expands macros, like a C preprocessor. Includes other header files.
 Like when compiling a C/C++ program, header files that use something from other files must #include those header files. For example, if uses LPSTR, HWND and other things defined in windows.h, must #include <windows.h>. Alternatively, use pch_file.
 You can see initial preprocessor definitions in ConverCH.AddDef. Versions are set to Windows 2003, IE 6. To change, add more or delete, you can edit it or use #undef/#define in your source file (to change, #undef is necessary).
 Conversion is not perfect. It is impossible to convert everything, because QM does not support everything in C/C++.
 Some errors and incomplete conversions are possible. Some declarations are followed by comments that shoud help to manually complete conversion, correct errors, etc.
 Some C statements that fails to convert are displayed in output. Others, known as cannot be converted, are skipped silently.
 Many C++ keywords that are not supported by QM are removed in ConvertCH.Tidy and other functions. Examples: const, virtual, __stdcall, __declspec(something). However, not all are removed, especially those added to newer C++ versions. These keywords will cause errors. Edit ConvertCH.Tidy so that they would be removed.
 Some declarations that cannot be converted to QM, can be useful for reference when manually converting C declarations or code. They are placed to dest_file_other.txt. They also are placed in the same file (dest_file.txt), unless flag 4 is used.
 Also may create several other files, documented in other functions.
 If flag 4 is used, removes some not very useful declarations, eg UNICODE versions of functions, functions with unknown dll name, typedefs, etc.

 Converts:
  #define -> def.
    Typecasts are removed, which can cause incorrect value in some cases. Read more in ConvertCH.CalcConst.
  #define function-style -> dll.
    These are not supported by QM, and therefore are followed by commented definition which must be used instead.
  #define with no value -> def.
  #define function names, etc -> used to replace function names, etc. Not included as def.
  enum -> def.
  const -> def. Not all can be converted. Little tested.
  GUID definitions -> def. Also attached to interfaces.
  dll functions -> dll.
    Since C function declarations don't include dll names, dll names in QM declarations will be ??? by default. To add dll names, do one of the following, or both:
      1. Provide a list of dlls (dll_list) that contain these functions.
      2. At first run a macro that calls function CH_GetDllNames and saves its output to a file. Then use that file as dll_dn_file when calling ConvertCtoQM. Read more in ConvertCH.GetDllName.
    Also, some C declarations may be without argument names (declared are only types). Then for argument names are used a, b, and so on, unless found in dll_an_file. Read more in ConvertCH.GetArgNames.
  struct, union -> type.
    Bitfield members are joined to single member, and its name begins with bf.
    Read more in ConvertCH._struct.
  interfaces (struct containing only pure virtual functions) -> interface.
  typedef -> type.
  typedef of callback functions -> type, followed by commented function statement that can be used in the callback function.
    Commented function statement also is appended to declarations of dll and interface functions where used, in next line.
    Don't confuse function type names with function names that are used in documentation. Function names are not declared in C header files. You can use any names. Function types are not used in QM, and are converted to int.

 Does not convert:
  Source code. Converts only declarations.
  #using and #import directives. They are displayed in output and skipped.
  #pragma directives, except #pragma once and #pragma pack. They are just skipped.
  Predefined C macros that require special processing, such as __FILE__, __DATE__, __FUNCTION__.
  Interfaces that don't have a base interface.
  Most C++ declarations. Most of them are listed below.
  Classes. Skipped.
  Namespaces. Members are included as globals.
  Templates. Skipped.
  Operators ::, ., ->.
  Overloaded operators. Skipped.
  Inline functions. Skipped.
  Attributes.
  MFC, ATL, STL, etc, because contain many C++ declarations.

 You can read more in other functions in this folder.


#compile ctoqm_dll

type CHRX ~define ~definef ~expand ~sp1 ~sp2 ~cast ~numberL ~numberue1 ~numberue2
type CHDLL ~name h

class ConvertCH
	~m_s ~m_file ~m_dest ~m_sInclDir ~m_sFuncDll ~m_sFuncArgsWin ~m_sFuncArgsCrt
	IStringMap'm_mf IStringMap'm_mfcb IStringMap'm_mtd IStringMap'm_mt IStringMap'm_mi IStringMap'm_mc IStringMap'm_mg IStringMap'm_mcf IStringMap'm_mtag
	IStringMap'm_mh IStringMap'm_mfan IStringMap'm_mfdn IStringMap'm_mcomm IStringMap'm_mall IStringMap'm_mut IStringMap'm_mpch IStringMap'm_mo
	IExprC'm_expr IExprC'm_expr2
	ARRAY(str*)m_aems ARRAY(CHDLL)m_adll
	!m_crt !m_outIncl !m_compact !m_pch !m_needclasses !m_delayload !m_pack
	~m_ps ;;packing stack
	CHRX'm_rx
 ;;don't reorder second line, because it is usead as array by PCH functions!

ConvertCH c.Main(src_file dest_file include_dir preproc_def flags dll_dn_file dll_an_file pch_file dll_list ref_list type_map)
