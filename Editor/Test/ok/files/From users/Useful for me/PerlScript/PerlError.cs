 /
function$ MSScript.ScriptControl'PerlScript

out "Inside PerlError"

MSScript.Error e=PerlScript.Error
str ed(e.Description) et(e.Text) es(e.Source)
str- errstr.format("%s: %s[][9]Line %i: %s" es ed e.Line et)
e.Clear
ret errstr
