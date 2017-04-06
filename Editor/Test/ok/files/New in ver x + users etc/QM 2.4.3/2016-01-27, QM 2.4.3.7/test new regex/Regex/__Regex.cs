 PCRE2 API

#region def
def PCRE2_ALLOW_EMPTY_CLASS 0x00000001
def PCRE2_ALT_BSUX 0x00000002
def PCRE2_ALT_CIRCUMFLEX 0x00200000
def PCRE2_ANCHORED 0x80000000
def PCRE2_AUTO_CALLOUT 0x00000004
def PCRE2_BSR_ANYCRLF 2
def PCRE2_BSR_UNICODE 1
def PCRE2_CASELESS 0x00000008
def PCRE2_CODE_UNIT_WIDTH 8
def PCRE2_CONFIG_BSR 0
def PCRE2_CONFIG_JIT 1
def PCRE2_CONFIG_JITTARGET 2
def PCRE2_CONFIG_LINKSIZE 3
def PCRE2_CONFIG_MATCHLIMIT 4
def PCRE2_CONFIG_NEWLINE 5
def PCRE2_CONFIG_PARENSLIMIT 6
def PCRE2_CONFIG_RECURSIONLIMIT 7
def PCRE2_CONFIG_STACKRECURSE 8
def PCRE2_CONFIG_UNICODE 9
def PCRE2_CONFIG_UNICODE_VERSION 10
def PCRE2_CONFIG_VERSION 11
def PCRE2_DATE 0x000007BB
def PCRE2_DFA_RESTART 0x00000040
def PCRE2_DFA_SHORTEST 0x00000080
def PCRE2_DOLLAR_ENDONLY 0x00000010
def PCRE2_DOTALL 0x00000020
def PCRE2_DUPNAMES 0x00000040
def PCRE2_ERROR_BADDATA 0xFFFFFFE3
def PCRE2_ERROR_BADMAGIC 0xFFFFFFE1
def PCRE2_ERROR_BADMODE 0xFFFFFFE0
def PCRE2_ERROR_BADOFFSET 0xFFFFFFDF
def PCRE2_ERROR_BADOPTION 0xFFFFFFDE
def PCRE2_ERROR_BADREPLACEMENT 0xFFFFFFDD
def PCRE2_ERROR_BADUTFOFFSET 0xFFFFFFDC
def PCRE2_ERROR_CALLOUT 0xFFFFFFDB
def PCRE2_ERROR_DFA_BADRESTART 0xFFFFFFDA
def PCRE2_ERROR_DFA_RECURSE 0xFFFFFFD9
def PCRE2_ERROR_DFA_UCOND 0xFFFFFFD8
def PCRE2_ERROR_DFA_UFUNC 0xFFFFFFD7
def PCRE2_ERROR_DFA_UITEM 0xFFFFFFD6
def PCRE2_ERROR_DFA_WSSIZE 0xFFFFFFD5
def PCRE2_ERROR_INTERNAL 0xFFFFFFD4
def PCRE2_ERROR_JIT_BADOPTION 0xFFFFFFD3
def PCRE2_ERROR_JIT_STACKLIMIT 0xFFFFFFD2
def PCRE2_ERROR_MATCHLIMIT 0xFFFFFFD1
def PCRE2_ERROR_MIXEDTABLES 0xFFFFFFE2
def PCRE2_ERROR_NOMATCH 0xFFFFFFFF
def PCRE2_ERROR_NOMEMORY 0xFFFFFFD0
def PCRE2_ERROR_NOSUBSTRING 0xFFFFFFCF
def PCRE2_ERROR_NOUNIQUESUBSTRING 0xFFFFFFCE
def PCRE2_ERROR_NULL 0xFFFFFFCD
def PCRE2_ERROR_PARTIAL 0xFFFFFFFE
def PCRE2_ERROR_RECURSELOOP 0xFFFFFFCC
def PCRE2_ERROR_RECURSIONLIMIT 0xFFFFFFCB
def PCRE2_ERROR_UNAVAILABLE 0xFFFFFFCA
def PCRE2_ERROR_UNSET 0xFFFFFFC9
def PCRE2_ERROR_UTF16_ERR1 0xFFFFFFE8
def PCRE2_ERROR_UTF16_ERR2 0xFFFFFFE7
def PCRE2_ERROR_UTF16_ERR3 0xFFFFFFE6
def PCRE2_ERROR_UTF32_ERR1 0xFFFFFFE5
def PCRE2_ERROR_UTF32_ERR2 0xFFFFFFE4
def PCRE2_ERROR_UTF8_ERR1 0xFFFFFFFD
def PCRE2_ERROR_UTF8_ERR10 0xFFFFFFF4
def PCRE2_ERROR_UTF8_ERR11 0xFFFFFFF3
def PCRE2_ERROR_UTF8_ERR12 0xFFFFFFF2
def PCRE2_ERROR_UTF8_ERR13 0xFFFFFFF1
def PCRE2_ERROR_UTF8_ERR14 0xFFFFFFF0
def PCRE2_ERROR_UTF8_ERR15 0xFFFFFFEF
def PCRE2_ERROR_UTF8_ERR16 0xFFFFFFEE
def PCRE2_ERROR_UTF8_ERR17 0xFFFFFFED
def PCRE2_ERROR_UTF8_ERR18 0xFFFFFFEC
def PCRE2_ERROR_UTF8_ERR19 0xFFFFFFEB
def PCRE2_ERROR_UTF8_ERR2 0xFFFFFFFC
def PCRE2_ERROR_UTF8_ERR20 0xFFFFFFEA
def PCRE2_ERROR_UTF8_ERR21 0xFFFFFFE9
def PCRE2_ERROR_UTF8_ERR3 0xFFFFFFFB
def PCRE2_ERROR_UTF8_ERR4 0xFFFFFFFA
def PCRE2_ERROR_UTF8_ERR5 0xFFFFFFF9
def PCRE2_ERROR_UTF8_ERR6 0xFFFFFFF8
def PCRE2_ERROR_UTF8_ERR7 0xFFFFFFF7
def PCRE2_ERROR_UTF8_ERR8 0xFFFFFFF6
def PCRE2_ERROR_UTF8_ERR9 0xFFFFFFF5
def PCRE2_EXTENDED 0x00000080
def PCRE2_FIRSTLINE 0x00000100
def PCRE2_INFO_ALLOPTIONS 0
def PCRE2_INFO_ARGOPTIONS 1
def PCRE2_INFO_BACKREFMAX 2
def PCRE2_INFO_BSR 3
def PCRE2_INFO_CAPTURECOUNT 4
def PCRE2_INFO_FIRSTBITMAP 7
def PCRE2_INFO_FIRSTCODETYPE 6
def PCRE2_INFO_FIRSTCODEUNIT 5
def PCRE2_INFO_HASCRORLF 8
def PCRE2_INFO_JCHANGED 9
def PCRE2_INFO_JITSIZE 10
def PCRE2_INFO_LASTCODETYPE 12
def PCRE2_INFO_LASTCODEUNIT 11
def PCRE2_INFO_MATCHEMPTY 13
def PCRE2_INFO_MATCHLIMIT 14
def PCRE2_INFO_MAXLOOKBEHIND 15
def PCRE2_INFO_MINLENGTH 16
def PCRE2_INFO_NAMECOUNT 17
def PCRE2_INFO_NAMEENTRYSIZE 18
def PCRE2_INFO_NAMETABLE 19
def PCRE2_INFO_NEWLINE 20
def PCRE2_INFO_RECURSIONLIMIT 21
def PCRE2_INFO_SIZE 22
def PCRE2_JIT_COMPLETE 0x00000001
def PCRE2_JIT_PARTIAL_HARD 0x00000004
def PCRE2_JIT_PARTIAL_SOFT 0x00000002
def PCRE2_MAJOR 10
def PCRE2_MATCH_UNSET_BACKREF 0x00000200
def PCRE2_MINOR 20
def PCRE2_MULTILINE 0x00000400
def PCRE2_NEVER_BACKSLASH_C 0x00100000
def PCRE2_NEVER_UCP 0x00000800
def PCRE2_NEVER_UTF 0x00001000
def PCRE2_NEWLINE_ANY 4
def PCRE2_NEWLINE_ANYCRLF 5
def PCRE2_NEWLINE_CR 1
def PCRE2_NEWLINE_CRLF 3
def PCRE2_NEWLINE_LF 2
def PCRE2_NOTBOL 0x00000001
def PCRE2_NOTEMPTY 0x00000004
def PCRE2_NOTEMPTY_ATSTART 0x00000008
def PCRE2_NOTEOL 0x00000002
def PCRE2_NO_AUTO_CAPTURE 0x00002000
def PCRE2_NO_AUTO_POSSESS 0x00004000
def PCRE2_NO_DOTSTAR_ANCHOR 0x00008000
def PCRE2_NO_START_OPTIMIZE 0x00010000
def PCRE2_NO_UTF_CHECK 0x40000000
def PCRE2_PARTIAL_HARD 0x00000020
def PCRE2_PARTIAL_SOFT 0x00000010
def PCRE2_SUBSTITUTE_GLOBAL 0x00000100
def PCRE2_UCP 0x00020000
def PCRE2_UNGREEDY 0x00040000
def PCRE2_UNSET 0xFFFFFFFF
def PCRE2_UTF 0x00080000
def PCRE2_ZERO_TERMINATED 0xFFFFFFFF

