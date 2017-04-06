

############ Begin - SmartOptimizer
#
<IfModule mod_expires.c>
	<FilesMatch "\.(gif|jpg|jpeg|png|swf|css|js|html?|xml|txt)$">
		ExpiresActive On
		ExpiresDefault "access plus 10 years"
	</FilesMatch>
</IfModule>
<IfModule mod_rewrite.c>
	RewriteEngine On
	
	RewriteCond %{REQUEST_FILENAME} !-f
	RewriteCond %{REQUEST_FILENAME} !-d
	RewriteRule ^(.*\.(js|css))$ smartoptimizer/?$1
	
	<IfModule mod_expires.c>
		RewriteCond %{REQUEST_FILENAME} -f
		RewriteRule ^(.*\.(js|css|html?|xml|txt))$ smartoptimizer/?$1
	</IfModule>

	<IfModule !mod_expires.c>
		RewriteCond %{REQUEST_FILENAME} -f
		RewriteRule ^(.*\.(gif|jpg|jpeg|png|swf|css|js|html?|xml|txt))$ smartoptimizer/?$1
	</IfModule>
</IfModule>
<FilesMatch "\.(gif|jpg|jpeg|png|swf|css|js|html?|xml|txt)$">
	FileETag none
</FilesMatch>
#
############ End - SmartOptimizer
