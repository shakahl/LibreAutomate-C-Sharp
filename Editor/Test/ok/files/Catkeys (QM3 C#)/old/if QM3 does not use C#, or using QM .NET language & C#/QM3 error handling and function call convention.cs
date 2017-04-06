 By default, any UDF and intrinsic function returns HRESULT, and returns the actual value through a hidden reference parameter.
 The we'll use that HRESULT for error handling.

 For example, how a class method is translated from QM to C:
 QM
int Add(int a, int b)
{
}
 C
HRESULT Add(Class* this, int a, int b, int* _ret_)
{
}

 Allow standard return too, which can be used for API callback.
 Then can return only an integer or pointer or double/float.
 Then the function cannot throw. Its exceptions are handled.
[stdcall|cdecl]
int CallbaclProc(int a, int b)
{
...
}

