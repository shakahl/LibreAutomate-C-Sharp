 /
function# $s $rx [compFlags] [&length]

 compFlags - pcre2_compile flags (not pcre2_match flags)

 ref __pcre2 "$qm$\pcre2.txt" ;;in init2
opt noerrorshere

int en eo
byte* re=__pcre2.pcre2_compile(rx -1 compFlags &en &eo 0)
if(re=0) end "pcre2_compile failed"
PN

byte* match_data=__pcre2.pcre2_match_data_create_from_pattern(re 0)
int R=__pcre2.pcre2_match(re s -1 0 0 match_data 0)
 out R
if R>0
	int* v=__pcre2.pcre2_get_ovector_pointer(match_data)
	 int i
	 for i 0 R
		 out F"{v[i*2]} {v[i*2+1]}"
	R=v[0]; if(&length) length=v[1]-R

__pcre2.pcre2_match_data_free(match_data)
__pcre2.pcre2_code_free(re)

if(R<=0 and R!-1) end F"match failed, error {R}"
ret R
