
 Checks for new QM version.
 If there is new version, displays version info in QM output and adds tray icon (i).
 To run this at startup, assign trigger: QM events -> QM started.

 you can change these values
int firstCheckAfterSeconds=30
int periodHours=24
int flags=0 ;;flag 1 - debug (always behave like a new version is available; ignore firstCheckAfterSeconds; allow multiple threads)

 ____________________________

if flags&1=0
	if(getopt(nthreads)>1) ret
	wait firstCheckAfterSeconds

rep
	str s
	IntGetFile _s.format("http://www.quickmacros.com/version.php?v=0x%X" iif(flags&1 0 QMVER)) s iif(flags&1 0 2)
	err wait 60*60; continue
	if(s.len) break
	wait periodHours*60*60

IXml xml=CreateXml
xml.FromString(s)
IXmlNode r=xml.RootElement

str s1 s2 s3; int nv=r.AttributeValueInt("v")
s1.format("%i.%i.%i.%i" QMVER>>24 QMVER>>16&255 QMVER>>8&255 QMVER&255)
s2.format("%i.%i.%i.%i" nv>>24 nv>>16&255 nv>>8&255 nv&255)

s3.format("<><link ''http://www.quickmacros.com/download.html''>New Quick Macros version available</link>. Your version: %s. New version: %s. Beta: %s." s1 s2 r.AttributeValue("b"))
s1=r.Value; s1.trim; if(s1.len) s3.addline(s1 1)
out s3

Tray t.AddIcon("$qm$\info.ico" "New Quick Macros version available" 0 &CFNQV_Callback)
wait -1
