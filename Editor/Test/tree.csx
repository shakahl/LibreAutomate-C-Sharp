#define THREAD

Print(1);

#if THREAD
{
#endif
	Print(2);
#if THREAD
}
#endif
