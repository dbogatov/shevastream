# Deploy and build instructions #

Login to virtual machine
```
ssh root@37.139.24.88
```

Test build
```
dnu restore
dnu build
dnx web
```

Publish
```
dnu publish --runtime dnx-coreclr-linux-x64.1.0.0-rc1-update1 --out /var/aspnet/shevastream
```

Test publish
```
/var/aspnet/shevastream/approot/web
```

Test load
```
nmap -p 80 localhost
curl localhost
```

## Supervisor ##

General commands
```
supervisorctl reread
supervisorctl update

supervisorctl stop shevastream
supervisorctl start shevastream
supervisorctl 
```

Logs
```
less /var/log/shevastream.out.log
less /var/log/shevastream.err.log
```

General conf file
```
; supervisor config file

[inet_http_server]
port = 9001
username = dima4ka # Basic auth username
password = Lapulenka1 # Basic auth password

[unix_http_server]
file=/var/run/supervisor.sock   ; (the path to the socket file)
chmod=0700                       ; sockef file mode (default 0700)

[supervisord]
logfile=/var/log/supervisor/supervisord.log ; (main log file;default $CWD/supervisord.log)
pidfile=/var/run/supervisord.pid ; (supervisord pidfile;default supervisord.pid)
childlogdir=/var/log/supervisor            ; ('AUTO' child log dir, default $TEMP)

; the below section must remain in the config file for RPC
; (supervisorctl/web interface) to work, additional interfaces may be
; added by defining them in separate rpcinterface: sections
[rpcinterface:supervisor]
supervisor.rpcinterface_factory = supervisor.rpcinterface:make_main_rpcinterface

[supervisorctl]
serverurl=unix:///var/run/supervisor.sock ; use a unix:// URL  for a unix socket

; The [include] section can just contain the "files" setting.  This
; setting can list multiple files (separated by whitespace or
; newlines).  It can also contain wildcards.  The filenames are
; interpreted as relative to this file.  Included files *cannot*
; include files themselves.

[include]
files = /etc/supervisor/conf.d/*.conf
```

Specific conf file
```
[program:shevastream]
command=bash /var/aspnet/shevastream/approot/web
autostart=true
autorestart=true
stderr_logfile=/var/log/shevastream.err.log
stdout_logfile=/var/log/shevastream.out.log
environment=ASPNET_ENV=Production
user=www-data
stopsignal=INT
```

## Nginx ##

Conf file
```
# You may add here your
# server {
#	...
# }
# statements for each of your virtual hosts to this file

##
# You should look at the following URL's in order to grasp a solid understanding
# of Nginx configuration files in order to fully unleash the power of Nginx.
# http://wiki.nginx.org/Pitfalls
# http://wiki.nginx.org/QuickStart
# http://wiki.nginx.org/Configuration
#
# Generally, you will want to move this file somewhere, and start with a clean
# file but keep this around for reference. Or just disable in sites-enabled.
#
# Please see /usr/share/doc/nginx-doc/examples/ for more detailed examples.
##

server {
	listen 80;
	# listen [::]:80 default_server ipv6only=on;

	# root /usr/share/nginx/html;
	# index index.html index.htm;

	# Make site accessible from http://localhost/
	# server_name localhost;

	location / {
		# First attempt to serve request as file, then
		# as directory, then fall back to displaying a 404.
		# try_files $uri $uri/ =404;
		# Uncomment to enable naxsi on this location
		# include /etc/nginx/naxsi.rules
		proxy_pass http://localhost:5000;
		# proxy_buffering off;
		# keepalive_requests 0;
		proxy_set_header Connection keep-alive;
		## proxy_pass http://unix:/var/aspnet/shevastream/kestrel.sock;
         	## proxy_http_version 1.1;
         	## proxy_set_header Upgrade $http_upgrade;
         	## proxy_set_header Connection keep-alive;
         	## proxy_set_header Host $host;
         	## proxy_cache_bypass $http_upgrade;
	}

	# Only for nginx-naxsi used with nginx-naxsi-ui : process denied requests
	#location /RequestDenied {
	#	proxy_pass http://127.0.0.1:8080;    
	#}

	#error_page 404 /404.html;

	# redirect server error pages to the static page /50x.html
	#
	#error_page 500 502 503 504 /50x.html;
	#location = /50x.html {
	#	root /usr/share/nginx/html;
	#}

	# pass the PHP scripts to FastCGI server listening on 127.0.0.1:9000
	#
	#location ~ \.php$ {
	#	fastcgi_split_path_info ^(.+\.php)(/.+)$;
	#	# NOTE: You should have "cgi.fix_pathinfo = 0;" in php.ini
	#
	#	# With php5-cgi alone:
	#	fastcgi_pass 127.0.0.1:9000;
	#	# With php5-fpm:
	#	fastcgi_pass unix:/var/run/php5-fpm.sock;
	#	fastcgi_index index.php;
	#	include fastcgi_params;
	#}

	# deny access to .htaccess files, if Apache's document root
	# concurs with nginx's one
	#
	#location ~ /\.ht {
	#	deny all;
	#}
}


# another virtual host using mix of IP-, name-, and port-based configuration
#
#server {
#	listen 8000;
#	listen somename:8080;
#	server_name somename alias another.alias;
#	root html;
#	index index.html index.htm;
#
#	location / {
#		try_files $uri $uri/ =404;
#	}
#}


# HTTPS server
#
#server {
#	listen 443;
#	server_name localhost;
#
#	root html;
#	index index.html index.htm;
#
#	ssl on;
#	ssl_certificate cert.pem;
#	ssl_certificate_key cert.key;
#
#	ssl_session_timeout 5m;
#
#	ssl_protocols SSLv3 TLSv1 TLSv1.1 TLSv1.2;
#	ssl_ciphers "HIGH:!aNULL:!MD5 or HIGH:!aNULL:!MD5:!3DES";
#	ssl_prefer_server_ciphers on;
#
#	location / {
#		try_files $uri $uri/ =404;
#	}
#}
```

Commands
```
sudo service nginx restart
```

## SSL ##

* http://stackoverflow.com/questions/24455238/lxml-installation-error-ubuntu-14-04-internal-compiler-error/26762938#26762938
* https://community.letsencrypt.org/t/how-to-nginx-configuration-to-enable-acme-challenge-support-on-all-http-virtual-hosts/5622
* https://www.digitalocean.com/community/tutorials/how-to-secure-nginx-with-let-s-encrypt-on-ubuntu-14-04

## PostgeSQL ##

* https://github.com/sosedoff/pgweb/wiki/Usage
* https://www.digitalocean.com/community/tutorials/how-to-install-and-use-postgresql-on-ubuntu-14-04
* https://www.digitalocean.com/community/tutorials/how-to-set-up-a-firewall-using-iptables-on-ubuntu-14-04

