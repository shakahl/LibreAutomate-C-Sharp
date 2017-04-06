 int lpSectorsPerCluster lpBytesPerSector lpNumberOfFreeClusters lpTotalNumberOfClusters
 out GetDiskFreeSpace("Q:" &lpSectorsPerCluster &lpBytesPerSector &lpNumberOfFreeClusters &lpTotalNumberOfClusters)
 out "%i %i %i %i" lpSectorsPerCluster lpBytesPerSector lpNumberOfFreeClusters lpTotalNumberOfClusters

int i; str s=" :"
for i 0 16
	s[0]='A'+i
	out F"{s} {GetDriveType(s)}"

