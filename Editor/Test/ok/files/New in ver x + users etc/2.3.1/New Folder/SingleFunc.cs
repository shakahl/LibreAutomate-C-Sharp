__Handle+ __mutex_singlefunction ;;if __Handle is unknown identifier, replace to int
if(!__mutex_singlefunction) __mutex_singlefunction=CreateMutex(0 0 0)
if(WaitForSingleObject(__mutex_singlefunction 0)=WAIT_TIMEOUT) end
