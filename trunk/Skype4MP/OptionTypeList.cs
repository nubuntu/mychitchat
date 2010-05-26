using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;

using MediaPortal.GUI.Library;

namespace maniac.MediaPortal.Skype
{
  class OptionTypeList : Dictionary<string, bool>
  {

    private class OptionInitialiser
    {
      public string name;
      public bool defaultValue;
      public OptionInitialiser(string n, bool b)
      {
        name = n;
        defaultValue = b;
      }
    }

    OptionInitialiser[] initOpts = {
      new OptionInitialiser("StartWithMP", false),
      new OptionInitialiser("StopWithMP", false),
      new OptionInitialiser("HangupIfDND", false),
      new OptionInitialiser("HandleChats", true),
      new OptionInitialiser("HandleVoiceCalls", true),
      new OptionInitialiser("AllowSkypeOutCalls", false),
      new OptionInitialiser("AllowSMS", false),
      new OptionInitialiser("PauseForIncomingCalls", true),
      new OptionInitialiser("PauseForIncomingChats", true),
      new OptionInitialiser("UseSilentMode", true),
      new OptionInitialiser("IgnoreIncomingCalls", false)
    };

    public void LoadList()
    {

      if (File.Exists(SkypeHelper.FileName) == true)
      {
        StreamReader sReader = null;
        try
        {
          XmlDocument doc = new XmlDocument();
          XmlReader reader = null;

          sReader = new StreamReader(SkypeHelper.FileName, Encoding.Unicode);
          reader = new XmlTextReader(sReader.BaseStream);
          doc.Load(reader);

          foreach (OptionInitialiser oi in initOpts)
          {
            this.Add(oi.name, SkypeHelper.LoadNode(doc, oi.name, oi.defaultValue));
          }
        }
        catch (Exception)
        {
          Log.Warn("Unable to load Skype config, check xml file...");
        }
        finally
        {
          sReader.Close();
        }
      }
      else
      {
        LoadDefaults();
      }

    }
    public void LoadDefaults()
    {
      foreach (OptionInitialiser oi in initOpts)
      {
        this.Add(oi.name, oi.defaultValue);
      }
    }

        /// <summary>
    /// Save the current values of the settings to the xml file
    /// </summary>
    public void SaveList()
    {
      StreamWriter writer = null;

      try
      {
        writer = new StreamWriter(SkypeHelper.FileName, false, Encoding.Unicode);

        XmlWriter xmlW = new XmlTextWriter(writer);

        // Build the content of the file from the settings
        ToXml().Save(xmlW);
      }
      catch (Exception e)
      {
        Log.Warn("SP->Config - Unable to save the settings file", e);
      }
      finally
      {
        writer.Close();
      }
      return;
    }

    public XmlDocument ToXml()
    {
      XmlDocument doc = new XmlDocument();
      XmlNode root = null;

      if (Count == 0)
      {
        LoadDefaults();
      }

      root = doc.CreateNode(XmlNodeType.Element, "Skype4MPConfig", null);
      doc.AppendChild(root);

      XmlNode aNode;

      foreach (string ot in Keys)
      {
        aNode = doc.CreateNode(XmlNodeType.Element, ot, null);
        aNode.InnerText = this[ot].ToString();
        doc.DocumentElement.AppendChild(aNode);
      }

      return doc;
    }


  }
}
