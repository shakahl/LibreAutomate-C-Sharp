 int w=win("TOOLBAR71" "QM_toolbar")
 SendMessage w 666 0 0

str s.expandpath("$qm$\winapi.txt")
SHAddToRecentDocs(SHARD_PATHW +@s)
