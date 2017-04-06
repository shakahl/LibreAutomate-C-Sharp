 QM
Function x y
Function(x y) ;;or this
Function(x, y); ;;or this

 C++, C#
Function(x, y);
________________________________

 QM
 comments
code ;;comments

 C++, C#
//comments
code //comments
/*block of
comments*/
________________________________

 QM
int variable
byte variable
word variable
long variable
double variable
lpstr variable
str variable
BSTR variable
ARRAY(int) variable
int a b(5) c=6
int handle hwnd
int functionAddress=&Function

 C++
int variable; //or long variable; //also there are unsigned types - UINT (unsigned int) and DWORD (unsigned long). QM does not have unsigned int. Also can be LPARAM, LRESULT, HRESULT etc etc.
BYTE variable; //or unsigned char variable; //also there is signed type char.
WORD variable; //or unsigned short variable; //also there is signed type short.
__int64 variable;
double variable
LPSTR variable; //or char* variable
CString variable; //MFC, ATL
CComBSTR variable; //ATL
CArray <int> variable; //MFC
CSimpleArray <int> variable; //MFC
 more string and array classes are available, from various libraries
int a, b=5, c=6;
HANDLE handle; HWND hwnd; //there are many handle types, whereas in QM all are int
void Function(void); //used to specify that the function does not return a value or sometimes when it does not have parameters. There is no such thing in QM.
int (__stdcall* FunctionType)(int x, int y); FunctionType functionAddress=Function; //callback function type and address

 C#
int variable; //uint for unsigned
byte variable; //sbyte for signed
ushort variable; //short for signed, char for Unicode UTF-16 character
long variable; //ulong for unsigned
double variable;
String variable; //Unicode UTF-16 string
int[] variable; //array
int a, b=5, c=6;
void Function(); //does not return a value
delegate int DelegateType(int x, int y); static DelegateType functionAddress; //callback function type and address
________________________________

 QM
int+ global_variable
int- thread_variable
int local_variable
int* pointer1 pointer2
int& reference_variable=variable
int'member_variable int*member_variable2 //in class

 C++
int global_variable; //not in function or class
static int global_variable; //anywhere
__declspec(thread) int thread_variable; //not in function
void Function()
{
int local_variable; //in a function
int* pointer1; int* pointer2; //or int *pointer1, *pointer2;
int& reference_variable=variable;
}
int member_variable; int* member_variable2; //in class

 C#
static int global_variable; //in class
[ThreadStatic] static int thread_variable; //in class
void Function()
{
int local_variable; //in function
}
int member_variable; //in class
________________________________

 QM
def CO_NSTANT 5
def CO_NSTANT2 5+6

 C++
#define CO_NSTANT 5
#define CO_NSTANT2 (5+6)
const int C1=5;
enum CONSTANTS { C1=5, C2=5+6 };

 C#
const int C1=5; //in class or function
enum CONSTANTS { C1=5, C2=5+6 }; //globally or in class
________________________________

 QM
out variable

 C++
printf("%i", variable); //ANSI project
printf(L"%i", variable); //Unicode project

 C#
Console.WriteLine(variable);
________________________________

 QM
if condition
	code
else
	code2

 C++, C#
if(condition)
{
	code
}
else
{
	code2
}
________________________________

 QM
for i 0 5
	code

 C++, C#
for(i=0; i<5; i++)
{
	code
}
________________________________

 QM
rep 5
	code

 C++, C#
for(int i=0; i<5; i++)
{
	code
}
________________________________

 QM
rep
	if(condition=0) break
	...

 C++, C#
while(condition)
{
	...
}
________________________________

 QM
rep
	...
	if(condition=0) break

 C++, C#
do
{
	...
}
while(condition);
________________________________

 QM
sel i
	case 1
	code
	case 2
	code
	case else
	code

 C++, C#
switch(i)
{
case 1:
	code
	break;
case 2:
	code
	break;
default:
	code
}
________________________________

 QM
goto label
...
 label

 C++, C#
goto label;
...
label:
________________________________

 QM
code
err code2

 C++
try
{
code
}
catch(...) { code2 }

 C#
try
{
code
}
catch { code2 }
________________________________

 QM
i+1 ;;or can be: i+=1
a = b or c ;;or can be: a = b || c
if(a=b) code ;;or can be: if(a==b) code
x = iif(condition A B)

 C++, C#
i+=1; //or can be: i++;
a = b || c;
if(a==b) code ;;if(a=b) would assign b to a
x = condition ? A : B;

 Most operators are the same in QM, C++ and C#.
________________________________

 QM
function'int x [y] ;;function name is not here
code
ret 1

 C++
int FunctionName(int x, int y=0)
{
code
return 1;
}

 C#
int FunctionName(int x, int y) //C# does not have optional arguments; use overloads or params
{
code
return 1;
}
________________________________

 QM
dll "file.dll" int'Function int'x lpstr's
 don't need for most API, because it is in WINAPI reference file etc, or if it is in an explicitly declared reference file or type library

 C++
#pragma comment(lib, "file.lib") //or add in project Options dialog. Don't need if it is one of default libraries, such as kernel32 or user32.
int Function(int x, LPSTR s);
 don't need if headers included, like #include <system_header_file.h> or #include "user_header_file.h"

 C#
using System.Runtime.InteropServices; ... [DllImport("file.dll")] public static extern IntPtr Function(int x, [MarshalAs(UnmanagedType.LPStr)] String s); //in class
________________________________

 QM
type Type x y double'd

 C++
struct Type { int x, y; double d; };

 C#
struct Type { public int x, y; public double d; };
________________________________

 QM
class Class --m_private m_public ;;here declared just member variables, not functions

 C++
class Class
{
int m_private;
public:
int m_public;
Class { code } //constructor
~Class { code } //destructor
void MemberFunction(int x, int y); //here is just declaration; definition (code) is somewhere
};
//here is definition (code) of function that is declared somewhere
void Class::MemberFunction(int x, int y)
{
code
}

 C#
class Class
{
int m_private;
public int m_public;
public Class { code } //constructor
~Class { code } //destructor
public void MemberFunction(int x, int y)
{
code
}
};
________________________________

 QM
category X : functions ;;QM does not have namespaces. category is something similar, but very different.

 C++
namespace X
{
classes, functions, variables, etc
}

 C#
namespace X
{
classes
}
