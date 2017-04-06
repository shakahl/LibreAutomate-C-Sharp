[
 Script's main body (statements that are executed when the script is launched).
 Also variables of various storage/lifetime but available only in this script.
 Also other declarations (types, dll, etc) that are available only in this script.
]

[
 Private functions and classes. Available only in this script.
]

 The main body and each function can be preceded by one or more [optionX=...] and [trigger=...].

[
 Public namespaces. Available anywhere, unless somehow restricted eg to current file.
 Namespaces can contain classes, static functions, global/thread variables, various declarations, nested namespaces.
]

Class and namespace functions by default are public, but some can be made private. Probably don't need 'protected'; anyway nobody will use it; let inherited classes use private functions of base class.
Classes can contain nested classes and types.

EXAMPLE:

 Main script body:

[trigger=...]
[optionX=...]
int localVar
static int staticVar; ;;available only in this script, because is not in a class or namespace
thread int threadVar; ;;available only in this script, because is not in a class or namespace
local int scriptLocalVar; ;;local variable that is also available in called functions of this script

int w=win(...)
Click 10 20 w
int k=LocalFunc(w 1)


 Private functions and classes:

[trigger=...]
[optionX=...]
int LocalFunc(int w, int m)
{
...
}

class LocalClass
{
...
}

 Namespaces (can be used in any script, unless somehow restricted eg to current file):

namespace [My] ;;if namespace name omitted, uses User namespace (or System, if in System folder)
{
int staticVar; ;;available everywhere where this namespace is used
thread int threadVar; ;;available everywhere where this namespace is used

int StaticFunc(...)
{
...
}

class MyClass :BaseClassOrInterface
{
	[private] [static] int memberVar;
	
	[private] [static] int PublicFunc(...)
	{
	...
	}
	
	class NestedClass
	{
	...
	}
}

namespace NestedNS
{
...
}
}


 Or use directives:

#f[unction] [attributes]
int LocalFunc(int w, int m)
...


#namespace [My] [attributes] ;;if namespace name omitted, uses User namespace (or System, if in System folder)
int staticVar; ;;available everywhere where this namespace is used
thread int threadVar; ;;available everywhere where this namespace is used

#f [attributes]
int StaticFunc(...)
...

#class MyClass :BaseClassOrInterface
[private] [static] int memberVar;

#f [attributes] ;;attributes: static, private
int PublicFunc(...)
...

#class NestedClass
...
#end_class
#end_class

#namespace NestedNS

#end_namespace
