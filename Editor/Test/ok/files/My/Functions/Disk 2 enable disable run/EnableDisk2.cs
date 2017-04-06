 /
function [disable]

DiskOfflineOnline !GetDiskNumberFromDriveLetter("C") disable "E:"

 Note: disk number sometimes is 1, sometimes 0. Therefore we get C number (because it always enabled) and use other.
