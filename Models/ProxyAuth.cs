using System;
using System.Collections.Generic;
using System.Text;

namespace SeleniumProxyAuth.Models
{
    public class ProxyAuth
    {
        public ProxyAuth(string proxy, int port, string username, string password)
        {
            Proxy = proxy;
            Port = port;
            Username = username;
            Password = password;
        }

        public string Proxy { get; }
        public int Port { get; }
        public string Username { get; }
        public string Password { get; }

    }
}
