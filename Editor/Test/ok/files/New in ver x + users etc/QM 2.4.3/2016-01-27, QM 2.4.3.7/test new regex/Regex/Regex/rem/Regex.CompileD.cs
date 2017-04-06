function $rx [options] [jitOptions] [pcre2_compile_context*c] ;;options: PCRE2_ANCHORED|PCRE2_ALLOW_EMPTY_CLASS|PCRE2_ALT_BSUX|PCRE2_ALT_CIRCUMFLEX|PCRE2_AUTO_CALLOUT|PCRE2_CASELESS|PCRE2_DOLLAR_ENDONLY|PCRE2_DOTALL|PCRE2_DUPNAMES|PCRE2_EXTENDED|PCRE2_FIRSTLINE|PCRE2_MATCH_UNSET_BACKREF|PCRE2_MULTILINE|PCRE2_NEVER_BACKSLASH_C|PCRE2_NEVER_UCP|PCRE2_NEVER_UTF|PCRE2_NO_AUTO_CAPTURE|PCRE2_NO_AUTO_POSSESS|PCRE2_NO_DOTSTAR_ANCHOR|PCRE2_NO_START_OPTIMIZE|PCRE2_NO_UTF_CHECK|PCRE2_UCP|PCRE2_UNGREEDY|PCRE2_UTF;  jitOptions: PCRE2_JIT_COMPLETE|PCRE2_JIT_PARTIAL_HARD|PCRE2_JIT_PARTIAL_SOFT

 Compiles a regular expression.
 Error if the regular expression is invalid.

 rx - regular expression string.
 options - flags to pass to pcre2_compile(). See above. <link "http://www.pcre.org/current/doc/html/pcre2api.html#SEC18">Reference</link>.
 jitOptions - if nonzero, this function calls pcre2_jit_compile() and passes these flags. See above. <link "http://www.pcre.org/current/doc/html/pcre2jit.html">Reference</link>.
 c - a context to pass to pcre2_compile(). Rarely used. You can create/modify/free contexts with PCRE2 API functions.

 REMARKS
 At first call Compile(). Then call Match() and/or other functions. The variable can be used to search in multiple different strings without recompiling.
 This function calls pcre2_compile() API. If jitOptions used and not 0, also calls pcre2_jit_compile() API.


int e=pcre2qm_compile(&this rx options jitOptions c)
if(e) end F"{e} {pcre2qm_last_error}"
