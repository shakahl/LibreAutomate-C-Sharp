function$ $winHandle

 Replaces {window} in this with winHandle.
 This should be child(...) or id(...).

if(s.beg("id(")) s.findreplace("{window}" winHandle 4)
else if(s.beg("child(")) s.replacerx("(''.*?'' ''.*?'' )\{window\}" F"$1{winHandle}" 4)
ret s
