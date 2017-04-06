 /
function[c] !*c nargs !**v ;;sqlite3_context*,int,sqlite3_value**
if(nargs!1) ret
_s=__sqlite.sqlite3_value_text(v[0])
_s.ucase
__sqlite.sqlite3_result_text(c _s _s.len __sqlite.SQLITE_TRANSIENT)
 out "%i %s  %i %s" na a nb b