def PCRE2_COMPILE_FLAGS PCRE2_ANCHORED|PCRE2_ALLOW_EMPTY_CLASS|PCRE2_ALT_BSUX|PCRE2_ALT_CIRCUMFLEX|PCRE2_AUTO_CALLOUT|PCRE2_CASELESS|PCRE2_DOLLAR_ENDONLY|PCRE2_DOTALL|PCRE2_DUPNAMES|PCRE2_EXTENDED|PCRE2_FIRSTLINE|PCRE2_MATCH_UNSET_BACKREF|PCRE2_MULTILINE|PCRE2_NEVER_BACKSLASH_C|PCRE2_NEVER_UCP|PCRE2_NEVER_UTF|PCRE2_NO_AUTO_CAPTURE|PCRE2_NO_AUTO_POSSESS|PCRE2_NO_DOTSTAR_ANCHOR|PCRE2_NO_START_OPTIMIZE|PCRE2_NO_UTF_CHECK|PCRE2_UCP|PCRE2_UNGREEDY|PCRE2_UTF
def PCRE2_JITCOMPILE_FLAGS PCRE2_JIT_COMPLETE|PCRE2_JIT_PARTIAL_HARD|PCRE2_JIT_PARTIAL_SOFT
def PCRE2_MATCH_FLAGS PCRE2_ANCHORED|PCRE2_NOTBOL|PCRE2_NOTEOL|PCRE2_NOTEMPTY|PCRE2_NOTEMPTY_ATSTART|PCRE2_NO_UTF_CHECK|PCRE2_PARTIAL_HARD|PCRE2_PARTIAL_SOFT
#endregion
#region type
type pcre2_callout_block version callout_number capture_top capture_last *offset_vector $mark $subject subject_length start_match current_position pattern_position next_item_length callout_string_offset callout_string_length $callout_string
type pcre2_callout_enumerate_block version pattern_position next_item_length callout_number callout_string_offset callout_string_length $callout_string

