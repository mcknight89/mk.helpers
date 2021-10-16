using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace mk.helpers
{
    public static class IpHelper
    {
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

        public static string NumberToIpAddress(long address)
        {
            return IPAddress.Parse(address.ToString()).ToString();
        }
    }
}
