function# [$_type]

 Makes unique name for a new local variable in current macro.

 this - variable name.
   Initially must contain suggested name. Must not be empty or invalid.
   If it is valid for a new variable (a variable etc does not exist with this name), returns 0.
   Else appends a number >0 (eg x -> x1, or x1 -> x2) and returns the number.
 _type - required type, eg "int" or "int*".
   If used, returns -1 if a variable of the type already exists. Then does not change sVar.

 Also searches variables previously used with this func in this thread.
 Currently ignores #sub.


str-- t_prevVars
int R=__LocalVarUniqueName(s &s t_prevVars _type)
if(empty(_type)) _type="ARRAY" ;;will add as existing variable, but type will not match
if(R>=0) t_prevVars.formata("%s %s;" _type s)
ret R
