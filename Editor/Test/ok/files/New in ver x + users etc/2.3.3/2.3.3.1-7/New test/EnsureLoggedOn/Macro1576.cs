out
 out IsLoggedOn

 shutdown 6; 5
20
out EnsureLoggedOn(1)
1
clo "qmtul.log"; err
run "qmtul.log"

MES m.timeout=5; mes "" "" m
