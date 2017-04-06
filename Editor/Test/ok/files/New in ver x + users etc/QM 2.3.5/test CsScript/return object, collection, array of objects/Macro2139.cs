out

CsScript x.AddCode("")
IDispatch t=x.CreateObject("Test")

 VARIANT v=t.testStrings; out "0x%X" v.vt ;;correct, 0x2009
ARRAY(IDispatch) testStrings = t.testStrings

out F"Length of testStrings array {testStrings.len}"
for _i 0 testStrings.len
	out testStrings[_i].aString
	out testStrings[_i].bString

#ret
using System;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;

public class Test2Strings{
	public string aString {get;set;}
	public string bString;
}

public class Test
{
public Test2Strings[] testStrings { get; set;}
//public Test2Strings[] testStrings;

public Test()
{
testStrings = new Test2Strings[3];
for (int i=0;i<testStrings.Length; i++){
	testStrings[i] = new Test2Strings();
	testStrings[i].aString = "First String " + i;
	testStrings[i].bString = "Second String " + i;
	}
}
}
