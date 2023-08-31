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
    }
}
