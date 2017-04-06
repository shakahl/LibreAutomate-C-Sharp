 /
function# $httpfile str&data [usecache] [keepconnection] ;;usecache: 0 download always, 1 download if modified, 2 always use cache.

 Obsolete.

Http- _http
ret _http.FileGet(httpfile data usecache keepconnection)
