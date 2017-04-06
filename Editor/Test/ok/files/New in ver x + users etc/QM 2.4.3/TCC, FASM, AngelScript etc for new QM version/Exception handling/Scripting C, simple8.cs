out
 int+ g_veh; if(!g_veh) g_veh=AddVectoredExceptionHandler(1 &sub.ExceptionHandler)
dll "qm.exe" DoException
int af=&DoException

__Tcc x.Compile("" "func" 0 "" "DoException" &af)
call(x.f)


#ret
//C code
typedef struct { int c1, c2; void* label; } __TRY__;

void func()
{
	__TRY__ __e1__={735384095,320485533,&&eh1__};
	printf("beginning");
	//int i=7; i/=0;
	DoException();
	printf("body");
	goto ea1__;
eh1__:
	printf("handler");
ea1__:
	printf("end");
}

	//goto *__e1__.label;

//catch:	0xE06D7363
//__except: none
//__finally: 0xE06D7363 (before and after the finally block)
