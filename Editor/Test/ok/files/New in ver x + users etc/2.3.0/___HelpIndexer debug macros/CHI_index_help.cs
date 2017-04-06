 This macro is only for debugging.
 __CHelpIndexer.CreateMenu creates index automatically. 

out
#compile _____HelpIndexer

___HelpIndexer ind.Init
ind.IndexHelp

ret
IStringMap& m=ind.m_mw_help
str sk sv
m.EnumBegin
rep
	if(!m.EnumNext(sk sv)) break
	out "%s %s" sk sv
	 if(sv.len>300) out "%s %s" sk sv
	 if(sv.len>150 and sv.len<=200) out "%s %s" sk sv
	 if(sk.len<=3) out "%s %s" sk sv
out "-- count = %i --" m.Count
 4006
