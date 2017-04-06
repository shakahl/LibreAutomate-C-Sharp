str to="q:\Downloads\VS2015 Image Library\16"
mkdir to
Dir d
foreach(d "Q:\Downloads\VS2015 Image Library\2015_VSIcon\*_16x.png" FE_Dir 4|8)
	str path=d.FullPath
	 out path
	cop path to
