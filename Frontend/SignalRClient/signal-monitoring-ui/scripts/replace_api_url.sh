#!/bin/sh
start.sh
# replace static values with environment-variables
     if [ -n "$RESOURCEA" ]; then
         sed -i "s#RESOURCEA#$AUTHSERVER_URL#g" /usr/share/nginx/html/main.*.bundle.js
     fi
     # start webserver
     exec nginx
