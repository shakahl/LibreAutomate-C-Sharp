function [flags] ;;flags: 1 don't clear

 Creates index of words/topics for the menu. Saves in several files in $appdata$.
 This function is called automatically when need.


#compile _____HelpIndexer

___HelpIndexer ind.Init
ind.IndexHelp
ind.IndexTools
ind.IndexFunc
ind.IndexTips

if flags&1=0
	___HelpIndexer+ ___chi_indexer.Clear
