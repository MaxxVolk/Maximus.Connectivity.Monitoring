using Microsoft.EnterpriseManagement.Configuration;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maximus.Connectivity.Modules
{
  class TCPConnectResult : PropertyBagObject
  {
    private HealthState _healthState = Microsoft.EnterpriseManagement.Configuration.HealthState.Uninitialized;
    private DateTime _startTime = DateTime.UtcNow;
    public string HealthState => _healthState.ToString().ToUpperInvariant();
    public string Message { get; set; }
    public string ReceivedData { get; set; }
    public bool ConnectSuccess { get; set; } = false;
    public bool SendSuccess { get; set; } = false;
    public bool ReceivedSuccess { get; set; } = false;
    public bool RegExMatch { get; set; } = false;
    public double TimeTakenMs => DateTime.UtcNow.Subtract(_startTime).TotalMilliseconds;

    public void SetHealth(HealthState healthState )
    {
      _healthState = healthState;
    }
  }
}
