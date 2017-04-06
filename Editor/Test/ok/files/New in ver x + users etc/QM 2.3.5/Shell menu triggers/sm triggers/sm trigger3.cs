function $files
lock ;;other thread will wait here until previous thread ends. It is a simple way to make threads run in a queue, not simultaneously.
foreach f files
	FCleanthefile(f)
