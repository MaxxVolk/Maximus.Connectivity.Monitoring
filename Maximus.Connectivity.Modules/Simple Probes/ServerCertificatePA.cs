using Maximus.Library.ManagedModuleBase;

using Microsoft.EnterpriseManagement.HealthService;
using Microsoft.EnterpriseManagement.Mom.Modules.DataItems;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Maximus.Connectivity.Modules
{
  [MonitoringModule(ModuleType.ReadAction)]
  [ModuleOutput(true)]
  class ServerCertificatePA : ModuleBaseSimpleAction<PropertyBagDataItem>
  {
    private int TargetIndex, Port;
    private string FullyQualifiedDomainName, TestDisplayName, Schema;
    private bool IgnoreRevocationCheck, AllowUnknownCertificateAuthority, IgnoreCertificateAuthorityRevocationUnknown, IgnoreCtlNotTimeValid,
      IgnoreCtlSignerRevocationUnknown, IgnoreEndRevocationUnknown, IgnoreInvalidBasicConstraints, IgnoreInvalidName, IgnoreInvalidPolicy,
      IgnoreNotTimeNested, IgnoreNotTimeValid, IgnoreRootRevocationUnknown, IgnoreWrongUsage;
    private List<Oid> ApplicationPolicy, CertificatePolicy, DisabledHash;
    private List<SslProtocols> AllowedSSLProtocols, DisabledSSLProtocols;
    private SslPolicyErrors PolicyErrors; // not really need a lock

    public ServerCertificatePA(ModuleHost<PropertyBagDataItem> moduleHost, XmlReader configuration, byte[] previousState) : base(moduleHost, configuration, previousState)
    {
    }

    protected override void PreInitialize(ModuleHost<PropertyBagDataItem> moduleHost, XmlReader configuration, byte[] previousState)
    {
      ModInit.Logger.AddLoggingSource(GetType(), ModInit.evtId_ServerCertificatePA);
      base.PreInitialize(moduleHost, configuration, previousState);
    }

    protected override PropertyBagDataItem[] GetOutputData(DataItemBase[] inputDataItems)
    {
      TcpClient tcpClient;
      SslStream sslStream = null;
      int remotePort = -1;
      if (!string.IsNullOrWhiteSpace(Schema))
        try { remotePort = new Uri($"{Schema}://{FullyQualifiedDomainName}").Port; }
        catch (Exception e)
        {
          ModInit.Logger.WriteWarning($"Unable to get port number from {Schema} schema. Will retry using explicit port number (if available).\r\nError message: {e.Message}", this);
        }
      if (remotePort <= 0 && Port >= 0)
        remotePort = Port;
      if (remotePort <= 0)
      {
        ModInit.Logger.WriteWarning($"Unable to get port number from. Test is skipped.", this);
        return null;
      }
      List<SslProtocols> SupportedSslProtocols = new List<SslProtocols>();
      X509Certificate2 remoteCert = null;
      try
      {
        bool authSuccess = false;
        foreach (SslProtocols protocolType in Enum.GetValues(typeof(SslProtocols)))
        {
          if (protocolType == SslProtocols.None || protocolType == SslProtocols.Default) // need explicit values only
            continue;
          try
          {
            tcpClient = new TcpClient(FullyQualifiedDomainName, remotePort);
            sslStream = new SslStream(tcpClient.GetStream(), false, new RemoteCertificateValidationCallback(RecordCertificateProperties))
            {
              ReadTimeout = 15000,
              WriteTimeout = 15000
            };
          }
          catch (Exception e)
          {
            ModInit.Logger.WriteWarning($"Cannot connect to {FullyQualifiedDomainName}:{remotePort} to test certificate. No test performed.\r\nError message: {e.Message}", this);
            try
            {
              if (sslStream != null)
              {
                sslStream.Dispose();
                sslStream = null;
              }
            }
            catch { }
            return null;
          }
          try
          {
            sslStream.AuthenticateAsClient(FullyQualifiedDomainName, null, protocolType, false);
            if (remoteCert == null) // get first available
              remoteCert = new X509Certificate2(sslStream.RemoteCertificate.GetRawCertData());
            SupportedSslProtocols.Add(protocolType);
            authSuccess = true;
          }
          catch
          {
          }
          finally
          {
            if (sslStream != null)
            {
              sslStream.Dispose();
              sslStream = null;
            }
          }
          // then continue to try all of them
        }
        if (!authSuccess || remoteCert == null)
        {
          ModInit.Logger.WriteWarning($"Unable to create SSL connection to {FullyQualifiedDomainName}:{remotePort}. Secure protocol might be not supported.", this);
          return null; // cannot create and SSL stream
        }
        // dump some cert properties in a property bag
        Dictionary<string, object> bagItem = new Dictionary<string, object>
        {
          { "DaysToExpire", Math.Round(remoteCert.NotAfter.Subtract(DateTime.UtcNow).TotalDays, 2) },
          { "StartDate", remoteCert.NotBefore.ToString("yyyy-MM-dd HH:mm:ss") },
          { "EndDate", remoteCert.NotAfter.ToString("yyyy-MM-dd HH:mm:ss") },
          { "Issuer", remoteCert.Issuer },
          { "Subject", remoteCert.Subject },
          { "SerialNumber", remoteCert.SerialNumber },
          { "SignatureAlgorithm", remoteCert.SignatureAlgorithm.FriendlyName },
          { "RemotePort", remotePort },
          { "HasDisabledProtocols", SupportedSslProtocols.Any(sp => DisabledSSLProtocols.Any(dp => sp == dp)) },
          { "DontSupportProtocols", !SupportedSslProtocols.Any(sp => AllowedSSLProtocols.Any(ap => sp == ap)) },
          { "Thumbprint", remoteCert.Thumbprint }
        };
        string supportedSSLProtocolList = "";
        foreach (SslProtocols proto in SupportedSslProtocols)
          supportedSSLProtocolList += $"{proto}; ";
        bagItem.Add("SupportedSslProtocols", supportedSSLProtocolList);
        if (DisabledHash.TrueForAll(x => x.Value != remoteCert.SignatureAlgorithm.Value))
          bagItem.Add("SignatureAlgorithmDisabled", "false");
        else
          bagItem.Add("SignatureAlgorithmDisabled", "true");
        if (PolicyErrors == SslPolicyErrors.None)
          bagItem.Add("PolicyErrors", "NONE");
        else
          bagItem.Add("PolicyErrors", PolicyErrors.ToString());

        // Verify the cert chain
        var certificateChain = new X509Chain(true); // Use Machine Context
        certificateChain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
        certificateChain.ChainPolicy.RevocationMode = IgnoreRevocationCheck ? X509RevocationMode.NoCheck : X509RevocationMode.Online;
        if (ApplicationPolicy != null && ApplicationPolicy.Count > 0)
          foreach (Oid apOid in ApplicationPolicy)
            certificateChain.ChainPolicy.ApplicationPolicy.Add(apOid);
        if (CertificatePolicy != null && CertificatePolicy.Count > 0)
          foreach (Oid caOid in CertificatePolicy)
            certificateChain.ChainPolicy.CertificatePolicy.Add(caOid);

        X509VerificationFlags flags = X509VerificationFlags.NoFlag;
        if (AllowUnknownCertificateAuthority)
          flags |= X509VerificationFlags.AllowUnknownCertificateAuthority;
        if (IgnoreCertificateAuthorityRevocationUnknown)
          flags |= X509VerificationFlags.IgnoreCertificateAuthorityRevocationUnknown;
        if (IgnoreCtlNotTimeValid)
          flags |= X509VerificationFlags.IgnoreCtlNotTimeValid;
        if (IgnoreCtlSignerRevocationUnknown)
          flags |= X509VerificationFlags.IgnoreCtlSignerRevocationUnknown;
        if (IgnoreEndRevocationUnknown)
          flags |= X509VerificationFlags.IgnoreEndRevocationUnknown;
        if (IgnoreInvalidBasicConstraints)
          flags |= X509VerificationFlags.IgnoreInvalidBasicConstraints;
        if (IgnoreInvalidName)
          flags |= X509VerificationFlags.IgnoreInvalidName;
        if (IgnoreInvalidPolicy)
          flags |= X509VerificationFlags.IgnoreInvalidPolicy;
        if (IgnoreNotTimeNested)
          flags |= X509VerificationFlags.IgnoreNotTimeNested;
        if (IgnoreNotTimeValid)
          flags |= X509VerificationFlags.IgnoreNotTimeValid;
        if (IgnoreRootRevocationUnknown)
          flags |= X509VerificationFlags.IgnoreRootRevocationUnknown;
        if (IgnoreWrongUsage)
          flags |= X509VerificationFlags.IgnoreWrongUsage;

        certificateChain.ChainPolicy.VerificationFlags = flags;
        certificateChain.Build(remoteCert);
        if (certificateChain.ChainStatus.Length == 0)
        {
          bagItem.Add("CertificateIsValid", "true");
          bagItem.Add("CertificateStatus", "");
        }
        else
        {
          bagItem.Add("CertificateIsValid", "false");
          string strStatusMessage = "";
          foreach (X509ChainStatus status in certificateChain.ChainStatus)
            strStatusMessage += status.StatusInformation + "; ";
          bagItem.Add("CertificateStatus", strStatusMessage);
        }

        return new List<PropertyBagDataItem>() { CreatePropertyBag(bagItem) }.ToArray();
      }
      catch (Exception e)
      {
        ModuleErrorSignalReceiver(ModuleErrorSeverity.DataLoss, ModuleErrorCriticality.ThrowAndContinue, e, $"Unexpected exception while testing secure connection to the {TestDisplayName} test object related to the {FullyQualifiedDomainName}:{TargetIndex} destination object.");
        return null;
      }
      
    }

    private bool RecordCertificateProperties(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
      PolicyErrors = sslPolicyErrors;
      return true;
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
        LoadConfigurationElement(cfgDoc, "Schema", out Schema, null, false);
        LoadConfigurationElement(cfgDoc, "Port", out Port, -1, false);
        LoadConfigurationElement(cfgDoc, "IgnoreRevocationCheck", out IgnoreRevocationCheck, false, false);
        LoadConfigurationElement(cfgDoc, "ApplicationPolicy", out string strApplicationPolicy, "", false);
        LoadConfigurationElement(cfgDoc, "CertificatePolicy", out string strCertificatePolicy, "", false);
        LoadConfigurationElement(cfgDoc, "DisabledHash", out string strDisabledHash, "", false);
        LoadConfigurationElement(cfgDoc, "AllowedSSLProtocols", out string strAllowedSSLProtocols, "", false);
        LoadConfigurationElement(cfgDoc, "DisabledSSLProtocols", out string strDisabledSSLProtocols, "", false);
        LoadConfigurationElement(cfgDoc, "AllowUnknownCertificateAuthority", out AllowUnknownCertificateAuthority, false, false);
        LoadConfigurationElement(cfgDoc, "IgnoreCertificateAuthorityRevocationUnknown", out IgnoreCertificateAuthorityRevocationUnknown, false, false);
        LoadConfigurationElement(cfgDoc, "IgnoreCtlNotTimeValid", out IgnoreCtlNotTimeValid, false, false);
        LoadConfigurationElement(cfgDoc, "IgnoreCtlSignerRevocationUnknown", out IgnoreCtlSignerRevocationUnknown, false, false);
        LoadConfigurationElement(cfgDoc, "IgnoreEndRevocationUnknown", out IgnoreEndRevocationUnknown, false, false);
        LoadConfigurationElement(cfgDoc, "IgnoreInvalidBasicConstraints", out IgnoreInvalidBasicConstraints, false, false);
        LoadConfigurationElement(cfgDoc, "IgnoreInvalidName", out IgnoreInvalidName, false, false);
        LoadConfigurationElement(cfgDoc, "IgnoreInvalidPolicy", out IgnoreNotTimeNested, false, false);
        LoadConfigurationElement(cfgDoc, "IgnoreNotTimeValid", out IgnoreNotTimeValid, false, false);
        LoadConfigurationElement(cfgDoc, "IgnoreRootRevocationUnknown", out IgnoreRootRevocationUnknown, false, false);
        LoadConfigurationElement(cfgDoc, "IgnoreWrongUsage", out IgnoreWrongUsage, false, false);
        // parse
        ApplicationPolicy = new List<Oid>();
        if (!string.IsNullOrWhiteSpace(strApplicationPolicy))
          foreach (string strOid in strApplicationPolicy.Split(ModInit.Separators, StringSplitOptions.RemoveEmptyEntries))
            try { ApplicationPolicy.Add(new Oid(strOid.Trim())); }
            catch (Exception e)
            {
              ModInit.Logger.WriteWarning($"Error while parsing OID list for Application Policy. Value {strOid} is not recognized as a valid OID. OID was skipped.\r\nError message: {e.Message}", this);
            }
        CertificatePolicy = new List<Oid>();
        if (!string.IsNullOrWhiteSpace(strCertificatePolicy))
          foreach (string strOid in strCertificatePolicy.Split(ModInit.Separators, StringSplitOptions.RemoveEmptyEntries))
            try { CertificatePolicy.Add(new Oid(strOid.Trim())); }
            catch (Exception e)
            {
              ModInit.Logger.WriteWarning($"Error while parsing OID list for Certificate Policy. Value {strOid} is not recognized as a valid OID. OID was skipped.\r\nError message: {e.Message}", this);
            }
        DisabledHash = new List<Oid>();
        foreach (string hashName in strDisabledHash.Split(ModInit.Separators, StringSplitOptions.RemoveEmptyEntries))
        {
          DisabledHash.Add(Oid.FromFriendlyName(hashName.Trim(), OidGroup.SignatureAlgorithm));
        }
        AllowedSSLProtocols = new List<SslProtocols>();
        if (!string.IsNullOrWhiteSpace(strAllowedSSLProtocols))
          foreach (string sslProtoName in strAllowedSSLProtocols.Split(ModInit.Separators, StringSplitOptions.RemoveEmptyEntries))
          {
            if (Enum.TryParse(sslProtoName, true, out SslProtocols protocol))
              AllowedSSLProtocols.Add(protocol);
            else
              ModInit.Logger.WriteWarning($"Error while parsing SSL Protocols list for Allowed SSL Protocols. Value {sslProtoName} is not recognized as a valid SSL protocol name. Value was skipped.", this);
          }
        DisabledSSLProtocols = new List<SslProtocols>();
        if (!string.IsNullOrWhiteSpace(strDisabledSSLProtocols))
          foreach(string sslProtoName in strDisabledSSLProtocols.Split(ModInit.Separators, StringSplitOptions.RemoveEmptyEntries))
          {
            if (Enum.TryParse(sslProtoName, true, out SslProtocols protocol))
              DisabledSSLProtocols.Add(protocol);
            else
              ModInit.Logger.WriteWarning($"Error while parsing SSL Protocols list for Disabled SSL Protocols. Value {sslProtoName} is not recognized as a valid SSL protocol name. Value was skipped.", this);
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
