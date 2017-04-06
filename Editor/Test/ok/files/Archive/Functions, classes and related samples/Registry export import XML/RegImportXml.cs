 /
function $xmlFile [$regKey] [hive]

 Imports registry key from an XML file created by RegExportKeyXml.
 Error if failed.
 Error if QM is running not in Unicode mode.

 regKey and/or hive can be used to import to a different place than exported.


if(!_unicode) end "Error: QM must be running in Unicode mode."

IXml xml=CreateXml
xml.FromFile(xmlFile)
IXmlNode x=xml.RootElement

if(empty(regKey)) regKey=x.AttributeValue("n")
if(!hive) hive=x.AttributeValueInt("hive"); if(!hive) hive=HKEY_CURRENT_USER

if(!RIX_Key(hive regKey x)) end ES_FAILED

err+ end _error
