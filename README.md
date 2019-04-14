# DomainTesting

Small program designed to read IIS bindings in an IIS-based web server. The program will ping the URLs (www and non-www) found
within ALL IIS bindings (regardless of the number of sites and associated domains on those sites).

The result is compared against pre-defined IP addresses to determine if the domains are pointing to said IP address(es) or not.

Will email a recipient with a list of domains that DO NOT point to specified IP addresses using Sendgrid.

