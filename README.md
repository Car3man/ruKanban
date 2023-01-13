# ruKanban

### Deployment on machine (Ubuntu 22.04)

1. Be sure that you have installed git
2. Run `git-init.sh` script to initialize git, skip if already set
3. Run `install-docker.sh` script to install docker, skip if already set
4. Run `pgadmin-init.sh` script to install pgadmin (optional)
5. Run `ru-kanban-init.sh` script in parent folder of future project directory to prepare project repository
6. Every time that you want to run project, run `app-run.sh` script with sudo