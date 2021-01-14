# Maximus Connectivity Monitoring
## Management Pack for System Center Operations Manager 
Multi-protocol connectivity testing for SCOM, including ping, HTTP, SSL/TLS, etc.

User manual is available at: http://maxcoreblog.com/2021/01/14/maximus-connectivity-monitoring/

See "making of" article at http://maxcoreblog.com/2021/01/11/connectivity-monitoring-scom-management-pack-the-making-of/

Main management pack features:

  - All intuitive GUI driven configuration.
  - Extendable -- other management pack authors can add more tests, which will be picked up by UI.


Tests objects implemented in the current release:
  - Ping (2 monitors and 1 performance collection rule)
  - SSL Connection and Remote Certificate Validation (6 monitors)
  - HTTP Probe (1 monitor and 1 performance collection rule)
  - DNS Name Resolution (1 monitor)
  - SQL Server (4 monitors and 3 performance collection rule) via extension: https://github.com/MaxxVolk/Maximus.Connectivity.SQLExtension.Monitoring

The main idea behind this management pack is to create flexible infrastructure, 
which allows to create and configure a suitable monitoring sets for each destination. 
Say, for instance, if you have an SQL server, you can add it as a destination,
then add 'Ping', 'DNS Resolution', 'SSL', 'TCP Connect', and 'SQL Synthetic transaction' test objects.
For a mail server, you can create a destination and add 'Ping', 'DNS Resolution', 'SSL', 'TCP Connect',
'SMTP Send', and 'IMAP Receive' modules. Or, for an S3-compatible storage service you can make
a set of 'Ping', 'DNS Resolution', 'SSL', 'TCP Connect', and 'S3 Synthetic Transaction'.

Example extension simple MP will be added soon, meanwhile https://github.com/MaxxVolk/Maximus.Connectivity.SQLExtension.Monitoring
can be used as example.


