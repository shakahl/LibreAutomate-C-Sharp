out
 #compile "____Tcc2" ;;in init2
__Tcc2 x
x.Compile("" "TestK")
out call(x.f)


#ret
__declspec(dllexport)
int TestK()
{
	//return strlen("test");
	//return strlen("test")+GetCurrentProcessId();
	return GetCurrentProcessId()+1;
	//return 7;//GetTickCount()+strlen("strii")+PrivFunc(2, 3);
}
