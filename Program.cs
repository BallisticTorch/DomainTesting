﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Web.Administration;
using System.IO;
using System.Net;
using SendGrid;
using System.Net.Sockets;
using System.Net.Mail;

namespace DomainTestingV2
{
    class Program
    {
        static void Main(string[] args)
        {
            StringBuilder sb = new StringBuilder();

            using (ServerManager iis = new ServerManager())
            {   //Get domain names from IIS bindings
                foreach (Site site in iis.Sites.OrderBy(a => a.Name))
                {
                    if (site.Name == "Default Web Site")
                        continue;
                    List<string> bindings = new List<string>();
                    foreach (Microsoft.Web.Administration.Binding bind in site.Bindings.Reverse().Where(a => !a.Host.Contains("ndrv.in")).Where(a => !a.Host.Contains("nd-cdn.us")))
                    {
                        try
                        {
                            if (!String.IsNullOrEmpty(bind.Host) && (bind.Host.IndexOf("www.") == -1))
                            {
                                IPHostEntry hostEntry = Dns.GetHostEntry(bind.Host);
                                if (!(hostEntry.AddressList[0].ToString() == "184.106.2.74") && !(hostEntry.AddressList[0].ToString() == "10.96.191.74") &&
                                    !(hostEntry.AddressList[0].ToString() == "184.106.2.72") && !(hostEntry.AddressList[0].ToString() == "10.96.191.72") &&
                                    !(hostEntry.AddressList[0].ToString() == "184.106.2.76") && !(hostEntry.AddressList[0].ToString() == "10.96.191.76") &&
                                    !(hostEntry.AddressList[0].ToString() == "184.106.2.75") && !(hostEntry.AddressList[0].ToString() == "10.96.191.75") &&
                                    !(hostEntry.AddressList[0].ToString() == "127.0.0.1"))

                                    sb.AppendLine(site + "," + bind.Host + "," + hostEntry.AddressList[0].ToString()); //add Domain that fails to resolve to webserver IPs
                            }
                        }
                        catch (Exception ex)
                        {
                            if (ex is ArgumentNullException || ex is ArgumentOutOfRangeException || ex is ArgumentException || ex is SocketException)
                            {
                                sb.AppendLine(site + "," + bind.Host + "," + ex.Message);                                
                            }
                            else
                            {
                                sb.AppendLine(ex.ToString());
                            }
                        }
                    }
                }
            }

            string emailBody = sb.ToString();

            var myMessage = new SendGridMessage();

            myMessage.AddTo("bcoleman@netdriven.com");
            myMessage.From = new MailAddress("tech@netdriven.com");
            myMessage.Subject = Environment.MachineName;
            myMessage.Text = emailBody;

            var credentials = new NetworkCredential("netdriven", "tirewob101");

            var transportWeb = new Web(credentials);

            transportWeb.Deliver(myMessage);
        }
    }
}
