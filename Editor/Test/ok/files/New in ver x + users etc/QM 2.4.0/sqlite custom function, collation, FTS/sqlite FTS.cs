out
dll "qm.exe" Sqlite*__FtsDB

Sqlite& x=__FtsDB
ARRAY(str) a
str q
 q="text MATCH 'database'"
 q="name MATCH 'database'"
 q="any MATCH 'database'"
 q="text MATCH 'resto*'"
 q="any MATCH 'name:performance'"
 q="any MATCH 'trigger:Dreamwea* str'"
 q="name MATCH 'trigger:Dreamwea* Dreamwea*'"
 q="any MATCH 'where all words'"
 q=
  any MATCH '"get from"'
 q=
  text MATCH '"mous* pos*"'
 q="text MATCH 'Excel NEAR database'"
 q="text MATCH 'Excel NEAR/4 database'"
 q="text MATCH 'create NEAR Excel NEAR database'"
 q="text MATCH 'Excel OR database'"
 q="text MATCH 'Excel -data*'"
 q="text MATCH 'Excel AND (database OR file)'"
 q="text MATCH 'select from text name trigger'"
q="text MATCH 'ShowDialog' LIMIT 10" ;;QM crashes if without limit; then 817 results; not tested with SqliteStatement.

 x.Exec(F"SELECT name FROM any WHERE {q}" a)
x.Exec(F"SELECT name,snippet(any) FROM any WHERE {q}" a)
 x.Exec(F"SELECT name,snippet(any,'''','''',''...'',-1,15) FROM any WHERE {q}" a)

 ___________________________
int i
 for(i 0 a.len) out a[1 i]
for(i 0 a.len)
	out F"<><c 0xff>{a[0 i]}</c>[]{a[1 i]}"
	 out F"<><c 0xff>{a[0 i]}</c>[]<code>{a[1 i]}</code>" ;;conflicts with <b> by default
out a.len

 TODO: test "unicode61" tokenizer.
