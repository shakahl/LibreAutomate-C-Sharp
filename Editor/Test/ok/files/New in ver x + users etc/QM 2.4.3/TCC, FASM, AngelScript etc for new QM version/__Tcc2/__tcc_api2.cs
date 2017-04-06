#ret
def TCC_OUTPUT_DLL 2
def TCC_OUTPUT_EXE 1
def TCC_OUTPUT_FORMAT_BINARY 1
def TCC_OUTPUT_FORMAT_COFF 2
def TCC_OUTPUT_FORMAT_ELF 0
def TCC_OUTPUT_MEMORY 0
def TCC_OUTPUT_OBJ 3
def TCC_OUTPUT_PREPROCESS 4
dll "$qm$\tccqm\tcc" #tcc_add_file !*s $filename
dll "$qm$\tccqm\tcc" #tcc_add_include_path !*s $pathname
dll "$qm$\tccqm\tcc" #tcc_add_library !*s $libraryname
dll "$qm$\tccqm\tcc" #tcc_add_library_path !*s $pathname
dll "$qm$\tccqm\tcc" #tcc_add_symbol !*s $name val
dll "$qm$\tccqm\tcc" #tcc_add_sysinclude_path !*s $pathname
dll "$qm$\tccqm\tcc" #tcc_compile_string !*s $buf
dll "$qm$\tccqm\tcc" tcc_define_symbol !*s $sym $value
dll "$qm$\tccqm\tcc" tcc_delete !*s
dll "$qm$\tccqm\tcc" !*tcc_get_symbol !*s $name
dll "$qm$\tccqm\tcc" !*tcc_new
dll "$qm$\tccqm\tcc" #tcc_output_file !*s $filename
dll "$qm$\tccqm\tcc" #tcc_relocate !*s1 !*ptr
dll "$qm$\tccqm\tcc" tcc_set_error_func !*s !*error_opaque fa_error_func
dll "$qm$\tccqm\tcc" tcc_set_lib_path !*s $path
 dll "$qm$\tccqm\tcc" tcc_set_options !*s $options
dll "$qm$\tccqm\tcc" #tcc_set_output_type !*s output_type
dll "$qm$\tccqm\tcc" tcc_undefine_symbol !*s $sym

dll "$qm$\tccqm\tcc" tcc_set_codepage cp
dll "$qm$\tccqm\tcc" tcc_quick_options !*s flags
dll "$qm$\tccqm\tcc" !*tcc_alloc_code_memory nBytes
dll "$qm$\tccqm\tcc" tcc_free_code_memory !*mem
dll "$qm$\tccqm\tcc" tcc_set_printf _printf
dll "$qm$\tccqm\tcc" tcc_set_getlib _getlib
dll "qm.exe" #qm_tcc_getlib_addr
