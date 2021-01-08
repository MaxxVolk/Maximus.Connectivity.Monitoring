using Maximus.Library.ManagedModuleBase;

using Microsoft.EnterpriseManagement.HealthService;
using Microsoft.EnterpriseManagement.Mom.Modules.DataItems;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Maximus.Connectivity.Modules
{
  [MonitoringModule(ModuleType.ReadAction)]
  [ModuleOutput(true)]
  public class PingPA : ModuleBaseSimpleAction<PropertyBagDataItem>
  {
    private int MaxTTL, BufferSize, Timeout, TargetIndex;
    private bool DontFragment;
    private string FullyQualifiedDomainName, TestDisplayName;

    public PingPA(ModuleHost<PropertyBagDataItem> moduleHost, XmlReader configuration, byte[] previousState) : base(moduleHost, configuration, previousState)
    {
    }

    protected override void PreInitialize(ModuleHost<PropertyBagDataItem> moduleHost, XmlReader configuration, byte[] previousState)
    {
      ModInit.Logger.AddLoggingSource(GetType(), ModInit.evtId_PingPA);
      base.PreInitialize(moduleHost, configuration, previousState);
    }

    protected override PropertyBagDataItem[] GetOutputData(DataItemBase[] inputDataItems)
    {
      try
      {
        // ModInit.Logger.WriteWarning($"Entering Ping Probe Action -- test cookdown.  {MaxTTL}, {BufferSize}, {Timeout}, {TargetIndex}, {DontFragment}, {FullyQualifiedDomainName}, {TestDisplayName}", this);

        PingOptions pingOptions = new PingOptions() { DontFragment = DontFragment };
        int bufferSize = BufferSize <= 0 ? 32 : BufferSize;
        int timeout = Timeout <= 0 ? 5000 : Timeout;
        byte[] pingData = Encoding.ASCII.GetBytes(new string('B', bufferSize));
        if (MaxTTL > 0)
          pingOptions.Ttl = MaxTTL;
        using (Ping pingTest = new Ping())
        {
          PingReply pingReply = pingTest.Send(FullyQualifiedDomainName, timeout, pingData, pingOptions);
          Dictionary<string, object> bagItem = new Dictionary<string, object>();
          if (pingReply.Status == IPStatus.Success)
          {
            bagItem.Add("State", "OK");
            bagItem.Add("Error", pingReply.Status.ToString());
            bagItem.Add("Address", pingReply.Address?.ToString());
            bagItem.Add("RoundTripTime", pingReply.RoundtripTime);
          }
          else
          {
            bagItem.Add("State", "ERROR");
            bagItem.Add("Error", pingReply?.Status.ToString() ?? "Unknown");
            bagItem.Add("Address", pingReply?.Address?.ToString() ?? "NULL");
            bagItem.Add("RoundTripTime", -1);
          }

          return new List<PropertyBagDataItem>() { CreatePropertyBag(bagItem) }.ToArray();
        }
      }
      catch (PingException pe)
      {
        string errorMsg = $"Exception message: {pe.Message}";
        if (pe.InnerException != null)
          errorMsg += $"; Internal exception message: {pe.InnerException.Message}";
        ModInit.Logger.WriteInformation($"Unable to perform ping test for the {TestDisplayName} test object related to the {FullyQualifiedDomainName}:{TargetIndex} destination object.\r\nError message: {errorMsg}", this);

        return null;
      }
      catch (Exception e)
      {
        ModuleErrorSignalReceiver(ModuleErrorSeverity.DataLoss, ModuleErrorCriticality.ThrowAndContinue, e, $"Failed to perform ping test for the {TestDisplayName} test object related to the {FullyQualifiedDomainName}:{TargetIndex} destination object.");
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
        LoadConfigurationElement(cfgDoc, "MaxTTL", out MaxTTL, -1, false);
        LoadConfigurationElement(cfgDoc, "BufferSize", out BufferSize, -1, false);
        LoadConfigurationElement(cfgDoc, "Timeout", out Timeout, -1, false);
        LoadConfigurationElement(cfgDoc, "DontFragment", out DontFragment, false, false);
      }
      catch (Exception e)
      {
        ModuleErrorSignalReceiver(ModuleErrorSeverity.FatalError, ModuleErrorCriticality.Stop, e, "Failed to load module configuration.");
        throw new ModuleException("Failed to load module configuration.", e);
      }
    }
  }
}
