//QM code
CsScript x.AddCode("")

//let C# call QM callback function
out x.Call("CallQmCallback" &sub.Callback 100)

//call C# callback function
int CsCallback=x.Call("GetCallback")
out call(CsCallback 300)

#sub Callback
function# param
out param
ret param

#ret
//C# code
using System;
using System.Runtime.InteropServices;

public delegate int CbType(int param);

public class Test
{

//calls QM callback function
public static int CallQmCallback(int cbFunc, int cbParam)
{
CbType CsScriptExample_Callback=(CbType)Marshal.GetDelegateForFunctionPointer((IntPtr)cbFunc, typeof(CbType));
return CsScriptExample_Callback(cbParam);
}

//C# callback function that QM will call
public static int CsCallback(int cbParam)
{
Console.Write(cbParam);
return cbParam;
}

public static CbType m_cb=CsCallback;

//returns address of C# callback function
public static int GetCallback()
{
return (int)Marshal.GetFunctionPointerForDelegate(m_cb);
}

}
