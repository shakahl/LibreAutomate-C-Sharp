 /
function $xmlFile $regKey [hive]

 Exports registry key to an XML file.
 Error if failed.
 Error if QM is running not in Unicode mode.

 xmlFile - file. Example: "$desktop$\my program settings.xml". If exists, the function replaces it.
 regKey - registry key. Example: "Software\My Company\My Program".
 hive - registry hive constant. Default or 0: HKEY_CURRENT_USER.

 Why don't use RegSaveKey? 1. Does not work on Vista User account. 2. It is better if the file is not binary.
 Why don't use regedit.exe? On some computers regedit is disabled.


if(!_unicode) end "Error: QM must be running in Unicode mode."

IXml xml=CreateXml
IXmlNode x=xml.Add("k")
x.SetAttributeInt("hive" hive)

if(!REX_Key(hive regKey x)) end ES_FAILED

xml.ToFile(xmlFile)
err+ end _error
