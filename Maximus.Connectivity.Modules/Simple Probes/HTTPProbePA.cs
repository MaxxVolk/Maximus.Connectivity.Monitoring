using Maximus.Library.ManagedModuleBase;

using Microsoft.EnterpriseManagement.HealthService;
using Microsoft.EnterpriseManagement.Mom.Modules.DataItems;

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace Maximus.Connectivity.Modules
{
  [MonitoringModule(ModuleType.ReadAction)]
  [ModuleOutput(true)]
  public class HTTPProbePA : ModuleBaseSimpleAction<PropertyBagDataItem>
  {
    private int TargetIndex;
    private bool AllowRedirect, ExpressionIsNegative;
    private string FullyQualifiedDomainName, TestDisplayName, HTTPMethod, AuthenticationScheme, RequestBody, RegularExpression;
    private NameValueCollection Headers;
    private CredentialCache Authentication = null;
    private Uri Destination;

    public HTTPProbePA(ModuleHost<PropertyBagDataItem> moduleHost, XmlReader configuration, byte[] previousState) : base(moduleHost, configuration, previousState)
    {
    }

    protected override void PreInitialize(ModuleHost<PropertyBagDataItem> moduleHost, XmlReader configuration, byte[] previousState)
    {
      ModInit.Logger.AddLoggingSource(GetType(), ModInit.evtId_HTTPProbePA);
      base.PreInitialize(moduleHost, configuration, previousState);
    }

    protected override PropertyBagDataItem[] GetOutputData(DataItemBase[] inputDataItems)
    {
      DateTime startTime = DateTime.UtcNow;
      try
      {
        HttpWebRequest Request = WebRequest.CreateHttp(Destination);
        Request.Method = string.IsNullOrEmpty(HTTPMethod) ? "GET" : HTTPMethod;
        SetHeaders(Request, Headers);
        if (Authentication != null)
          Request.Credentials = Authentication;
        Request.AllowAutoRedirect = AllowRedirect;

        if (!string.IsNullOrEmpty(RequestBody))
        {
          UTF8Encoding encoding = new UTF8Encoding();
          byte[] byteBody = encoding.GetBytes(RequestBody);
          Request.ContentLength = byteBody.Length;
          startTime = DateTime.UtcNow;
          using (Stream dataStream = Request.GetRequestStream())
            dataStream.Write(byteBody, 0, byteBody.Length);
        }

        using (WebResponse Response = Request.GetResponse())
        {
          using (Stream ResponseStream = Response.GetResponseStream())
          {
            using (StreamReader Reader = new StreamReader(ResponseStream, Encoding.UTF8))
            {
              string FullResponse = Reader.ReadToEnd();
              Dictionary<string, object> bagItem = new Dictionary<string, object>();
              if (string.IsNullOrWhiteSpace(RegularExpression)) // no reg ex check
              {
                bagItem.Add("State", "OK");
                bagItem.Add("Error", "");
                bagItem.Add("Response", FullResponse.Substring(0, FullResponse.Length >= 255 ? 255 : FullResponse.Length));
                bagItem.Add("ResponseLength", FullResponse.Length);
                bagItem.Add("ResponseTimeMs", DateTime.UtcNow.Subtract(startTime).TotalMilliseconds);
                bagItem.Add("EffectiveUri", Destination.ToString());
                //ModInit.Logger.WriteWarning($"{FullyQualifiedDomainName}. RegEx: {RegularExpression}, ExpressionIsNegative: {ExpressionIsNegative}, State: OK", this);
                return new List<PropertyBagDataItem>() { CreatePropertyBag(bagItem) }.ToArray();
              }

              bool regEdMatched = Regex.IsMatch(FullResponse, RegularExpression);
              if (ExpressionIsNegative)
                regEdMatched = !regEdMatched;
              if (regEdMatched)
              {
                bagItem.Add("State", "OK");
                bagItem.Add("Error", "");
                bagItem.Add("Response", FullResponse.Substring(0, FullResponse.Length >= 255 ? 255 : FullResponse.Length));
                bagItem.Add("ResponseLength", FullResponse.Length);
                bagItem.Add("ResponseTimeMs", DateTime.UtcNow.Subtract(startTime).TotalMilliseconds);
                bagItem.Add("EffectiveUri", Destination.ToString());
                //ModInit.Logger.WriteWarning($"{FullyQualifiedDomainName}. RegEx: {RegularExpression}, regEdMatched:{regEdMatched}, ExpressionIsNegative: {ExpressionIsNegative}, State: OK", this);
              }
              else
              {
                bagItem.Add("State", "WARNING");
                bagItem.Add("Error", "Page regular expression does not match.");
                bagItem.Add("Response", FullResponse.Substring(0, FullResponse.Length >= 255 ? 255 : FullResponse.Length));
                bagItem.Add("ResponseLength", FullResponse.Length);
                bagItem.Add("ResponseTimeMs", DateTime.UtcNow.Subtract(startTime).TotalMilliseconds);
                bagItem.Add("EffectiveUri", Destination.ToString());
                //ModInit.Logger.WriteWarning($"{FullyQualifiedDomainName}. RegEx: {RegularExpression}, regEdMatched:{regEdMatched}, ExpressionIsNegative: {ExpressionIsNegative}, State: WARNING", this);
              }
              return new List<PropertyBagDataItem>() { CreatePropertyBag(bagItem) }.ToArray();
            }
          }
        }
      }
      catch (Exception e)
      {
        Dictionary<string, object> bagItem = new Dictionary<string, object>
        {
          { "State", "ERROR" },
          { "Error", e.Message },
          { "Response", "" },
          { "ResponseLength", 0 },
          { "ResponseTimeMs", -1 },
          { "EffectiveUri", Destination.ToString() }
        };
        return new List<PropertyBagDataItem>() { CreatePropertyBag(bagItem) }.ToArray();
      }
    }

    public static void SetHeaders(HttpWebRequest request, NameValueCollection headers)
    {
      foreach (string valueKey in headers.Keys)
        switch (valueKey)
        {
          // https://docs.microsoft.com/en-us/dotnet/api/system.net.httpwebrequest.headers?view=netframework-4.7.2
          case "Accept": request.Accept = headers[valueKey]; break;
          case "Connection": request.Connection = headers[valueKey]; break;
          case "Content-Length": request.ContentLength = int.Parse(headers[valueKey]); break;
          case "Content-Type": request.ContentType = headers[valueKey]; break;
          case "Expect": request.Expect = headers[valueKey]; break;
          case "Date": request.Date = DateTime.Parse(headers[valueKey]); break;
          case "Host": request.Host = headers[valueKey]; break;
          case "If-Modified-Since": request.IfModifiedSince = DateTime.Parse(headers[valueKey]); break;
          case "Range": break; // not supported  request.AddRange= headers[valueKey]; break;
          case "Referer": request.Referer = headers[valueKey]; break;
          case "Transfer-Encoding": request.TransferEncoding = headers[valueKey]; break;
          case "User-Agent": request.UserAgent = headers[valueKey]; break;
          default: request.Headers.Add(valueKey, headers[valueKey]); break;
        }
    }

    protected override void ModuleErrorSignalReceiver(ModuleErrorSeverity severity, ModuleErrorCriticality criticality, Exception e, string message, params object[] extaInfo)
    {
      ModInit.ModuleErrorSignalReceiver(severity, criticality, e, message, this);
    }

    protected override void LoadConfiguration(XmlDocument cfgDoc)
    {
      try
      {
        // base class properties
        LoadConfigurationElement(cfgDoc, "TestDisplayName", out TestDisplayName, "<no test name provided>", false); // for logging purposes only
        // parent class property
        LoadConfigurationElement(cfgDoc, "FullyQualifiedDomainName", out FullyQualifiedDomainName);
        LoadConfigurationElement(cfgDoc, "TargetIndex", out TargetIndex);
        // specific class properties
        LoadConfigurationElement(cfgDoc, "Schema", out string strSchema, "http", false);
        LoadConfigurationElement(cfgDoc, "Path", out string strPath, "", false);
        LoadConfigurationElement(cfgDoc, "Port", out int intPort, -1, false);
        LoadConfigurationElement(cfgDoc, "AllowRedirect", out AllowRedirect, true, false);
        LoadConfigurationElement(cfgDoc, "HTTPMethod", out HTTPMethod, "GET", false);
        LoadConfigurationElement(cfgDoc, "AuthenticationScheme", out AuthenticationScheme, "Basic", false);
        LoadConfigurationElement(cfgDoc, "RequestBody", out RequestBody, "", false);
        LoadConfigurationElement(cfgDoc, "RegularExpression", out RegularExpression, "", false);
        LoadConfigurationElement(cfgDoc, "ExpressionIsNegative", out ExpressionIsNegative, false, false);
        LoadConfigurationElement(cfgDoc, "ExtraHeaders", out string strExtraHeaders, "", false);
        LoadConfigurationElement(cfgDoc, "Host", out string strHost, "", false);
        LoadConfigurationElement(cfgDoc, "Accept", out string strAccept, "", false);
        LoadConfigurationElement(cfgDoc, "ContentType", out string strContentType, "", false);
        LoadConfigurationElement(cfgDoc, "UserAgent", out string strUserAgent, "", false);
        LoadConfigurationElement(cfgDoc, "Username", out string strUsername , "", false);
        LoadConfigurationElement(cfgDoc, "Password", out string strPassword, "", false);
        // parse
        Destination = new Uri($"{strSchema}://{FullyQualifiedDomainName}{(intPort > 0 ? $":{intPort}" : "")}/{strPath}");
        Headers = new NameValueCollection();
        if (string.IsNullOrWhiteSpace(strHost))
          Headers.Add("Host", Destination.Host);
        else
          Headers.Add("Host", strHost);
        if (!string.IsNullOrWhiteSpace(strAccept))
          Headers.Add("Accept", strAccept);
        if (!string.IsNullOrWhiteSpace(strContentType))
          Headers.Add("Content-Type", strContentType);
        if (!string.IsNullOrWhiteSpace(strUserAgent))
          Headers.Add("User-Agent", strUserAgent);
        char[] headerSeparator = new char[] { '|' };
        char[] headerNameSeparator = new char[] { ':' };
        foreach (string headerNVPair in strExtraHeaders.Split(headerSeparator, StringSplitOptions.RemoveEmptyEntries))
        {
          if (string.IsNullOrWhiteSpace(headerNVPair))
            continue;
          string[] thePair = headerNVPair.Split(headerNameSeparator);
          if (thePair == null || thePair.Length < 1 || thePair.Length > 2) // 1 => header with empty value, 2 => header and value
            continue;
          if (Headers.AllKeys.Contains(thePair[0].Trim()))
            continue;
          if (thePair.Length == 1)
            Headers.Add(thePair[0].Trim(), "");
          else
            Headers.Add(thePair[0].Trim(), thePair[1].Trim());
        }
        string strUsernameDomain = "";
        if (!string.IsNullOrWhiteSpace(strUsername) && strUsername.IndexOf('\\') > 0) // strict condition, because domain name must have at least 1 char
        {
          char[] domainSeparator = new char[] { '\\' };
          strUsernameDomain = strUsername.Split(domainSeparator)[0];
          strUsername = strUsername.Split(domainSeparator)[1];
        }
        if (!string.IsNullOrWhiteSpace(AuthenticationScheme) && !string.IsNullOrWhiteSpace(strUsername))
        {
          Authentication = new CredentialCache();
          if (string.IsNullOrWhiteSpace(strUsernameDomain))
            Authentication.Add(new Uri($"{strSchema}://{FullyQualifiedDomainName}{(intPort > 0 ? $":{intPort}" : "")}"), AuthenticationScheme, new NetworkCredential(strUsername, strPassword));
          else
            Authentication.Add(new Uri($"{strSchema}://{FullyQualifiedDomainName}{(intPort > 0 ? $":{intPort}" : "")}"), AuthenticationScheme, new NetworkCredential(strUsername, strPassword, strUsernameDomain));
        }
      }
      catch (Exception e)
      {
        ModuleErrorSignalReceiver(ModuleErrorSeverity.FatalError, ModuleErrorCriticality.Stop, e, "Failed to load module configuration.");
        throw new ModuleException("Failed to load module configuration.", e);
      }
    }
  }
}
