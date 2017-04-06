#ret
def TCC_OUTPUT_DLL 2
def TCC_OUTPUT_EXE 1
def TCC_OUTPUT_FORMAT_BINARY 1
def TCC_OUTPUT_FORMAT_COFF 2
def TCC_OUTPUT_FORMAT_ELF 0
def TCC_OUTPUT_MEMORY 0
def TCC_OUTPUT_OBJ 3
def TCC_OUTPUT_PREPROCESS 4
dll "$qm$\tcc\tcc" int'tcc_add_file !*s $filename
dll "$qm$\tcc\tcc" int'tcc_add_include_path !*s $pathname
dll "$qm$\tcc\tcc" int'tcc_add_library !*s $libraryname
dll "$qm$\tcc\tcc" int'tcc_add_library_path !*s $pathname
dll "$qm$\tcc\tcc" int'tcc_add_symbol !*s $name !*val
dll "$qm$\tcc\tcc" int'tcc_add_sysinclude_path !*s $pathname
dll "$qm$\tcc\tcc" !*tcc_alloc_code_memory int'nBytes
dll "$qm$\tcc\tcc" int'tcc_compile_string !*s $buf
dll "$qm$\tcc\tcc" tcc_define_symbol !*s $sym $value
dll "$qm$\tcc\tcc" tcc_delete !*s
dll "$qm$\tcc\tcc" tcc_enable_debug !*s
dll "$qm$\tcc\tcc" tcc_free_code_memory !*mem
dll "$qm$\tcc\tcc" !*tcc_get_symbol !*s $name
dll "$qm$\tcc\tcc" !*tcc_new
dll "$qm$\tcc\tcc" int'tcc_output_file !*s $filename
dll "$qm$\tcc\tcc" int'tcc_relocate !*s1 !*ptr
dll "$qm$\tcc\tcc" int'tcc_run !*s int'argc $*argv
dll "$qm$\tcc\tcc" tcc_set_error_func !*s !*error_opaque int'fa_error_func
dll "$qm$\tcc\tcc" tcc_set_lib_path !*s $path
dll "$qm$\tcc\tcc" int'tcc_set_output_type !*s int'output_type
dll "$qm$\tcc\tcc" int'tcc_set_warning !*s $warning_name int'value
dll "$qm$\tcc\tcc" tcc_undefine_symbol !*s $sym
dll "$qm$\tcc\tcc" tcc_disable_warnings !*s
dll "$qm$\tcc\tcc" tcc_set_codepage cp