#region Copyright (C) 2005-2007 Team MediaPortal

/* 
 *	Copyright (C) 2005-2007 Team MediaPortal
 *	http://www.team-mediaportal.com
 *
 *  This Program is free software; you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation; either version 2, or (at your option)
 *  any later version.
 *   
 *  This Program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 *  GNU General Public License for more details.
 *   
 *  You should have received a copy of the GNU General Public License
 *  along with GNU Make; see the file COPYING.  If not, write to
 *  the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA. 
 *  http://www.gnu.org/copyleft/gpl.html
 *
 */

#endregion

using System;
using System.IO;
using MediaPortal.GUI.Library;
using MediaPortal.Player;
using MediaPortal.Dialogs;


using SKYPE4COMLib;
using System.Threading;


namespace maniac.MediaPortal.Skype
{
    /// <summary>
    /// 
    /// </summary>
    public class GUISkypeSMS : GUIWindow
    {
        enum Controls : int
        {
            Status = 2,
            Input = 51
        }

        public GUISkypeSMS()
        {
            GetID = (int)SkypeHelper.SMS_WINDOW_ID;
        }

        public override bool Init()
        {
            return Load(GUIGraphicsContext.Skin + @"\mySkypeSMS.xml");
        }
        public override void OnAction(Action action)
        {
            if (action.wID == Action.ActionType.ACTION_PREVIOUS_MENU)
            {
                //GUIMessage msg= new GUIMessage (GUIMessage.MessageType.GUI_MSG_MSN_CLOSECONVERSATION, (int)GUIWindow.Window.WINDOW_MSN, GetID, 0,0,0,null );
                //msg.SendToTargetWindow = true;
                //GUIGraphicsContext.SendMessage(msg);

                //// GUIMSNPlugin.CloseConversation();

                GUIWindowManager.ShowPreviousWindow();
                return;
            }

            if (action.wID == Action.ActionType.ACTION_KEY_PRESSED)
            {
                // Check focus on sms input control
                if (GetFocusControlId() != (int)Controls.Input)
                {
                    // set focus to the default control then
                    GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_SETFOCUS, GetID, 0, (int)Controls.Input, 0, 0, null);
                    OnMessage(msg);
                }
            }
            else
            {
                // translate all other actions from regular keypresses back to keypresses
                if (action.m_key != null && action.m_key.KeyChar >= 32)
                {
                    action.wID = Action.ActionType.ACTION_KEY_PRESSED;
                }
            }

            base.OnAction(action);
        }


        public override bool OnMessage(GUIMessage message)
        {
            switch (message.Message)
            {
                case GUIMessage.MessageType.GUI_MSG_WINDOW_INIT:
                    base.OnMessage(message);
                    GUIWindowManager.RouteToWindow(GetID);
                    return true;

                case GUIMessage.MessageType.GUI_MSG_WINDOW_DEINIT:
                    GUIWindowManager.UnRoute();
                    break;

                case GUIMessage.MessageType.GUI_MSG_CLICKED:
                    int iControl = message.SenderControlId;
                    break;

                case GUIMessage.MessageType.GUI_MSG_NEW_LINE_ENTERED:
                    // This is the easy way:
                    //GUISkype.SkypeAccess.SendSms("+64211494527", "Testing once more", string.Empty);

                    // This is the hard way...
                    try
                    {
                        String phone = GUISkype.CurrentCallTarget.PhoneMobile.Replace(" ", "");
                        if (phone.Equals(""))
                        {
                            // Must be SkypeOut
                            phone = GUISkype.CurrentCallTarget.Handle.Replace(" ", "");
                            if (phone.Equals(""))
                            {
                                Log.Info("SkypePlugin->Not a valid target for SMS");
                                GUIWindowManager.ShowPreviousWindow();
                            }
                        }
                        Log.Info("SkypeSMS->Sending SMS to " + phone + "(" + GUISkype.CurrentCallTarget.PhoneMobile + ")");
                        SmsMessage sms = GUISkype.SkypeAccess.CreateSms(TSmsMessageType.smsMessageTypeOutgoing, phone);
                        sms.Body = message.Label;
                        //while (sms.Price < 0)
                        //{
                        //    Thread.Sleep(500);
                        //}
                        if (ConfirmSend(sms))
                        {
                            Log.Info("SkypeSMS->Sent SMS for {0}{1}", sms.PriceCurrency, ((Double)sms.Price / Math.Pow(10, sms.PricePrecision)).ToString("0.000"));
                            sms.Send();
                        }
                        else
                        {
                            Log.Info("SkypeSMS->Didn't send SMS");
                            break;
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Info("SkypeSMS->Error creating SMS Message:");
                        Log.Info(e.Message);
                        Log.Info(e.StackTrace);
                    }
                    GUIWindowManager.ShowPreviousWindow();
                    break;
            }
            return base.OnMessage(message);
        }

        private bool ConfirmSend(SmsMessage sms)
        {
            Thread.Sleep(500);
            bool stillWaiting = true;
            while (stillWaiting)
            {
                stillWaiting = false;
                foreach (SmsTarget tgt in sms.Targets)
                {
                    if (tgt.Status == TSmsTargetStatus.smsTargetStatusAnalyzing)
                    {
                        Thread.Sleep(500);
                        stillWaiting = true;
                    }
                    else if (tgt.Status != TSmsTargetStatus.smsTargetStatusAcceptable)
                    {
                        GUIDialogOK okDlg = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
                        okDlg.SetLine(1, "Phone number " + tgt.Number + " cannot receive text messages");
                        okDlg.DoModal(GetID);
                        return false;
                    }
                    else
                    {
                        Log.Info("SkypePlugin->TargetStatus {0}", tgt.Status);
                    }
                }
            }
            // init question dialog
            GUIDialogYesNo m_acceptanceWindow = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
            m_acceptanceWindow.SetHeading("Confirm SMS Send");
            m_acceptanceWindow.SetLine(1, "Your SMS " +
                " to " + sms.TargetNumbers + " will cost " +
                ((Double)sms.Price / Math.Pow(10, sms.PricePrecision)).ToString("0.000") +
                " " + sms.PriceCurrency);
            m_acceptanceWindow.SetLine(2, "Do you want to send it?");
            m_acceptanceWindow.DoModal(GetID);

            return m_acceptanceWindow.IsConfirmed;
        }

    }
}
