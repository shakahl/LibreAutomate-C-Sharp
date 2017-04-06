str tempDir=F"$temp qm$\ver 0x{QMVER}"
str tempFile.from(tempDir "\qmzip.dll")
mkdir tempDir

tempFile.expandpath
out tempFile
out FileExists(tempFile)
