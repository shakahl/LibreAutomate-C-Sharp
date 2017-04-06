 \
function $query

 The main function.
 Finds matching help topics, tool dialogs, etc, and creates/shows the menu.
 QM launches CHI_Menu on Enter in the help search edit control.


#compile _____HelpIndexer

0.01

___HelpIndexer+ ___chi_indexer.CreateMenu(query) ;;2.5 ms
 ___HelpIndexer ind.CreateMenu(query) ;;6.6 ms
