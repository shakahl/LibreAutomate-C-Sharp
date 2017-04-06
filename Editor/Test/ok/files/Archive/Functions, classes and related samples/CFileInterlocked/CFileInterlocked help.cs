 Use this class to read/write files that can be simultaneously accessed by multiple macros running in separate threads, processes or computers.
 Synchronizes access to the file to avoid errors.
 You probably don't need this if you only want to read the file. Then instead use str.getfile.

 Run this code to declare the class.
#compile __CFileInterlocked

 EXAMPLES
out
CFileInterlocked f
str s

 text file
f.Open("$desktop$\test CFileInterlocked.txt")
f.ToStr(s)
out s
s.addline("test")
f.FromStr(s)
 s="test append[]"; f.FromStr(s 1)

 xml file
IXml xml=CreateXml
f.Open("$desktop$\test CFileInterlocked.xml")
f.ToIXml(xml "<x/>")
xml.ToString(s)
out s
xml.RootElement.Add("i" "xxxxx")
f.FromIXml(xml)
