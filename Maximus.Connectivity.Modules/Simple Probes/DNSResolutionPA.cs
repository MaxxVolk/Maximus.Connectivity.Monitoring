using Maximus.Library.ManagedModuleBase;

using Microsoft.EnterpriseManagement.HealthService;
using Microsoft.EnterpriseManagement.Mom.Modules.DataItems;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Maximus.Connectivity.Modules
{
  [MonitoringModule(ModuleType.ReadAction)]
  [ModuleOutput(true)]
  public class DNSResolutionPA : ModuleBaseSimpleAction<PropertyBagDataItem>
  {
    private string FullyQualifiedDomainName, TestDisplayName;
    private int TargetIndex;
    public DNSResolutionPA(ModuleHost<PropertyBagDataItem> moduleHost, XmlReader configuration, byte[] previousState) : base(moduleHost, configuration, previousState)
    {
    }

    protected override void PreInitialize(ModuleHost<PropertyBagDataItem> moduleHost, XmlReader configuration, byte[] previousState)
    {
      ModInit.Logger.AddLoggingSource(GetType(), ModInit.evtId_DNSResolutionPA);
      base.PreInitialize(moduleHost, configuration, previousState);
    }

    protected override PropertyBagDataItem[] GetOutputData(DataItemBase[] inputDataItems)
    {
      try
      {
        Dictionary<string, object> bagItem = new Dictionary<string, object>();
        try
        {
          string IPAddressesText = "";
          IPAddress[] IPAddresses = Dns.GetHostAddresses(FullyQualifiedDomainName);
          foreach (IPAddress ipAddress in IPAddresses)
            IPAddressesText += ipAddress.ToString() + "; ";
          bagItem.Add("State", "OK");
          bagItem.Add("Error", "");
          bagItem.Add("IP", IPAddressesText);
        }
        // catch only name resolution process related exceptions to alert on resolution problem
        catch (SocketException se)
        {
          bagItem.Add("State", "ERROR");
          bagItem.Add("Error", se.Message);
          bagItem.Add("IP", "");
        }
        catch (ArgumentException se)
        {
          bagItem.Add("State", "ERROR");
          bagItem.Add("Error", se.Message);
          bagItem.Add("IP", "");
        }

        return new List<PropertyBagDataItem>() { CreatePropertyBag(bagItem) }.ToArray();
      }
      catch (Exception e)
      {
        ModuleErrorSignalReceiver(ModuleErrorSeverity.DataLoss, ModuleErrorCriticality.ThrowAndContinue, e, $"Failed to perform DNS name resolution test for the {TestDisplayName} test object related to the {FullyQualifiedDomainName}:{TargetIndex} destination object.");
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
        // specific class properties -- none
      }
      catch (Exception e)
      {
        ModuleErrorSignalReceiver(ModuleErrorSeverity.FatalError, ModuleErrorCriticality.Stop, e, "Failed to load module configuration.");
        throw new ModuleException("Failed to load module configuration.", e);
      }
    }
  }
}
