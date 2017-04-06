 1. Install phpbb3.
Control Panel -> Fantastico -> Phpbb -> New installation
Fill and submit the form, and finish the installation. Notes:
  Forum directory name can be any, but must not exist. Later can simply rename using FTP.
  Check 'do not delete install directory'.

 2. Convert from PHPBB2.
Disable the old forum to avoid new posts.
Go to http://netenberg.com/phpBB/docs/INSTALL.html#convert and read how to convert phpbb2 to 3.
Convert phpbb2 to 3. Notes:
  It will not touch the phpbb2 files or data.
  To start converting, open http://quickmacros.com/forum4/install in the web browser.
Delete the install folder.

 3. Upgrade to the latest phpbb version.
Log in. Log in to ACP.
ACP -> System. Upgrade to the latest version. To upgrade, follow the instructions.

 4. Edit database.
If needed, cleanup, eg delete recycled topics.
Download db (CP -> Backup).
Open in QM and replace something in the sql file. Notes:
  Replace default charset and collation of every table from latin to utf8. Remove explicit collations of fields. If collations are different, will fail to create mysql search index.
  Replace or remove html, because now not supported.
Upload.

 5. Set forum settings.
Test how forum works.
Set forum settings.
Create search index. Use the Native option (not MySQL) (in ACP -> General -> Search settings too), because otherwise will not find 3-letter words. Also remove 986 th line in includes/search/fulltext_native.php, because by default does not search in [code].

 6. Mods.
The qm mod file is in app\web\forum. Follow the instructions.
If replaced html code, find and resubmit these posts.
Replace html to bbcode in user signatures.
Finally uncheck 'recompile stale style components' in ACP -> General -> Load settings.
Install RSS.

 7. Finally.
Rename old forum folder to 'forum_old'.
Rename the new folder to 'forum'. Also change 'script path' in ACP -> General -> Server settings.
Test everything.
Set email notifications. Ie subscribe to all forums.
