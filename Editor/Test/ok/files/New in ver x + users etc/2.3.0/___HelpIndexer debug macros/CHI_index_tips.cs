 This macro is only for debugging.
 __CHelpIndexer.CreateMenu creates index automatically. 

out
#compile _____HelpIndexer

 int t1=perf
___HelpIndexer ind.Init
ind.IndexTips
 int t2=perf
 out t2-t1

 ret
IStringMap& m=ind.m_mw_tips
str sk sv
m.EnumBegin
rep
	if(!m.EnumNext(sk sv)) break
	out "%s %s" sk sv
out "-- count = %i --" m.Count
 482
