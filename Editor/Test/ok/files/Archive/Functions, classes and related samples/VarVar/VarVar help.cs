 This class allows to use variables whose names can be created at run time, eg from values of other variables.

 Add() creates variable and optionally sets value.
 Set() sets value.
 GetLpstr() gets value as string.
 GetInt() gets value as integer.

 Add2 and other "2" functions allow to compose variable names from 2 parts.

 Add() functions trow error if the variable already exists.
 Other functions throw error if the variable does not exist.
 The errors are thrown at run time.

 The scope (local, thread or global) of the variables is the same as of the VarVar variable.
 The variables don't have a type.
    With Add() and Set() functions, value can be string or numeric.
    To get value of lpstr type, use GetLpstr() functions. The value is temporary, so please assign it to a str variable if need to use later.
    To get value of int type, use GetInt() functions.
    To get value of other types (double, long), add member functions that would get lpstr and convert it to other type.

 EXAMPLES

#compile "__VarVar"
VarVar v

str s="apple"
int i=2

v.Add(s "green") ;;createvariable apple="green"
out v.GetLpstr(s) ;;out apple
v.Set(s 5) ;;apple=5
out v.GetInt(s) ;;out apple

v.Add2(s i "red") ;;createvariable apple2="red"
out v.GetLpstr2(s i) ;;out apple2
v.Set2(s i 10) ;;apple2=10
out v.GetInt2(s i) ;;out apple2
