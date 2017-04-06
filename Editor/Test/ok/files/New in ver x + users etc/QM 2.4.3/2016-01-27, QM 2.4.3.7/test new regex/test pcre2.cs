out
ref __pcre2 "$qm$\pcre2.txt"

str s="one 12 two"
str rx="\d+"

int en eo
byte* re=__pcre2.pcre2_compile(rx rx.len 0 &en &eo 0)
if(re=0) end "pcre2_compile failed"

byte* match_data=__pcre2.pcre2_match_data_create_from_pattern(re 0)

int R=__pcre2.pcre2_match(re s s.len 0 0 match_data 0)
out R
if R>0
	int* v=__pcre2.pcre2_get_ovector_pointer(match_data)
	int i
	for i 0 R
		out F"{v[i*2]} {v[i*2+1]}"
	

__pcre2.pcre2_match_data_free(match_data)
__pcre2.pcre2_code_free(re)
