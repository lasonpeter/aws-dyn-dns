# Dynamic DNS update script for [AWS Route 53](aws.amazon.com/route53)

## Usage
Set AWS Route 53 credentials and information in config.json then execute the program
<br>
Preferred way to use the script is to add it as a `cron` routine, for Linux systems example cron routine would look like this:
```cronexp
*/2 * * * * /home/server/dyndns/dyndns.sh
```
in such configuration the script will run every 2 minutes, this can be adjusted to your needs

where `dyndns.sh` looks like this:
```bash
#!/bin/bash
cd /path/to/binary/directory/
/path/to/binary/dyn-dns
```
