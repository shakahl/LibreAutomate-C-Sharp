#define __SPECSTRINGS_STRICT_LEVEL 0 //prevent redefining _Outptr_ etc to something useless
#undef _SA_annotes3
#define _SA_annotes3(n,pp1,pp2,pp3) ^pp1
//only _SA_annotes3 is used
//#undef _SA_annotes1
//#define _SA_annotes1(n,pp1) ^pp1
//#undef _SA_annotes2
//#define _SA_annotes2(n,pp1,pp2) ^pp1
