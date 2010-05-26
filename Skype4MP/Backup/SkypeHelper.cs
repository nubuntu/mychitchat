using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Win32;

using MediaPortal.Dialogs;
using MediaPortal.GUI.Library;
using MediaPortal.Util;

using SKYPE4COMLib;
using System.Xml;


namespace maniac.MediaPortal.Skype
{
  /// <summary>
  /// Description résumée de SkypeHelper.
  /// </summary>
  public class SkypeHelper
  {
    #region Constants
    public static int MAIN_WINDOW_ID = 1911;
    public static int SKYPEOUT_WINDOW_ID = 1913;
    public static int HISTORY_WINDOW_ID = 1914;
    public static int SMS_WINDOW_ID = 1915;
    public static int CHAT_WINDOW_ID = 1916;
    public static int ACCEPTANCE_WINDOW_ID = 1917;
    public static int SETTINGS_WINDOW_ID = 1918;
    public static int AUDIO_DEVICES_WINDOW_ID = 1919;
    
    public const string PLUGIN_DESCRIPTION = "Skype plugin for Mediaportal";
    public const string PLUGIN_AUTHOR = "TesterBoy (originally Maniac's)";
    public const string PLUGIN_NAME = "Skype4MP";
    public static string FileName = "Skype.xml";

    public const string STATUS_ONLINE = "Online";
    public const string STATUS_OFFLINE = "Offline";
    public const string STATUS_NOTAVAILABLE = "Not Available";
    public const string STATUS_INVISIBLE = "Invisible";
    public const string STATUS_DONOTDISTURB = "Do Not Disturb";
    public const string STATUS_AWAY = "Away";
    public const string STATUS_SKYPEME = "Skype Me";
    public const string STATUS_SKYPEOUT = "SkypeOut";
    public const string STATUS_UNKNOWN = "Unknown";

    public const string SKIN_MEDIA_DIR = @"\Media\Skype\";
    public const string IMG_STATUS_NOTAVAIL = "StatusNotAvailable_128x128.png";
    public const string IMG_STATUS_OFFLINE = "StatusOffline_128x128.png";
    public const string IMG_STATUS_ONLINE = "StatusOnline_128x128.png";
    public const string IMG_STATUS_DONOTDIST = "StatusDoNotDisturb_128x128.png";
    public const string IMG_STATUS_AWAY = "StatusAway_128x128.png";
    public const string IMG_STATUS_SKYPEOUT = "SkypeOutInactive_128x128.png";
    public const string IMG_STATUS_SKYPEME = "StatusSkypeMe_128x128.png";
    public const string IMG_INCOMING_CALL = "CallIn_128x128.png";
    public const string IMG_OUTGOING_CALL = "CallOut_128x128.png";
    public const string IMG_MISSED_CALL = "CallMissed_128x128.png";

    public const string IMG_SKYPE_ICON = "SkypeBlue_128x128.png";

    public const string EMPTY_STRING = " ";
    #endregion

