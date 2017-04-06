 this macro shows how to write XML file that looks like

 <payments>
 <i>
 	<date>7/28/2008 12:49:31 PM</date>
 	<x>Data1|Data2|Data3|Data4|Data5|</x>
 	<!-- and here you can add more tags, for example use different tags for Data1, Data2, etc -->
 	<!-- unlike in HTML, tag names are not predefined. You define them. But use only alphanumeric characters, _ and -. -->
 </i>
 <i>
 	<date>7/28/2008 12:50:31 PM</date>
 	<x>Data1|Data2|Data3|Data4|Data5|</x>
 </i>
 <i>
 	<date>7/28/2008 12:51:31 PM</date>
 	<x>Data1|Data2|Data3|Data4|Data5|</x>
 </i>
 </payments>


str xmlfile="$desktop$\pa.xml"
if(!dir(xmlfile)) _s="<payments />"; _s.setfile(xmlfile)

IXml xml=CreateXml
xml.FromFile(xmlfile) ;;load existing file, because this macro is going to append, not replace
IXmlNode re=xml.RootElement ;;payments
IXmlNode ni

ni=re.Add("i")
ni.Add("date" "7/28/2008 12:49:31 PM")
ni.Add("x" "Data1|Data2|Data3|Data4|Data5|")

ni=re.Add("i")
ni.Add("date" "7/28/2008 12:50:31 PM")
ni.Add("x" "Data1|Data2|Data3|Data4|Data5|")

ni=re.Add("i")
ni.Add("date" "7/28/2008 12:51:31 PM")
ni.Add("x" "Data1|Data2|Data3|Data4|Data5|")

xml.ToFile(xmlfile)
 note: you can save (call ToFile) multiple times, not necessary only at the end
