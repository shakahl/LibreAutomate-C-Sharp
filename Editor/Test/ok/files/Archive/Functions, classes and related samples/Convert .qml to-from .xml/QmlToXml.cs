 /
function $qmlFile $xmlFile

 Converts .qml (QM file) to .xml.

 qmlFile - .qml file. The function does not modify it.
 xmlFile - .xml file. The function creates or replaces it.

 REMARKS
 QM files are binary. Use this function if want to convert to text format.
 QM must be in Unicode mode.


if(!_unicode) end "QM must be in Unicode mode."

str s.getfile(qmlFile)
IXml x=CreateXml
lpstr sep="[][0][0]"

 header
int j=findb(s sep 4)
if(!s.beg("//QM v2.") or j<0) goto ge
IXmlNode xr=x.Add("qml")
_s.left(s j); _s.encrypt(8)
xr.Add("header" _s)
xr.SetAttributeInt("version" 1)

 items
lpstr name triggerEtc flagsEtc folder text
rep
	j+4; if(j=s.len) break
	name=s+j
	triggerEtc=name+len(name)+1 ;;can contain not only trigger
	flagsEtc=triggerEtc+len(triggerEtc)+1 ;;flags[ date[ image]]
	folder=flagsEtc+len(flagsEtc)+1
	text=strstr(folder "[]"); if(!text) goto ge
	j=findb(s sep 4 text-s+2); if(j<0) goto ge
	
	IXmlNode xi=xr.Add("item")
	xi.SetAttribute("name" name)
	if(triggerEtc[0]) xi.SetAttribute("triggerEtc" triggerEtc)
	xi.SetAttribute("flagsEtc" flagsEtc)
	text[0]=0; xi.SetAttribute("folder" folder)
	text[0]=13; text[-1]='.'; xi.Value=text-1 ;;.[]text[] ;;. prevents removing empty lines

 x.ToString(_s); out _s
x.ToFile(xmlFile)

err+ end _error
ret
 ge
end "bad file format"
