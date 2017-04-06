str folder="\System"

QMITEM q
if(!qmitem(folder 0 q)) end "not found"
SendMessage id(2202 _hwndqm) TVM_SELECTITEM TVGN_CARET q.htvi
