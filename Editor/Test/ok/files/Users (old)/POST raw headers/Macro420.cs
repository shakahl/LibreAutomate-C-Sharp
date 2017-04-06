str response
str headers=""
 Http h.Connect("www.quickmacros.com"); err end _error
Http h.Connect("www.google.com"); err end _error
if(!h.Post2("form2.php" "name=xxx&email=yyy" response headers)) end ES_FAILED