type pcre2_code __
type pcre2_match_data __
type pcre2_general_context __
type pcre2_compile_context __
type pcre2_match_context __
type pcre2_jit_stack __
#endregion
#region dll
dll- "$qm$\pcre2"
	[pcre2_callout_enumerate_8]#pcre2_callout_enumerate pcre2_code*code callback !*user_data ;;callback: function[c]# pcre2_callout_enumerate_block*b !*user_data
	[pcre2_code_free_8]pcre2_code_free pcre2_code*code
	[pcre2_compile_8]pcre2_code*pcre2_compile $pattern length options *errorcode *erroroffset pcre2_compile_context*ccontext
	[pcre2_compile_context_copy_8]pcre2_compile_context*pcre2_compile_context_copy pcre2_compile_context*ccontext
	[pcre2_compile_context_create_8]pcre2_compile_context*pcre2_compile_context_create pcre2_general_context*gcontext
	[pcre2_compile_context_free_8]pcre2_compile_context_free pcre2_compile_context*ccontext
	[pcre2_config_8]#pcre2_config what !*where
	[pcre2_dfa_match_8]#pcre2_dfa_match pcre2_code*code $subject length startoffset options pcre2_match_data*match_data pcre2_match_context*mcontext *workspace wscount
	[pcre2_general_context_copy_8]pcre2_general_context*pcre2_general_context_copy pcre2_general_context*gcontext
	[pcre2_general_context_create_8]pcre2_general_context*pcre2_general_context_create private_malloc private_free !*memory_data ;;private_malloc: function[c]!* size !*memory_data  ,  private_free: function[c] !*memory !*memory_data
	[pcre2_general_context_free_8]pcre2_general_context_free pcre2_general_context*gcontext
	[pcre2_get_error_message_8]#pcre2_get_error_message errorcode $buffer bufflen
	[pcre2_get_mark_8]$pcre2_get_mark pcre2_match_data*match_data
	[pcre2_get_ovector_count_8]#pcre2_get_ovector_count pcre2_match_data*match_data
	[pcre2_get_ovector_pointer_8]#*pcre2_get_ovector_pointer pcre2_match_data*match_data
	[pcre2_get_startchar_8]#pcre2_get_startchar pcre2_match_data*match_data
	[pcre2_jit_compile_8]#pcre2_jit_compile pcre2_code*code options
	[pcre2_jit_free_unused_memory_8]pcre2_jit_free_unused_memory pcre2_general_context*gcontext
	[pcre2_jit_match_8]#pcre2_jit_match pcre2_code*code $subject length startoffset options pcre2_match_data*match_data pcre2_match_context*mcontext
	[pcre2_jit_stack_assign_8]pcre2_jit_stack_assign pcre2_match_context*mcontext callback_function !*callback_data ;;callback_function: function[c]pcre2_jit_stack* !*callback_data
	[pcre2_jit_stack_create_8]pcre2_jit_stack*pcre2_jit_stack_create startsize maxsize pcre2_general_context*gcontext
	[pcre2_jit_stack_free_8]pcre2_jit_stack_free pcre2_jit_stack*jit_stack
	[pcre2_maketables_8]!*pcre2_maketables pcre2_general_context*gcontext
	[pcre2_match_8]#pcre2_match pcre2_code*code $subject length startoffset options pcre2_match_data*match_data pcre2_match_context*mcontext
	[pcre2_match_context_copy_8]pcre2_match_context*pcre2_match_context_copy pcre2_match_context*mcontext
	[pcre2_match_context_create_8]pcre2_match_context*pcre2_match_context_create pcre2_general_context*gcontext
	[pcre2_match_context_free_8]pcre2_match_context_free pcre2_match_context*mcontext
	[pcre2_match_data_create_8]pcre2_match_data*pcre2_match_data_create ovecsize pcre2_general_context*gcontext
	[pcre2_match_data_create_from_pattern_8]pcre2_match_data*pcre2_match_data_create_from_pattern pcre2_code*code pcre2_general_context*gcontext
	[pcre2_match_data_free_8]pcre2_match_data_free pcre2_match_data*match_data
	[pcre2_pattern_info_8]#pcre2_pattern_info pcre2_code*code what !*where
	[pcre2_serialize_decode_8]#pcre2_serialize_decode pcre2_code**codes number_of_codes !*bytes pcre2_general_context*gcontext
	[pcre2_serialize_encode_8]#pcre2_serialize_encode pcre2_code**codes number_of_codes !**serialized_bytes *serialized_size pcre2_general_context*gcontext
	[pcre2_serialize_free_8]pcre2_serialize_free !*bytes
	[pcre2_serialize_get_number_of_codes_8]#pcre2_serialize_get_number_of_codes !*bytes
	[pcre2_set_bsr_8]#pcre2_set_bsr pcre2_compile_context*ccontext value
	[pcre2_set_callout_8]#pcre2_set_callout pcre2_match_context*mcontext callout_function !*callout_data ;;callout_function: function[c]# pcre2_callout_block*b !*callout_data
	[pcre2_set_character_tables_8]#pcre2_set_character_tables pcre2_compile_context*ccontext !*tables
	[pcre2_set_compile_recursion_guard_8]#pcre2_set_compile_recursion_guard pcre2_compile_context*ccontext guard_function !*user_data ;;guard_function: function[c]# n !*user_data
	[pcre2_set_match_limit_8]#pcre2_set_match_limit pcre2_match_context*mcontext value
	[pcre2_set_newline_8]#pcre2_set_newline pcre2_compile_context*ccontext value
	[pcre2_set_parens_nest_limit_8]#pcre2_set_parens_nest_limit pcre2_compile_context*ccontext value
	[pcre2_set_recursion_limit_8]#pcre2_set_recursion_limit pcre2_match_context*mcontext value
	[pcre2_set_recursion_memory_management_8]#pcre2_set_recursion_memory_management pcre2_match_context*mcontext private_malloc private_free !*memory_data ;;private_malloc: function[c]!* size !*memory_data  ,  private_free: function[c] !*memory !*memory_data
	[pcre2_substitute_8]#pcre2_substitute pcre2_code*code $subject length startoffset options pcre2_match_data*match_data pcre2_match_context*mcontext $replacement rlength $outputbuffer *outlengthptr
	[pcre2_substring_copy_byname_8]#pcre2_substring_copy_byname pcre2_match_data*match_data $name $buffer *bufflen
	[pcre2_substring_copy_bynumber_8]#pcre2_substring_copy_bynumber pcre2_match_data*match_data number $buffer *bufflen
	[pcre2_substring_free_8]pcre2_substring_free $buffer
	[pcre2_substring_get_byname_8]#pcre2_substring_get_byname pcre2_match_data*match_data $name $*bufferptr *bufflen
	[pcre2_substring_get_bynumber_8]#pcre2_substring_get_bynumber pcre2_match_data*match_data number $*bufferptr *bufflen
	[pcre2_substring_length_byname_8]#pcre2_substring_length_byname pcre2_match_data*match_data $name *length
	[pcre2_substring_length_bynumber_8]#pcre2_substring_length_bynumber pcre2_match_data*match_data number *length
	[pcre2_substring_list_free_8]pcre2_substring_list_free $*list
	[pcre2_substring_list_get_8]#pcre2_substring_list_get pcre2_match_data*match_data $**listptr **lengthsptr
	[pcre2_substring_nametable_scan_8]#pcre2_substring_nametable_scan pcre2_code*code $name $*first $*last
	[pcre2_substring_number_from_name_8]#pcre2_substring_number_from_name pcre2_code*code $name
