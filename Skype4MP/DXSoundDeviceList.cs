/*
 * 
public class wfEnum : System.Windows.Forms.Form
private DevicesCollection myDevices = null;
private struct myDeviceDescription
{
    public DeviceInformation info;
    public override string ToString()
    {
        return info.Description;
    }
        public myDeviceDescription(DeviceInformation di)
        {
            info = di;
        }
}
public wfEnum()
{
    // Retrieve the available DirectSound devices
    myDevices = new DevicesCollection();
    
    foreach (DeviceInformation dev in myDevices)
    {
        myDeviceDescription dd = new myDeviceDescription(dev);
        
        // Use DevicesCollection and DeviceInformation to query for devices.
    }
*/

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX.DirectSound;
using MediaPortal.GUI.Library;

namespace maniac.MediaPortal.Skype
{
  class DXSoundDeviceList
  {
    public DevicesCollection myOutDevices = null;
    public CaptureDevicesCollection myInDevices = null;
    private struct myDeviceDescription
    {
      public DeviceInformation info;
      public override string ToString()
      {
        return info.Description;
      }
      public myDeviceDescription(DeviceInformation di)
      {
        info = di;
      }
    }

    public DXSoundDeviceList()
    {
      myOutDevices = new DevicesCollection();
      myInDevices = new CaptureDevicesCollection();
    }

    public void DXAudioOutList()
    {
      // Retrieve the available DirectSound devices
      foreach (DeviceInformation dev in myOutDevices)
      {
        myDeviceDescription dd = new myDeviceDescription(dev);
        
          // Use DevicesCollection and DeviceInformation to query for devices.
        Log.Debug("SP->AudioOut: '{0}'", dd.ToString());
      }
    }

    public void DXAudioInList()
    {
      // Retrieve the available DirectSound devices
      foreach (DeviceInformation dev in myInDevices)
      {
        myDeviceDescription dd = new myDeviceDescription(dev);
        
          // Use DevicesCollection and DeviceInformation to query for devices.
        Log.Debug("SP->AudioIn: '{0}'", dd.ToString());
      }
    }
  }
}
