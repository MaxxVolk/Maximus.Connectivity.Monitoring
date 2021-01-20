using Maximus.Library.ManagedModuleBase;

using Microsoft.EnterpriseManagement.HealthService;
using Microsoft.EnterpriseManagement.Mom.Modules.DataItems;

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Maximus.Connectivity.Modules
{
  [MonitoringModule(ModuleType.ReadAction)]
  [ModuleOutput(true)]
  class TCPConnectPA : ModuleBaseSimpleAction<PropertyBagDataItem>
  {
    private int TargetIndex, Port, Timeout, MaxReadSize = 16 * 1024;
    private string FullyQualifiedDomainName, TestDisplayName, ResponseRegEx;
    private bool UseSSL, SendData, ExpectResponse;
    private Encoding encoding;
    private byte[] outBuffer = null;

    private readonly Guid enum_ASCII = new Guid("5e210b86-51a5-f911-91ef-1820b192a982");
    private readonly Guid enum_UTF7 = new Guid("35de482e-866c-d4d9-664c-2c28af10c1fd");
    private readonly Guid enum_UTF8 = new Guid("7eaf9641-5dd6-a927-d27e-cee90537f32c");
    private readonly Guid enum_UTF32 = new Guid("eed51547-5bf2-b01f-826f-79cc21c4e53d");
    private readonly Guid enum_Unicode = new Guid("93122f2f-82e0-8cfe-0d5c-dae4334cc317");

    public TCPConnectPA(ModuleHost<PropertyBagDataItem> moduleHost, XmlReader configuration, byte[] previousState) : base(moduleHost, configuration, previousState)
    {
    }

    protected override void PreInitialize(ModuleHost<PropertyBagDataItem> moduleHost, XmlReader configuration, byte[] previousState)
    {
      ModInit.Logger.AddLoggingSource(GetType(), ModInit.evtId_TCPConnectPA);
      base.PreInitialize(moduleHost, configuration, previousState);
    }

    protected override PropertyBagDataItem[] GetOutputData(DataItemBase[] inputDataItems)
    {
      try
      {
        TCPConnectResult result = new TCPConnectResult();
        using (TcpClient tcpConnection = new TcpClient())
        {
          tcpConnection.ReceiveTimeout = Timeout;
          tcpConnection.SendTimeout = Timeout;
          // Stage 1: Connect
          try
          {
            tcpConnection.Connect(FullyQualifiedDomainName, Port);
            result.ConnectSuccess = true;
            result.Message = "Connected OK";
          }
          catch (Exception e)
          {
            result.ConnectSuccess = false;
            result.SetHealth(Microsoft.EnterpriseManagement.Configuration.HealthState.Error);
            result.Message = $"Failed to connect: {e.Message}";
            return new PropertyBagDataItem[] { result.GetPropertyBagDataItem() };
          }
          if (!SendData && !ExpectResponse)
          {
            // job done -- to easy code reading
            result.SetHealth(Microsoft.EnterpriseManagement.Configuration.HealthState.Success);
            return new PropertyBagDataItem[] { result.GetPropertyBagDataItem() };
          }
          // Stage 2: Need Send? or Receive?
          using (Stream tcpStream = UseSSL ? (Stream)new SslStream(tcpConnection.GetStream()) : (Stream)tcpConnection.GetStream())
          {
            if (tcpStream is SslStream sslStream)
              try
              {
                sslStream.AuthenticateAsClient(FullyQualifiedDomainName);
                result.Message += "; Secure Connection OK";
              }
              catch (Exception e)
              {
                result.ConnectSuccess = false;
                result.SetHealth(Microsoft.EnterpriseManagement.Configuration.HealthState.Error);
                result.Message = $"Failed to create secure channel: {e.Message}";
                return new PropertyBagDataItem[] { result.GetPropertyBagDataItem() };
              }
            if (SendData && outBuffer != null && outBuffer.Length > 0)
              try
              {
                tcpStream.Write(outBuffer, 0, outBuffer.Length);
                result.SendSuccess = true;
                result.Message += $"; Sent {outBuffer.Length} bytes, OK";
              }
              catch (Exception e)
              {
                result.SendSuccess = false;
                result.SetHealth(Microsoft.EnterpriseManagement.Configuration.HealthState.Warning);
                result.Message += $"; Failed to send data: {e.Message}";
                return new PropertyBagDataItem[] { result.GetPropertyBagDataItem() };
              }
            if (!ExpectResponse)
            {
              // job done -- to easy code reading
              result.SetHealth(Microsoft.EnterpriseManagement.Configuration.HealthState.Success);
              return new PropertyBagDataItem[] { result.GetPropertyBagDataItem() };
            }
            else
              // if (ExpectResponse)
              try
              {
                byte[] inBuffer = new byte[MaxReadSize];
                int bytesRead = tcpStream.Read(inBuffer, 0, inBuffer.Length);
                if (bytesRead > 0)
                {
                  result.ReceivedSuccess = true;
                  result.Message += $"; Received {bytesRead} bytes (with {MaxReadSize} bytes limit), OK";
                  string readString = encoding.GetString(inBuffer);
                  if (readString.Length > 255)
                    result.ReceivedData = readString.Substring(0, 255);
                  else
                    result.ReceivedData = readString;
                  if (!string.IsNullOrWhiteSpace(ResponseRegEx))
                  {

                    if (Regex.IsMatch(readString, ResponseRegEx))
                    {
                      result.Message += "; Regular expression matched";
                      result.SetHealth(Microsoft.EnterpriseManagement.Configuration.HealthState.Success);
                      return new PropertyBagDataItem[] { result.GetPropertyBagDataItem() };
                    }
                    else
                    {
                      result.Message += "; Regular expression not matched";
                      result.SetHealth(Microsoft.EnterpriseManagement.Configuration.HealthState.Warning);
                      return new PropertyBagDataItem[] { result.GetPropertyBagDataItem() };
                    }
                  }
                  else
                  {
                    // job done 
                    result.SetHealth(Microsoft.EnterpriseManagement.Configuration.HealthState.Success);
                    return new PropertyBagDataItem[] { result.GetPropertyBagDataItem() };
                  }
                }
                else
                {
                  result.ReceivedSuccess = false;
                  result.Message += $"Received no response.";
                  result.SetHealth(Microsoft.EnterpriseManagement.Configuration.HealthState.Warning);
                  return new PropertyBagDataItem[] { result.GetPropertyBagDataItem() };
                }
              }
              catch (Exception e)
              {
                result.ReceivedSuccess = false;
                result.Message = $"Failed to receive data: {e.Message}";
                result.SetHealth(Microsoft.EnterpriseManagement.Configuration.HealthState.Warning);
                return new PropertyBagDataItem[] { result.GetPropertyBagDataItem() };
              }
          }
        }
      }
      catch (Exception e)
      {
        ModuleErrorSignalReceiver(ModuleErrorSeverity.DataLoss, ModuleErrorCriticality.ThrowAndContinue, e, $"Failed to perform TCP Connect test for the {TestDisplayName} test object related to the {FullyQualifiedDomainName}:{TargetIndex} destination object.");
        return null;
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
        LoadConfigurationElement(cfgDoc, "Port", out Port, -1, false);
        LoadConfigurationElement(cfgDoc, "UseSSL", out UseSSL, false, false);
        LoadConfigurationElement(cfgDoc, "Timeout", out Timeout, 15000, false);
        LoadConfigurationElement(cfgDoc, "SendData", out SendData, false, false);
        LoadConfigurationElement(cfgDoc, "RequestBody", out string strRequestBody, "", false);
        LoadConfigurationElement(cfgDoc, "RequestIsBinary", out bool boolRequestIsBinary, false, false);
        LoadConfigurationElement(cfgDoc, "UnescapeBody", out bool boolUnescapeBody, true, false);
        LoadConfigurationElement(cfgDoc, "ExpectResponse", out ExpectResponse, false, false);
        LoadConfigurationElement(cfgDoc, "Encoding", out Guid guidEncoding, Guid.Empty.ToString(), false);
        LoadConfigurationElement(cfgDoc, "ResponseRegEx", out ResponseRegEx, "", false);
        // parse
        if (!string.IsNullOrEmpty(strRequestBody))
        {
          if (boolRequestIsBinary)
          {
            // https://stackoverflow.com/questions/1230303/bitconverter-tostring-in-reverse
            string normalizedStr = strRequestBody.Trim().ToUpperInvariant().Replace(" ", "");
            outBuffer = new byte[normalizedStr.TrimEnd().Length / 2];
            for (int i = 0; i < outBuffer.Length; i++)
              outBuffer[i] = (byte)("0123456789ABCDEF".IndexOf(normalizedStr[i * 2]) * 16 + "0123456789ABCDEF".IndexOf(normalizedStr[i * 2 + 1]));
          }
          else
          {
            if (boolUnescapeBody)
              strRequestBody = Regex.Unescape(strRequestBody);
            encoding = null;
            if (guidEncoding == enum_ASCII)
              encoding = Encoding.ASCII;
            else if (guidEncoding == enum_UTF7)
              encoding = Encoding.UTF7;
            else if (guidEncoding == enum_UTF8)
              encoding = Encoding.UTF8;
            else if (guidEncoding == enum_UTF32)
              encoding = Encoding.UTF32;
            else if (guidEncoding == enum_Unicode)
              encoding = Encoding.Unicode;
            else
              encoding = Encoding.ASCII;
            if (encoding != null)
              outBuffer = encoding.GetBytes(strRequestBody);
          }
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
