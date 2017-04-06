Hi

I was looking for an embeddable scripting language similar to C++. AngelScript is the most close to what I need, and I like it.
Here are my suggestions. Most already suggested by others in the forum, some in your TODO, but anyway.
For most of the language suggestions I have this in mind: make the language more friendly for non-programmers. Shorter script code, more relaxed rules, etc.

LANGUAGE (the suggestions are sorted by the importance)

Exception handling.
I understand that implementing the handling of true exceptions (hardware/OS/C++) is difficult etc. And not necessary, because such exceptions usually just indicate script bugs.
But script and system functions should be able to throw virtual exceptions, and script could be able to handle them. It would make writing scripts much easier. For example, if exception "file access denied" caught, wait a while and retry.
Maybe it is not often useful in games, I don't know. But in many other contexts, most users are not programmers and they often will not check function's return value that may indicate a failure; and they will be angry if a function calls SetException() on a failure and they cannot add try/catch then.

Zero memory of primitives. Why humans should write =0 when computers can do it implicitly?

goto. If C# has it, it is probably not evil. Eg to jump from a nested loop or to jumt to another case in a switch block.

Option to use break-less switch/case. The reasons are obvious.
Also add case 1,2,3: (like in C#) that could be used instead of case 1:case 2:case 3:. And maybe case 1 to 3.

Class static methods and variables. See how elegant is C# when you can call Class.StaticFunc(...).
Or some other way to access class private members from a non-class function.

Thread variables. Like C# ThreadStatic attribute.

Shared (between modules) global variables. Now I read "Future versions may allow global variables to be shared too.".

A 'lock' keyword that would add a critical section or mutext.

Implicit conversion from integer types to bool. Eg allow if(intVar&intConst) and if(intFunc()) instead of requiring if((intVar&intConst)!=0) and if(0!=intFunc()).

Option to define custom operator precedence.
Or option 'no operator precedence', and instead require that multi-operator expressions contain parentheses. For example, error if a+b*c; must be a+(b*c) or (a+b)*c.
Benefits:
1. Don't need to learn and remember precedences (for example, I work with C++ >15 years but don't remember all its precedences).
2. When used for prototyping, will not be precedence differences in another language.

Option to define custom escape sequences (ES) in strings. I am aware about """strings""", but sometimes need ES anyway, and I don't like to write "line1\r\nline2 \"quote\"", it is so unreadable etc.
Possible implementation: let AS compiler call a callback that replaces escape sequences.

Interpolated strings, eg $"{variable1} text {variable2} text" like in PHP and C# 6. I use it VERY often. It is easier to write and read than ""+variable1+" text "+variable2+" text".

switch/case with strings. I use it often, and >50% of times with a 'wildcard match' option (case "*.cpp":...case "*.h").
From my experience, the speed is not important. I sometimes even use a 'regular expression' option.
The wildcard and regex options in AS could be implemented by an application's callback function, not by AS itself.

Functionless scripts. If a script begins with an executable code that is not in a function, automatically wrap the code (until the first explicitly defined function) into a 'void main(){}' function before compiling.

Define struct (POD value types) in script.

sizeof.

Nested classes within classes.

Allow classes and namespaces to span multiple files/modules. Like defining methods in multiple cpp files in C++, or partial classes in C#.

Allow scripts to easily extend system classes, such as string, either by defining inherited classes or by adding methods.

Option to omit semicolon after the last statement in a line.

Option to use namespace.Function (like in C#) instead of namespace::Function.

Option to not require 'return something;' in non-void functions. If it is omitted or just 'return;', let the function return 0 or false or null.

Variadic functions in script. Either with ... like in C or with a 'params' keyword like in C#.

INTERFACE

Option to call application's callback function when AS compiler encounters an unknown identifier. The function could register the identifier on demand, instead of registering everything before compiling.
I have a database of >250000 Windows API declarations. I want to be able to use them in script. But I don't want to register all them before compiling every script, for obvious reasons.
My callback just needs the identifier name. It could find it in the database. For example, if it is a dll function, it could load the dll/function, register it with RegisterGlobalFunction and return 0 (let AS compiler retry), or return -1 if not found.

SPEED

In my tests, AngelScript was surprisingly fast (when without a line callback) in loops and expressions, but too slow in calling functions, especially registered system functions.
With asCALL_GENERIC faster when there are no parameters. I did not test the JIT compiler.

