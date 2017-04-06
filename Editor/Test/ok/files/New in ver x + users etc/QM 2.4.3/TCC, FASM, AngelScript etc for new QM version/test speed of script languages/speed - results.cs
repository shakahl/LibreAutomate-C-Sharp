        loop if/expr string array userfunc sysfunc

 C++ MSVC
 speed:   3    5       3      4     64    -   //optimized for speed
 speed:   3    5       4      5     64    -   //optimized for size
 speed:   22   22      23     23    151   -   //not optimized

 C#
 speed:   2    2       3      2     90    -   //Release config
 speed:   21   28      40     24    221   -   //Debug config (no optimizations)

 Boo
 speed:   5    3       8      38    107   -   //while
 speed:   7    8       8      10    17    -   //for (something wrong, maybe ignores [MethodImpl(MethodImplOptions.NoInlining)])

 TCC
 speed:   22   22      23     25    115   93

 AngelScript (C++ code optimized for speed; without line callback; non-generic call)
 speed:   142  151     267    256   2220  3300
 speed:   26   32      211    209   1700  1610   //JIT, default options
 speed:   25   22      201    203   1675  168    //JIT, all 'faster' options (JIT_NO_SUSPEND makes sysfunc so fast)

 QM
 speed:   248  704     398    473   3964  1600   //optimized for speed
 speed:   237  696     397    473   4076  1650   //optimized for size (but some parts have #pragma optimize("t", on))
