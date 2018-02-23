#!/bin/bash

# to be run locally on the server in order to upgrade the bot

#remove the old backup
sudo rm -r -f /var/enlEliteBot_old

#create a backup of the current version
sudo mv /var/enlEliteBot /var/enlEliteBot_old

# we need an installation path
sudo mkdir /var/enlEliteBot

# copy the new files
sudo mv /tmp/bot_publish/* /var/enlEliteBot

# lock down perms
sudo chgrp www-data /var/enlEliteBot
sudo chmod 0755 /var/enlEliteBot

# restore the config file... the slack token lives in here
sudo cp /var/enlEliteBot_old/appsettings.json /var/enlEliteBot

# restart the service
sudo supervisorctl restart enlEliteBot