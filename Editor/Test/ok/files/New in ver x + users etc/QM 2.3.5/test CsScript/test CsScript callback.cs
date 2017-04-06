/exe
out

CsScript x.AddCode("")

 out &CsScriptCallback
out x.Call("Add" &CsScriptCallback 100)

IDispatch d=x.CreateObject("Test")
out d.Add2(&CsScriptCallback 200)

int cscb=x.Call("GetCallback")
out call(cscb 300)
PF
rep(7) call(cscb 300); PN
PO

#ret
using System;
using System.Runtime.InteropServices;

public delegate int CbType(int param);
public class Test
{

//public static int Add(IntPtr cbFunc, int cbParam)
public static int Add(int cbFunc, int cbParam)
{
//Console.Write(cbFunc); return 0;
CbType cb=(CbType)Marshal.GetDelegateForFunctionPointer((IntPtr)cbFunc, typeof(CbType));
return cb(cbParam);
}

//public int Add2(IntPtr cbFunc, int cbParam)
public int Add2(int cbFunc, int cbParam)
{
//Console.Write(cbFunc); return 0;
CbType cb=(CbType)Marshal.GetDelegateForFunctionPointer((IntPtr)cbFunc, typeof(CbType));
return cb(cbParam);
}

public static int Callback(int cbParam)
{
Console.Write(cbParam);
return cbParam;
}

public static CbType m_cb=Callback;
public static int GetCallback()
{
return (int)Marshal.GetFunctionPointerForDelegate(m_cb);
}

}
