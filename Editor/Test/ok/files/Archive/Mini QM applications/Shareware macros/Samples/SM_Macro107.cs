 Insert the following line in one or more macros and functions to prevent using them after the
 evaluation period. Of course you must change SM_CanRun to the name of your ..._CanRun function,
 and 1522539484 to the value of sm_ret that you have defined in your ..._CanRun.
if(SM_CanRun!=1522539484) end

 Your macro here.
mes "SM_Macro107"
SM_Function55 1 2
