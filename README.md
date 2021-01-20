# Selenium proxy authentication library for .NET

### The challenge
Selenium does not support basic proxy authentication out-of-the-box. It is possible to create an **extension** that handles proxy authentication and add the extension to the browser, but if you wish to run the driver **headless**, extensions are not loaded. That's why I created this library.

#### Workaround
The workaround is to create a **local proxy server** (this simple library uses [**Titanium-Web-Proxy**](https://github.com/justcoding121/Titanium-Web-Proxy)) and **handle the proxy authentication** from there.

### Instructions
1. **Create the local proxy server**
```csharp
var proxyServer = new SeleniumProxyServer();
```
2. **Add a new endpoint**
```csharp
var localPort = proxyServer.AddEndpoint(new ProxyAuth("123.123.123.123", 80, "prox-username1", "proxy-password1"));
```
3. **Use the endpoint's port when initializing the driver**
```csharp
options.AddArgument($"--proxy-server=127.0.0.1:{localPort}");
```
### Example
```csharp
public void Test()
{  
    // Create a local proxy server
    var proxyServer = new SeleniumProxyServer();

    // Don't await, have multiple drivers at once using the local proxy server
    TestSeleniumProxyServer(proxyServer, new ProxyAuth("123.123.123.123", 80, "prox-username1", "proxy-password1"));
    TestSeleniumProxyServer(proxyServer, new ProxyAuth("11.22.33.44", 80, "prox-username2", "proxy-password2"));
    TestSeleniumProxyServer(proxyServer, new ProxyAuth("111.222.222.111", 80, "prox-username3", "proxy-password3"));

    while (true) { }
}

private async Task TestSeleniumProxyServer(SeleniumProxyServer proxyServer, ProxyAuth auth)
{
    // Add a new local proxy server endpoint
    var localPort = proxyServer.AddEndpoint(auth);

    ChromeOptions options = new ChromeOptions();
    //options1.AddArguments("headless");

    // Configure the driver's proxy server to the local endpoint port
    options.AddArgument($"--proxy-server=127.0.0.1:{localPort}");

    // Optional
    var service = ChromeDriverService.CreateDefaultService();
    service.HideCommandPromptWindow = true;

    // Create the driver
    var driver = new ChromeDriver(service, options);

    // Test if the driver is working correctly
    driver.Navigate().GoToUrl("https://www.myip.com/");
    await Task.Delay(5000);
    driver.Navigate().GoToUrl("https://amibehindaproxy.com/");
    await Task.Delay(5000);

    // Dispose the driver
    driver.Dispose();
}
```