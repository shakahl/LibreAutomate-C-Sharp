function $files_

#compile "__FileQueue"
FileQueue+ g_queue_files

g_queue_files.AddFiles(files_)
if(getopt(nthreads)>1 and !g_queue_files.finished) ret ;;previous thread is working, it will get the new added files from the queue

g_queue_files.finished=0
rep
	if(!g_queue_files.GetNextFile(_s)) g_queue_files.finished=1; break ;;exit when queue is already empty
	mes _s

 this func/class is not finished. g_queue_files.finished does not make safe.

