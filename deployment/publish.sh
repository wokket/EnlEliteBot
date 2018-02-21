#!/bin/bash

# Build release package of the EnlEliteBot

cd ../EnlEliteBot.Web
dotnet publish -c Release

echo
echo 'Uploading published files to temp directory'
#copy the new files up to a tmp directory, -C = use compression, -p = preserve modified timestamps etc, -r = recursive
scp -C -p -r bin/Release/netcoreapp2.0/publish/* tim@163.172.163.77:/tmp/bot_publish


echo
echo 'Connecting to server to restart process...'
# connect to the server, -t = feed terminal input back for passwords etc
ssh -t tim@163.172.163.77 \
    'sudo rm -r -f /var/enlEliteBot_old && \
     sudo mv /var/enlEliteBot /var/enlEliteBot_old && \
     sudo mkdir /var/enlEliteBot && \
     sudo mv /tmp/bot_publish/* /var/enlEliteBot && \
     sudo chgrp www-data /var/enlEliteBot && \
     sudo chmod 0755 /var/enlEliteBot && \
     sudo supervisorctl restart enlEliteBot'


