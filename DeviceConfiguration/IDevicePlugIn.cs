using System;
using IPA.DAL.RBADAL.Helpers;

namespace IPA.DAL.RBADAL
{
  public interface IDevicePlugIn
  {
    // Device Events back to Main Form
    event EventHandler<DeviceNotificationEventArgs> OnDeviceNotification;

    // INITIALIZATION
    string PluginName { get; }
    // DISCOVERY
    bool FindIngenicoDevice(ref string description, ref string deviceID);
    bool IdentifyUIADevice();
    void DeviceInit();
    // GUI UPDATE
    string [] GetConfig();
    // NOTIFICATION
    void SetFormClosing(bool state);
    // Firmare update
    void UpdateUIAFirmware();
    void UpdateRBAFirmware(int version);
  }
}
