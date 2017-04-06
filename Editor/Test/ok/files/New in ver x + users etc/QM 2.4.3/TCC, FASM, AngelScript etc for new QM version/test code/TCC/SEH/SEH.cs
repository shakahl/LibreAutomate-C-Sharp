int k;

#{
asm __volatile__("
    pusha;
    pushl %1;
    pushl %0;
    pushl %%fs:0;
    movl %%esp, %%fs:0;
"
:
:"r"(UdfOnException),"r"(&&__e1__)
);

//int _esp, _ebp;
//printf("SEH: __f0__=0x%X __e1__=0x%X", &__c1__, &&__e1__);
//printf("1");
//k/=k;
//UdfGenerateException();
//printf("2");
__e1__:
printf("3");
//return;

asm __volatile__("
    movl 0(%esp), %eax
    movl %eax, %fs:0;
    addl $12, %esp;
    popa;
");
}















/*
//int UdfOnException(void* a, void* b, void* c, void* d);

int eh(void* a, void* b, void* c, void* d)
{
//crashes if we give a function address in heap, even if it is a jmp to a function in exe.
printf("eh");
return 1;
}


 :"a"(_get_UdfOnException())

    pushl $UdfOnException;
    addl $8, %esp;


*/
