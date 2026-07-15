using EDDY.IS.FormsEngine.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Core.Services
{
    public class IPAddressService: IIPAddressService
    {
        /// <summary>
        /// Gets the user's IP Address
        /// </summary>
        /// <param name="httpVia">HTTP_VIA Server variable</param>
        /// <param name="httpXForwardedFor">HTTP_X_FORWARDED Server variable</param>
        /// <param name="RemoteAddr">REMOTE_ADDR Server variable</param>
        /// <returns>user's IP Address</returns>

        public string GetIPAddress(string HttpVia, string HttpXForwardedFor, string RemoteAddr)
        {
            // Use a default address if all else fails.
            string result = "127.0.0.1";

            // Web user - if using proxy
            string tempIP = string.Empty;

            if (HttpVia != null)
                tempIP = HttpXForwardedFor;
            else // Web user - not using proxy or can't get the Client IP
                tempIP = RemoteAddr;

            // If we can't get a V4 IP from the above, try host address list for internal users.
            if (!IsIPV4(tempIP))
            {
                try
                {
                    string hostName = System.Net.Dns.GetHostName();
                    foreach (System.Net.IPAddress ip in System.Net.Dns.GetHostAddresses(hostName))
                    {
                        if (IsIPV4(ip))
                        {
                            result = ip.ToString();
                            break;
                        }
                    }
                }
                catch { }
            }

            return result;
        }


        /// <summary>
        /// Determines weather an IP Address is V4
        /// </summary>
        /// <param name="input">input string</param>
        /// <returns>Is IPV4 True or False</returns>
        private bool IsIPV4(string input)
        {
            bool result = false;
            System.Net.IPAddress address = null;

            if (System.Net.IPAddress.TryParse(input, out address))
                result = IsIPV4(address);

            return result;

        }

        /// <summary>
        /// Determines weather an IP Address is V4
        /// </summary>

        /// <param name="address">input IP address</param>

        /// <returns>Is IPV4 True or False</returns>

        private bool IsIPV4(System.Net.IPAddress address)
        {
            bool result = false;

            switch (address.AddressFamily)
            {
                case System.Net.Sockets.AddressFamily.InterNetwork:   // we have IPv4
                    result = true;
                    break;
                case System.Net.Sockets.AddressFamily.InterNetworkV6: // we have IPv6
                    break;
                default:
                    break;
            }

            return result;
        }
    }
}