#endregion

 COM functions that call the PCRE2 API

type OFFSETS b e ;;Substring offsets: b - beginning, e - end. Length is e-b.

interface# IRegex :IUnknown
	#Match($s [flags]) ;;Returns: >0 if matches, 0 doesn't, -1 partial match (flags PCRE2_PARTIAL_x).
	#MatchFromTo($s iFrom iTo [flags]) ;;Returns: >0 if matches, 0 doesn't, -1 partial match (flags PCRE2_PARTIAL_x).
	#MatchNext($s [flags]) ;;Returns: >0 if matches, 0 doesn't.
	#Get([subMatch] [str&text] [int&length]) ;;subMatch: 0, submatch index or name.  Returns offset.
	#GetArray([ARRAY(str)&as] [ARRAY(OFFSETS)&ao]) ;;Returns match offset.
	#Find($s [subMatch] [str&text] [int&length] [flags]) ;;subMatch: 0, submatch index or name.  Returns match offset, or <0 if not found.
	#FindFromTo($s iFrom iTo [subMatch] [str&text] [int&length] [flags]) ;;subMatch: 0, submatch index or name.  Returns match offset, or <0 if not found.
	#FindGetArray($s [ARRAY(str)&as] [ARRAY(OFFSETS)&ao] [flags]) ;;Returns match offset, or <0 if not found.
	#FindGetArrayFromTo($s iFrom iTo [ARRAY(str)&as] [ARRAY(OFFSETS)&ao] [flags]) ;;Returns match offset, or <0 if not found.
	#FindNext($s [subMatch] [str&text] [int&length] [flags]) ;;subMatch: 0, submatch index or name.  Returns match offset, or <0 if not found.
	#FindNextGetArray($s [ARRAY(str)&as] [ARRAY(OFFSETS)&ao] [flags]) ;;Returns match offset, or <0 if not found.
	#FindAll($s [subMatch] [ARRAY(str)&as] [ARRAY(OFFSETS)&ao] [limit] [flags]) ;;subMatch: 0, submatch index or name, or -1 to get match and submatches (2-dim array).  Returns n found or 0.
	#FindAllFromTo($s iFrom iTo [subMatch] [ARRAY(str)&as] [ARRAY(OFFSETS)&ao] [limit] [flags]) ;;subMatch: 0, submatch index or name, or -1 to get match and submatches (2-dim array).  Returns n found or 0.
	#Replace(str&s [$repl] [limit] [flags]) ;;Returns n found or 0.
	#ReplaceFromTo(str&s iFrom iTo [$repl] [limit] [flags]) ;;Returns n found or 0.
	#ReplaceCallback(str&s cbFunc [cbParam] [limit] [flags]) ;;Returns n found or 0.
	#ReplaceCallbackFromTo(str&s iFrom iTo cbFunc [cbParam] [limit] [flags]) ;;Returns n found or 0.
	#Split($s [ARRAY(str)&as] [limit] [splitFlags] [ARRAY(OFFSETS)&ao] [matchFlags]) ;;splitFlags: 1 modify s, 2 get all right part.  Returns n tokens.
	#SplitFromTo($s iFrom iTo [ARRAY(str)&as] [limit] [splitFlags] [ARRAY(OFFSETS)&ao] [matchFlags]) ;;splitFlags: 1 modify s, 2 get all right part.  Returns n tokens.
	SetCallout(cbFunc [cbParam]) ;;cbFunc: menu -> File -> New -> Templates -> Callback -> Callback_IRegex_ReplaceCallback.
	[g]pcre2_code*code()
	[g]pcre2_match_data*match_data()
	[g]OFFSETS*offsets()
	[g]$mark()
	{335A7194-1572-4526-A8E7-242328162356}
