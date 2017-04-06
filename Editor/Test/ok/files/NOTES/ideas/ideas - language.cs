Add what is obviously missing in the language:
   all functions: optional arg default value
   COM functions: ref args: passing constant values
   COM functions: omitting optional nonVARIANT args
   etc

operator= for assignment of nonclass var to a class that has single member. If there is no such operator, warning when trying to assign.

Automatically find and compile class definitions.

Allow interfaces as base. Now class CTest :IAccessible'i generates error "cannot extend this type". Also Now class CTest :IAccessible treats IAccessible as int.

Use + to add 1 element to array, eg a[+] (same as a[]) or a[0 +] (instead of a[0 a.redim(-1)]). Or keyword 'add'.

sel: support regular expressions. User suggestion.

Implementing interfaces.

Allow to add member functions to ARRAY. Each array type (ARRAY(str), ARRAY(int), ...) would have separate member functions. Naming: ARRAY(type).Function.

out-: disable if in folder options checked "Disable out-".

Directive #if in ref files.

Add ASSERT, VERIFY, TRACE, STACK. Active only in Debug mode. Add function IsDebugMode.

Classes: static functions.

Allow to get address of member function. Initially this would be invalid but can be set using something like &this=lParam. Or/and support static member functions.

BSTR=L"string" and BSTR=word* should work.

Mark mes formatting (...) obsolete. Now can use F"" instead, which is easier and safer. Same with other similar functions (end). Mention F"" in out/mes/end help.

Use keyword 'throw' instead of 'end'. Use 'endthread' instead of 'end'. Make 'end' obsolete.

err throw instead of err end _error.

err+ goto _retry

finally. Also optionally go to finally on end macro from outside (eg use with atend).

else with err.

Optional arguments: allow @ for UDF.
   Then also need a function that tells whether an argument is not set (omitted or @).

Help source declaration, eg: help "url" identifier1 [...]

scope keywords. Examples:
	scope global int a b
	scope thread
		int a
		str s
		type TYP a b
	scope local
		dll file func a b
	scope(function1 Dialog2 ...)
		type TYP a b

sel.UDF([...]) case "one" ... case "two" ...
	UDF receives array of case strings.
	It can eg show a list box, menu, etc.

F"string {variable%%{sizeVariable}.{precisionVariable}s}".


 REJECTED

L"[350]" must generate UTF-16 character, like C++ does. Now QM truncates 350 to byte. Rejected because it anyway would be not convenient way to use Unicode. Now instead can simply type UTF-8 text and use @ operator. The limitation is documented.