    #region Methods
    public static bool LoadNode(XmlDocument doc, string nodeName, bool def)
    {
      XmlNode tmpNode = null;
      tmpNode = doc.SelectSingleNode("Skype4MPConfig/" + nodeName);
      if (tmpNode != null)
      {
        return Convert.ToBoolean(tmpNode.InnerText);
      }
      return def;

    }


    
    public static int GetStatusItem(TUserStatus SkypeStatus)
    {
      int statusItem = 0;

      //			<subitem>Offline</subitem>
      //      <subitem>Online</subitem>
      //      <subitem>Do not disturb</subitem>
      //      <subitem>Away</subitem>
      //      <subitem>Invisible</subitem>
      //      <subitem>Skype me</subitem>
      //      <subitem>Unavailable</subitem>

      switch (SkypeStatus)
      {
        case TUserStatus.cusOffline:
          statusItem = 0;
          break;

        case TUserStatus.cusOnline:
          statusItem = 1;
          break;

        case TUserStatus.cusDoNotDisturb:
          statusItem = 2;
          break;

        case TUserStatus.cusAway:
          statusItem = 3;
          break;

        case TUserStatus.cusInvisible:
          statusItem = 4;
          break;

        case TUserStatus.cusSkypeMe:
          statusItem = 5;
          break;

        case TUserStatus.cusNotAvailable:
          statusItem = 6;
          break;

        default:
          statusItem = -1;
          break;
      }

      return statusItem;
    }
    public static string GetFriendNameToUse(SKYPE4COMLib.User friend)
    {
      string fDName = string.Empty;
      string fFName = string.Empty;

      if (friend == null) return null;

      // whose name to use
      fDName = friend.DisplayName;
      fFName = friend.FullName;
      if (fDName != string.Empty)
      {
        return fDName;
      }
      else if (fFName != string.Empty)
      {
        return fFName;
      }
      else
      {
        return friend.Handle;
      }
    }
    public static string GetCallStatus(TCallStatus CallProgress)
    {
      string statusString = string.Empty;

      switch (CallProgress)
      {
        case TCallStatus.clsBusy:
          statusString = "Busy";
          break;

        case TCallStatus.clsCancelled:
          statusString = "Cancelled";
          break;

        case TCallStatus.clsEarlyMedia:
          statusString = "Early media";
          break;

        case TCallStatus.clsFailed:
          statusString = "Failed";
          break;

        case TCallStatus.clsFinished:
          statusString = "Finished";
          break;

        case TCallStatus.clsInProgress:
          statusString = "In progress";
          break;

        case TCallStatus.clsLocalHold:
          statusString = "Held locally";
          break;

        case TCallStatus.clsMissed:
          statusString = "Missed";
          break;

        case TCallStatus.clsOnHold:
          statusString = "On hold";
          break;

        case TCallStatus.clsRemoteHold:
          statusString = "Held remotely";
          break;

        case TCallStatus.clsRefused:
          statusString = "Refused";
          break;

        case TCallStatus.clsRinging:
          statusString = "Ringing";
          break;

        case TCallStatus.clsRouting:
          statusString = "Routing";
          break;

        case TCallStatus.clsUnplaced:
          statusString = "Unplaced";
          break;

        default:
          statusString = EMPTY_STRING;
          break;
      }

      return statusString;
    }
    public static string GetCallTypeImage(TCallType CallType)
    {
      string path = GUIGraphicsContext.Skin + SKIN_MEDIA_DIR;

      switch (CallType)
      {
        case TCallType.cltIncomingPSTN:
        case TCallType.cltIncomingP2P:
          path += IMG_INCOMING_CALL;
          break;

        case TCallType.cltOutgoingPSTN:
        case TCallType.cltOutgoingP2P:
          path += IMG_OUTGOING_CALL;
          break;

        default:
          path = string.Empty;
          break;
      }

      return path;
    }
    public static string GetStatusImage(TOnlineStatus SkypeStatus)
    {
      string path = GUIGraphicsContext.Skin + SKIN_MEDIA_DIR;
      switch (SkypeStatus)
      {
        case TOnlineStatus.olsOnline:
          path += IMG_STATUS_ONLINE;
          break;

        case TOnlineStatus.olsOffline:
          path += IMG_STATUS_OFFLINE;
          break;

        case TOnlineStatus.olsNotAvailable:
          path += IMG_STATUS_NOTAVAIL;
          break;

        case TOnlineStatus.olsSkypeOut:
          path += IMG_STATUS_SKYPEOUT;
          break;

        case TOnlineStatus.olsDoNotDisturb:
          path += IMG_STATUS_DONOTDIST;
          break;

        case TOnlineStatus.olsAway:
          path += IMG_STATUS_AWAY;
          break;

        case TOnlineStatus.olsSkypeMe:
          path += IMG_STATUS_SKYPEME;
          break;

        default:
          path += IMG_STATUS_OFFLINE;
          break;
      }

      return path;
    }
    public static string GetSkypeImage()
    {
      return GUIGraphicsContext.Skin + IMG_SKYPE_ICON;
    }
    public static string GetStatusImage(TUserStatus SkypeStatus)
    {
      string path = GUIGraphicsContext.Skin + SKIN_MEDIA_DIR;

      switch (SkypeStatus)
      {
        case TUserStatus.cusOnline:
          path += IMG_STATUS_ONLINE;
          break;

        case TUserStatus.cusOffline:
          path += IMG_STATUS_OFFLINE;
          break;

        case TUserStatus.cusNotAvailable:
          path += IMG_STATUS_NOTAVAIL;
          break;

        case TUserStatus.cusInvisible:
          path += IMG_STATUS_OFFLINE;
          break;

        case TUserStatus.cusDoNotDisturb:
          path += IMG_STATUS_DONOTDIST;
          break;

        case TUserStatus.cusAway:
          path += IMG_STATUS_AWAY;
          break;

        case TUserStatus.cusSkypeMe:
          path += IMG_STATUS_SKYPEME;
          break;

        default:
          path += IMG_STATUS_OFFLINE;
          break;
      }

      return path;
    }
    public static TUserStatus ChangeOnlineStatus(string Status)
    {
      TUserStatus askedStatus = TUserStatus.cusUnknown;

      if (Status == null || Status == string.Empty) return askedStatus;

      if (Status.ToLower() == "away")
      {
        askedStatus = TUserStatus.cusAway;
      }
      else if (Status.ToLower() == "do not disturb")
      {
        askedStatus = TUserStatus.cusDoNotDisturb;
      }
      else if (Status.ToLower() == "invisible")
      {
        askedStatus = TUserStatus.cusInvisible;
      }
      else if (Status.ToLower() == "unavailable")
      {
        askedStatus = TUserStatus.cusNotAvailable;
      }
      else if (Status.ToLower() == "offline")
      {
        askedStatus = TUserStatus.cusOffline;
      }
      else if (Status.ToLower() == "online")
      {
        askedStatus = TUserStatus.cusOnline;
      }
      else if (Status.ToLower() == "skype me")
      {
        askedStatus = TUserStatus.cusSkypeMe;
      }

      return askedStatus;
    }

