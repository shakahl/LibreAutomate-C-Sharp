function# $name

int R=pcre2_substring_number_from_name(_p name)
if(R>=0 or R!=PCRE2_ERROR_NOUNIQUESUBSTRING) ret R

lpstr f last
int r offs step=pcre2_substring_nametable_scan(_p name &f &last)
if step>0
	for f f last+1 step
		r=f[0]<<8|f[1]
		offs=_v[r*2]; if(offs>=0) ret r ;;find first found
		if(R<0) R=r
ret R

 why pcre2 does not have an easy API to get sub offset by name?
