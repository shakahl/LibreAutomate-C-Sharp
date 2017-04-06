 /
function $qmlFile $xmlFile

 Converts .xml file created by QmlToXml back to .qml format.

 qmlFile - .qml file. The function creates or replaces it.
 xmlFile - .xml file. The function does not modify it.

 REMARKS
 QM must be in Unicode mode.


if(!_unicode) end "QM must be in Unicode mode."

str s
IXml x=CreateXml
x.FromFile(xmlFile)
lpstr sep="[][0][0]"

IXmlNode xr=x.RootElement
if(xr.AttributeValueInt("version")!1) end "different version"

IXmlNode xi=xr.FirstChild
if(StrCompare(xi.Name "header")) goto ge
s.decrypt(8 xi.Value)
s.geta(sep 0 4)

rep
	xi=xi.Next; if(!xi) break
	if(StrCompare(xi.Name "item")) goto ge
	lpstr text=xi.Value; if(text[0]='.') text+1; else goto ge ;;prevent removing empty lines
	s.fromn(s s.len xi.AttributeValue("name") -1 "" 1 xi.AttributeValue("triggerEtc") -1 "" 1 xi.AttributeValue("flagsEtc") -1 "" 1 xi.AttributeValue("folder") -1 text -1 "[0]" 2)

 outb s s.len 1
s.setfile(qmlFile)

err+ end _error
ret
 ge
end "bad file format"
