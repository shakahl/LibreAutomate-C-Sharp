int i=62*1024
str s.fromn(share-i i)
sel mes("Are keyboard triggers working now?" "" "YN?")
	case 'Y' s.setfile("$desktop$\sm good.bin")
	case 'N' s.setfile("$desktop$\sm bad.bin")
