typelib Eureka {02A0C1FC-A256-4DAB-A117-89795A0EEAF3} 4.0
 #opt dispatch 1
Eureka.Encryption c._create
BSTR s("texttext") sk("65572345518") se sd
se=c.Encrypt(&sk &s)
out se
sd=c.Decrypt(&sk &se)
out sd
