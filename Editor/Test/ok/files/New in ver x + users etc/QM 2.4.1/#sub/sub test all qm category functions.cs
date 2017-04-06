 all tested

 qm

sub.Test

mac "sub.Thread"
 1; EndThread "<00004>Thread"
 WaitForThreads 0 "<00004>Thread"
 0.1; ARRAY(QMTHREAD) a.create(10); int i n=EnumQmThreads(&a[0] 10 0 "<00004>Thread"); for(i 0 n) out a[i].qmitemid
 0.1; out IsThreadRunning("<00004>Thread")

#sub Test

 act "ddddddddd"
 err ErrMsg

#sub Thread
mes __FUNCTION__