dll- "$qm$\pcre2.dll" IRegex'CreateRegex $rx [flags] [jitFlags] [rxLen]

type REGEXREPLACECB IRegex'x $s str't number
def E_REGEX_BASE 0xC3570000 ;;base COM error code for IRegex errors. Eg if a PCRE2 API function returns -5, COM error code will be E_REGEX_BASE-5.

 Dll functions that call the COM API

type FROMTO from to
type REGEXOPTIONS compileFlags matchFlags fCallout fParam

dll- "$qm$\pcre2.dll"
	#RegexFind $s $rx [subMatch] [str&text] [int&length] [FROMTO&ft] [REGEXOPTIONS&op] ;;subMatch: 0, submatch index or name.  Returns match offset, or <0 if not found.
	#RegexFindGetArray $s $rx [ARRAY(str)&as] [ARRAY(OFFSETS)&ao] [FROMTO&ft] [REGEXOPTIONS&op] ;;Returns match offset, or <0 if not found.
	#RegexFindAll $s $rx [subMatch] [ARRAY(str)&as] [ARRAY(OFFSETS)&ao] [FROMTO&ft] [REGEXOPTIONS&op] ;;subMatch: 0, submatch index or name, or -1 to get match and submatches (2-dim array).  Returns n found or 0.
	#RegexReplace str&s $rx [$repl] [limit] [FROMTO&ft] [REGEXOPTIONS&op] ;;Returns n found or 0.
	#RegexReplaceCallback str&s $rx cbFunc [cbParam] [limit] [FROMTO&ft] [REGEXOPTIONS&op] ;;Returns n found or 0.
	#RegexSplit $s $rx [ARRAY(str)&as] [limit] [splitFlags] [ARRAY(OFFSETS)&ao] [FROMTO&ft] [REGEXOPTIONS&op] ;;splitFlags: 1 modify s, 2 get all right part.  Returns n tokens.
