using SeleniumProxyAuth.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Titanium.Web.Proxy;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Models;

namespace SeleniumProxyAuth
{
    public class SeleniumProxyAuth : IDisposable
    {
        private Dictionary<int, ProxyAuth> proxyAuths;
        private ProxyServer proxyServer;

        public SeleniumProxyAuth()
        {
            proxyAuths = new Dictionary<int, ProxyAuth>();
            proxyServer = new ProxyServer();
            proxyServer.BeforeRequest += OnRequest;
            proxyServer.Start();
        }

        /// <summary>
        /// Adds a new endpoint to the local proxy server
        /// </summary>
        /// <param name="auth">ProxyAuth</param>
        /// <returns>Port where the new proxy will be opened</returns>
        public int AddEndpoint(ProxyAuth auth)
        {
            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] conArr = ipGlobalProperties.GetActiveTcpListeners();

            for (int i = 50000; i < 60000; i++)
            {
                if (conArr.Any(x => x.Port == i)) continue;

                proxyServer.AddEndPoint(new ExplicitProxyEndPoint(IPAddress.Any, i, true));

                proxyAuths.Add(i, auth);
                return i;
            }
            throw new Exception("Couldn't find any available tcp port!");
        }

        /// <summary>
        /// When a new request is received, set up the upstream proxy based on the port of the request
        /// </summary>
        private async Task OnRequest(object sender, SessionEventArgs e)
        {
            if (!proxyAuths.TryGetValue(e.ClientLocalEndPoint.Port, out var auth))
            {
                e.Ok("<html><h>Error with proxy</h></html>");
                return;
            }

            e.CustomUpStreamProxy = new ExternalProxy(auth.Proxy, auth.Port, auth.Username, auth.Password)
            {
                ProxyType = ExternalProxyType.Http
            };
        }

        /// <summary>
        /// Dispose the proxy server
        /// </summary>
        public void Dispose()
        {
            this.proxyServer.Dispose();
        }
    }
}