    public static string GetOnlineStatus(TUserStatus SkypeStatus)
    {
      string status = string.Empty;

      switch (SkypeStatus)
      {
        case TUserStatus.cusOnline:
          status = STATUS_ONLINE;
          break;

        case TUserStatus.cusOffline:
          status = STATUS_OFFLINE;
          break;

        case TUserStatus.cusNotAvailable:
          status = STATUS_NOTAVAILABLE;
          break;

        case TUserStatus.cusInvisible:
          status = STATUS_INVISIBLE;
          break;

        case TUserStatus.cusDoNotDisturb:
          status = STATUS_DONOTDISTURB;
          break;

        case TUserStatus.cusAway:
          status = STATUS_AWAY;
          break;

        case TUserStatus.cusSkypeMe:
          status = STATUS_SKYPEME;
          break;

        default:
          status = STATUS_UNKNOWN;
          break;
      }

      return status;
    }

    public static string GetOnlineStatus(TOnlineStatus SkypeStatus)
    {
      string status = string.Empty;

      switch (SkypeStatus)
      {
        case TOnlineStatus.olsOnline:
          status = STATUS_ONLINE;
          break;

        case TOnlineStatus.olsOffline:
          status = STATUS_OFFLINE;
          break;

        case TOnlineStatus.olsNotAvailable:
          status = STATUS_NOTAVAILABLE;
          break;

        case TOnlineStatus.olsSkypeOut:
          status = STATUS_SKYPEOUT;
          break;

        case TOnlineStatus.olsDoNotDisturb:
          status = STATUS_DONOTDISTURB;
          break;

        case TOnlineStatus.olsAway:
          status = STATUS_AWAY;
          break;

        case TOnlineStatus.olsSkypeMe:
          status = STATUS_SKYPEME;
          break;

        default:
          status = STATUS_UNKNOWN;
          break;
      }

      return status;
    }

    public static string GetConnectionStatus(TConnectionStatus Status)
    {
      string status = string.Empty;

      switch (Status)
      {
        case TConnectionStatus.conConnecting:
          status = "Connecting";
          break;

        case TConnectionStatus.conOffline:
          status = "Offline";
          break;

        case TConnectionStatus.conOnline:
          status = "Online";
          break;

        case TConnectionStatus.conPausing:
          status = "Pausing";
          break;

        case TConnectionStatus.conUnknown:
          status = "Unknown";
          break;
      }
      return status;
    }
    public static long GetColorFromOnlineStatus(TOnlineStatus SkypeStatus)
    {
      long statusColor = 0xFFFFFFFF;

      switch (SkypeStatus)
      {
        case TOnlineStatus.olsOnline:
          statusColor = 0xFF76F471;
          break;

        case TOnlineStatus.olsOffline:
          statusColor = 0xFFFF1010;
          break;

        case TOnlineStatus.olsNotAvailable:
          statusColor = 0xFFFF1010;
          break;

        case TOnlineStatus.olsSkypeOut:
          statusColor = 0xFFFF1010;
          break;

        case TOnlineStatus.olsDoNotDisturb:
          statusColor = 0xFFFF1010;
          break;

        case TOnlineStatus.olsAway:
          statusColor = 0xFFFF1010;
          break;

        case TOnlineStatus.olsSkypeMe:
          statusColor = 0xFF76F471;
          break;

        default:
          break;
      }

      return statusColor;
    }

    #endregion
  }
}
