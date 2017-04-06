out

ARRAY(int) a.create(22)
a[0]=EXCEPTION_ACCESS_VIOLATION
a[1]=EXCEPTION_ARRAY_BOUNDS_EXCEEDED
a[2]=EXCEPTION_BREAKPOINT
a[3]=EXCEPTION_DATATYPE_MISALIGNMENT
a[4]=EXCEPTION_FLT_DENORMAL_OPERAND
a[5]=EXCEPTION_FLT_DIVIDE_BY_ZERO
a[6]=EXCEPTION_FLT_INEXACT_RESULT
a[7]=EXCEPTION_FLT_INVALID_OPERATION
a[8]=EXCEPTION_FLT_OVERFLOW
a[9]=EXCEPTION_FLT_STACK_CHECK
a[10]=EXCEPTION_FLT_UNDERFLOW
a[11]=EXCEPTION_GUARD_PAGE
a[12]=EXCEPTION_ILLEGAL_INSTRUCTION
a[13]=EXCEPTION_IN_PAGE_ERROR
a[14]=EXCEPTION_INT_DIVIDE_BY_ZERO
a[15]=EXCEPTION_INT_OVERFLOW
a[16]=EXCEPTION_INVALID_DISPOSITION
a[17]=EXCEPTION_INVALID_HANDLE
a[18]=EXCEPTION_NONCONTINUABLE_EXCEPTION
a[19]=EXCEPTION_PRIV_INSTRUCTION
a[20]=EXCEPTION_SINGLE_STEP
a[21]=EXCEPTION_STACK_OVERFLOW
 a[22]=STATUS_UNWIND_CONSOLIDATE

int i
for i 0 a.len
	out _s.dllerror("" "ntdll.dll" a[i])
	 out "---- 0x%X 0x%X" a[i] RtlNtStatusToDosError(a[i])
	 out _s.dllerror("" "" RtlNtStatusToDosError(a[i]))
	out "--------------------------------------------------"

 The instruction at 0x
 --------------------------------------------------
 {EXCEPTION}
 Array bounds exceeded.
 --------------------------------------------------
 {EXCEPTION}
 Breakpoint
 A breakpoint has been reached.
 --------------------------------------------------
 {EXCEPTION}
 Alignment Fault
 A datatype misalignment was detected in a load or store instruction.
 --------------------------------------------------
 {EXCEPTION}
 Floating-point denormal operand.
 --------------------------------------------------
 {EXCEPTION}
 Floating-point division by zero.
 --------------------------------------------------
 {EXCEPTION}
 Floating-point inexact result.
 --------------------------------------------------
 {EXCEPTION}
 Floating-point invalid operation.
 --------------------------------------------------
 {EXCEPTION}
 Floating-point overflow.
 --------------------------------------------------
 {EXCEPTION}
 Floating-point stack check.
 --------------------------------------------------
 {EXCEPTION}
 Floating-point underflow.
 --------------------------------------------------
 {EXCEPTION}
 Guard Page Exception
 A page of memory that marks the end of a data structure, such as a stack or an array, has been accessed.
 --------------------------------------------------
 {EXCEPTION}
 Illegal Instruction
 An attempt was made to execute an illegal instruction.
 --------------------------------------------------
 The instruction at 0x
 --------------------------------------------------
 {EXCEPTION}
 Integer division by zero.
 --------------------------------------------------
 {EXCEPTION}
 Integer overflow.
 --------------------------------------------------
 An invalid exception disposition was returned by an exception handler.
 --------------------------------------------------
 An invalid HANDLE was specified.
 --------------------------------------------------
 {EXCEPTION}
 Cannot Continue
 Windows cannot continue from this exception.
 --------------------------------------------------
 {EXCEPTION}
 Privileged instruction.
 --------------------------------------------------
 {EXCEPTION}
 Single Step
 A single step or trace operation has just been completed.
 --------------------------------------------------
 A new guard page for the stack cannot be created.
 --------------------------------------------------
