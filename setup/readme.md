# Setup

- Create user with name 'QPang'

- Copy `qserver_auth.service` and eddit file

- Do `dotnet pubish -o OUTPUT_DIR`

- Config the `.service` file correctly

## Create User
- `sudo adduser QPang` # Create User
- `sudo passwd -d QPang` # Remove Password
- `sudo mkdir /home/QPang/` # Create home dir
- `sudo usermod -aG sudo QPang` # Add to Sudoers
## Publish
- `git pull` or `git clone` # get latest changes
- `./publish.sh` # publish project
- `systemctl restart qserver_auth` # restart service

## Systemctl
- `cp qserver_auth.service /etc/systemd/systen/` # add
- `systemctl start qserver_auth` # start
- `systemctl status qserver_auth` # get info
- `systemctl stop qserver_auth` # stop for some reason

TODO: add scripts for publishing and whatnot?
