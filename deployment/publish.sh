#!/bin/bash

# get directory hostng the scipt
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"

# Build release package of the EnlEliteBot

cd $DIR/../EnlEliteBot.Web
dotnet publish -c Release

echo
echo 'Removing config file, leave the prod file where it is.  WARNING: This may break you if you''ve changed the config locally....'
rm bin/Release/netcoreapp2.0/publish/appsettings.json

echo
echo 'Adding installation script'
cp $DIR/install.sh bin/Release/netcoreapp2.0/publish/

echo
echo 'Uploading published files to temp directory'
#copy the new files up to a tmp directory, -C = use compression, -p = preserve modified timestamps etc, -r = recursive
scp -C -p -r bin/Release/netcoreapp2.0/publish/* tim@163.172.163.77:/tmp/bot_publish


echo
echo 'Connecting to server to restart process...'
# connect to the server, -t = feed terminal input back for passwords etc
ssh -t tim@163.172.163.77 '/tmp/bot_publish/install.sh'


