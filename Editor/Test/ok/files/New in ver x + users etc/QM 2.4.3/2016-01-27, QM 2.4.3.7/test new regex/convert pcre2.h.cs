out

WakeCPU

 __testq__
str h="$qm$\app_plus\pcre2\pcre2.h"
str t="$qm$\pcre2.txt"
lpstr incl=
 $program files$\Microsoft Visual Studio 9.0\VC\include
lpstr D="HAVE_CONFIG_H 1[]PCRE2_CODE_UNIT_WIDTH 8"
lpstr tm="pcre2_real_code_8 ![]pcre2_real_general_context_8 ![]pcre2_real_compile_context_8 ![]pcre2_real_match_context_8 ![]pcre2_real_match_data_8 ![]pcre2_real_jit_stack_8 !"
 lpstr tm="pcre2_real_code_8 pcre2_code[]pcre2_real_general_context_8 ![]pcre2_real_compile_context_8 ![]pcre2_real_match_context_8 ![]pcre2_real_match_data_8 ![]pcre2_real_jit_stack_8 !"
 PF
PerfOut 1
 ConvertCtoQM h t incl D 128|4|32 "" "" "" "$qm$\pcre2" "" tm
ConvertCtoQM h t incl D 128|4|32 "" "" "" "$qm$\pcre2"
 PN;PO
PerfOut 3

 all functions and types have two versions: normal and with _8 suffix. Remove all with _8.
str s.getfile(t)
s.replacerx("^([^\[\]\r\n]+)\w+_8(?!]).+\r\n" "" 8)
s.setfile(t)

out "<>Tasks:  <open>Check ref</open>."

 dll 112, type 4, interface 0, def 217, guid 0, typedef 54, callback 2, added 256
 speed: 28354  old
 speed: 32435  new, no UTF (with UTF 90)
 speed: 28388  new, cache, no UTF
 speed: 25774  new, cache, jit, no UTF

 old: 7 ms
 new: 11 ms
 new cached: 6.7 ms
 new cached, jit: 5.4 ms
