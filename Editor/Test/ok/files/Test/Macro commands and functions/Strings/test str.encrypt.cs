ClearOutput
str s="abcdefghij"
str ss="abc"
str sss="abcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefgh"
str sd
    QM
 s.encrypt()
 s.encrypt("abcdefgh")
 s.encrypt(0)
 s.encrypt(0 s "key" 1)
 s.encrypt(0 ss)
 out s
 out s.decrypt
 out sd.decrypt(s)
 out sd.decrypt(0 s)

    blowfish
 s.encrypt(1 s "key")
 s.encrypt(1 ss "key")
 out s
 out s.decrypt(1 s "key")
 out sd.decrypt(1 s "key")

 out s.encrypt(1|4 s "key" 1)
 out s.decrypt(1|4 s "key")
 out s.encrypt(1|8 s "key")
 out s.decrypt(1|8 s "key")

    MD5
 out s.encrypt(2)
 out s.encrypt(2 ss)
 out s.encrypt(2 ss "key")
 out s.encrypt(6 ss "" 3)
 s.encrypt(2 ss); out s.encrypt(4)
 out s.encrypt(10 ss)
 s.encrypt(2 ss); out s.encrypt(8)
 s=ss; s.encrypt(2); out s.encrypt(8)

    base64
 out s.encrypt(4)
 out sd.decrypt(4 s)
 out s.encrypt(4 sss "" 0)
 out s.encrypt(4 sss "" 3)
 out s.decrypt(4)

    hex
 s="ABC"; s[1]=0
 out sd.encrypt(8 s)
 out s.encrypt(8)
 out s.decrypt(8)

 s="ABCDEFGHIJKLMNOPQRSTUVWXYZ"
 out s.encrypt(8 s "" 1|8)
 out s.decrypt(8)
