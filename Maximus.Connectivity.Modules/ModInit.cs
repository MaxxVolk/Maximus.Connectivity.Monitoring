﻿using Maximus.Library.Helpers;
using Maximus.Library.ManagedModuleBase;

using Microsoft.EnterpriseManagement.HealthService;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Maximus.Connectivity.Modules
{
  internal static class ModInit
  {
    static ModInit()
    {
      ServicePointManager.CheckCertificateRevocationList = false;
      ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true; // ignore certificates
    }

    internal static char[] Separators = { ',', ';', '|' };

    const string LogSourceName = "Maximus Connectivity Monitoring Module";
    const int LogBaseEventId = 2860;

    static private LoggingHelper _Logger;
    static internal LoggingHelper Logger => _Logger ?? (_Logger = new LoggingHelper(LogSourceName, LogBaseEventId, EventLoggingLevel.Informational));

    internal const int evtId_PingPA                 = 0;
    internal const int evtId_ServerCertificatePA    = 1;
    internal const int evtId_DNSResolutionPA        = 2;
    internal const int evtId_HTTPProbePA            = 3;
    internal const int evtId_TCPConnectPA           = 4;
    //internal const int evtId_ = 5;
    //internal const int evtId_ = 6;

    internal static void ModuleErrorSignalReceiver(ModuleErrorSeverity severity, ModuleErrorCriticality criticality, Exception e, string message, object callerInstance)
    {
      Logger.WriteException($"Internal module exception or error.\r\nMessage: {message}\r\nError Severity: {severity}\r\nError Criticality: {criticality}", e ?? new Exception("Unknown exception"), callerInstance);
    }
  }
}
