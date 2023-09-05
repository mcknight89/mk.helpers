using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace mk.helpers
{
    /// <summary>
    /// Provides utility methods for working with IP addresses and their numeric representations.
    /// </summary>
    public static class IpHelper
    {
        /// <summary>
        /// Converts an IP address string to its corresponding numeric representation.
        /// </summary>
        /// <param name="address">The IP address string to convert.</param>
        /// <returns>The numeric representation of the IP address.</returns>
        public static long IpAddressToNumber(string address)
        {
            try
            {
                return (long)(uint)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(IPAddress.Parse(address).GetAddressBytes(), 0));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Converts a numeric IP address representation to its corresponding string representation.
        /// </summary>
        /// <param name="address">The numeric IP address to convert.</param>
        /// <returns>The string representation of the IP address.</returns>
        public static string NumberToIpAddress(long address)
        {
            return IPAddress.Parse(address.ToString()).ToString();
        }


        /// <summary>
        /// Checks if the provided IP address is valid.
        /// </summary>
        /// <param name="ipAddress">The IP address to validate.</param>
        /// <returns>True if the IP address is valid; otherwise, false.</returns>
        public static bool IsValid(string ipAddress)
        {
            IPAddress parsedIp;
            return IPAddress.TryParse(ipAddress, out parsedIp);
        }

        /// <summary>
        /// Checks if the provided IP address is a valid IPv4 address.
        /// </summary>
        /// <param name="ipAddress">The IP address to validate.</param>
        /// <returns>True if the IP address is a valid IPv4 address; otherwise, false.</returns>
        public static bool IsValidIpv4(string ipAddress)
        {
            if (IsValid(ipAddress))
            {
                IPAddress parsedIp = IPAddress.Parse(ipAddress);
                return parsedIp.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork;
            }
            return false;
        }

        /// <summary>
        /// Checks if the provided IP address is a valid IPv6 address.
        /// </summary>
        /// <param name="ipAddress">The IP address to validate.</param>
        /// <returns>True if the IP address is a valid IPv6 address; otherwise, false.</returns>
        public static bool IsValidIpv6(string ipAddress)
        {
            if (IsValid(ipAddress))
            {
                IPAddress parsedIp = IPAddress.Parse(ipAddress);
                return parsedIp.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6;
            }
            return false;
        }

    }
}
