function $csv [$rName] [$rKey] [flags] ;;flags: 1 call FromReg now.

 Initializes this variable.

 csv - list of names, labels (grid control text in the first column) and default values in CSV format: "name,label,value[]name,label,value[]...". Can be 1 or more columns of values.
 rName, rKey - registry value name and key that will be used by FromReg, ToReg and OptionsDialog. Same as with <help>rget</help>.

 EXAMPLE
 __Settings x.Init("name,Name,me[]speed,Speed,100" "settings" "\MyCompany\MyMacro" 1)


m_c._create
m_c.FromString(csv)

 map names to CSV row indices for faster/easier searching
m_m=CreateStringMap
for(_i 0 m_c.RowCount) m_m.IntAdd(m_c.Cell(_i 0) _i)

m_rName=rName
m_rKey=rKey
if(flags&1) FromReg()

err+ end ERR_BADARG
