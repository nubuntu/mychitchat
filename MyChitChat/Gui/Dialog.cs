using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaPortal.Dialogs;
using MediaPortal.GUI.Library;
using nJim;
using MyChitChat.Plugin;

namespace MyChitChat.Gui {
    public sealed class Dialog {
        static readonly Dialog _instance = new Dialog();

        private GUIDialogMenu _dlgSelect;
        private IEnumerable<GUIListItem> _lstSelectStatus;
        private IEnumerable<GUIListItem> _lstSelectMood;
        private IEnumerable<GUIListItem> _lstSelectActivity;

        private Dialog() {
            _dlgSelect = new GUIDialogMenu();
            _lstSelectStatus = BuildDialogSelectList(typeof(Enums.StatusType));
            _lstSelectMood = BuildDialogSelectList(typeof(Enums.MoodType));
            _lstSelectActivity = BuildDialogSelectList(typeof(Enums.ActivityType));
        }

        public static Dialog Instance {
            get {
                return _instance;
            }
        }

        public enum ContextMenuButtons {
            BtnSelectStatus,
            BtnSelectActivity,
            BtnSelectMood,
            BtnSendNewMessage,
            BtnFilterOnline,
            BtnFilterOffline,
            BtnFilterNone,
            BtnJabberReconnect,
            BtnJabberDisconnect,
            BtnSelectKeyboard,
            NothingSelected
        }

        public enum KeyBoardTypes {
            Normal,
            Web,
            SMS
        }

        public ContextMenuButtons ShowContextMenu(List<ContextMenuButtons> buttonCollection) {
            List<GUIListItem> tmpItemList = new List<GUIListItem>();
            foreach (ContextMenuButtons currentButton in buttonCollection) {
                tmpItemList.Add(new GUIListItem(Translations.GetByName(currentButton.ToString())));
            }
            try {
                return buttonCollection[ShowDialogSelect(tmpItemList, false).selectedIndex];
            } catch {
                return ContextMenuButtons.NothingSelected;
            }
        }

        private GUIDialogMenu BuildDialogSelect(IEnumerable<GUIListItem> list) {
            _dlgSelect = GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU) as GUIDialogMenu;
            _dlgSelect.ResetAllControls();
            _dlgSelect.Reset();
            _dlgSelect.Init();
            _dlgSelect.InitControls();
            _dlgSelect.Process();
            _dlgSelect.SetHeading("I'm currently ...");
            foreach (GUIListItem item in list) {
                _dlgSelect.Add(item);
            }
            return _dlgSelect;
        }

        private IEnumerable<GUIListItem> BuildDialogSelectList(Type enumType) {
            foreach (string type in Enum.GetNames(enumType)) {
                string iconPath = Helper.MEDIA_ICON_DEFAULT;
                if (enumType == typeof(Enums.StatusType)) {
                    iconPath = Helper.GetStatusIcon(type);
                } else if (enumType  == typeof(Enums.MoodType)) {
                    iconPath = Helper.GetMoodIcon(type);
                } else if (enumType  == typeof(Enums.ActivityType)) {
                    iconPath = Helper.GetActivityIcon(type);
                }
                yield return Helper.CreateGuiListItem(type,
                        Translations.GetByName(type.ToString()),
                        String.Empty,
                        String.Empty,
                        iconPath,
                        type == Enums.StatusType.Invisible.ToString(),
                        null,
                        false
                        );
            }
        }

        public DialogResult ShowDialogSelect(IEnumerable<GUIListItem> listLabels, bool addCustomButton) {
            _dlgSelect = BuildDialogSelect(listLabels);
            GUIListItem customButton = new GUIListItem("Set Custom Status...");
            if (addCustomButton) {
                _dlgSelect.Add(customButton);
            }
            _dlgSelect.DoModal(GUIWindowManager.ActiveWindow);

            DialogResult result = new DialogResult(_dlgSelect.SelectedLabel, _dlgSelect.SelectedLabelText, _dlgSelect.SelectedLabelText);
            _dlgSelect.ResetAllControls();
            _dlgSelect.Reset();
            if (result.selectedLabelText == "Set Custom Status...") {
                result = ShowDialogSelect(listLabels, false);
                result.message = GetKeyBoardInput(result.selectedLabelText, Helper.CurrentKeyboardType);
            }
            return result;
        }

        public void SelectAndSetKeyboardType() {
            DialogResult tmpResult = ShowDialogSelect(BuildDialogSelectList(typeof(Dialog.KeyBoardTypes)), false);
            Helper.CurrentKeyboardType = (Dialog.KeyBoardTypes)tmpResult.selectedIndex;
        }

        public void SelectAndSetStatus() {
            DialogResult tmpResult = ShowDialogSelect(_lstSelectStatus, true);
            if (tmpResult.selectedIndex > -1) {
                Helper.SetStatus((Enums.StatusType)tmpResult.selectedIndex, tmpResult.message);
            }
        }

        public void SelectAndSetMood() {
            DialogResult tmpResult = ShowDialogSelect(_lstSelectMood, true);
            if (tmpResult.selectedIndex > -1) {
                Helper.SetMood((Enums.MoodType)tmpResult.selectedIndex, tmpResult.message);
            }
        }

        public void SelectAndSetActivity() {
            DialogResult tmpResult = ShowDialogSelect(_lstSelectActivity, true);
            if (tmpResult.selectedIndex > -1) {
                Helper.JABBER_CLIENT.Presence.setActivity((Enums.ActivityType)tmpResult.selectedIndex, tmpResult.message);
            }
        }

        /// <summary>
        /// Displays a yes/no dialog with custom labels for the buttons
        /// This method may become obsolete in the future if media portal adds more dialogs
        /// </summary>
        /// <returns>True if yes was clicked, False if no was clicked</returns>
        /// This has been taken (stolen really) from the wonderful MovingPictures Plugin -Anthrax.
        public bool ShowCustomYesNo(int parentWindowID, string heading, string lines, string yesLabel, string noLabel, bool defaultYes) {
            GUIDialogYesNo dialog = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
            try {
                dialog.Reset();
                dialog.SetHeading(heading);
                string[] linesArray = lines.Split(new string[] { "\\n" }, StringSplitOptions.None);
                if (linesArray.Length > 0)
                    dialog.SetLine(1, linesArray[0]);
                if (linesArray.Length > 1)
                    dialog.SetLine(2, linesArray[1]);
                if (linesArray.Length > 2)
                    dialog.SetLine(3, linesArray[2]);
                if (linesArray.Length > 3)
                    dialog.SetLine(4, linesArray[3]);
                dialog.SetDefaultToYes(defaultYes);

                foreach (System.Windows.UIElement item in dialog.Children) {
                    if (item is GUIButtonControl) {
                        GUIButtonControl btn = (GUIButtonControl)item;
                        if (btn.GetID == 11 && !String.IsNullOrEmpty(yesLabel)) // Yes button
                            btn.Label = yesLabel;
                        else if (btn.GetID == 10 && !String.IsNullOrEmpty(noLabel)) // No button
                            btn.Label = noLabel;
                    }
                }
                dialog.DoModal(parentWindowID);
                return dialog.IsConfirmed;
            } finally {
                // set the standard yes/no dialog back to it's original state (yes/no buttons)
                if (dialog != null) {
                    dialog.ClearAll();
                }
            }
        }
        public void ShowNotifyDialog(string header, string icon, string text, Helper.PLUGIN_NOTIFY_WINDOWS notifyType) {
            ShowNotifyDialog(Settings.notifyTimeOut, header, icon, text, notifyType);
        }
        public void ShowNotifyDialog(int timeOut, string header, string icon, string text, Helper.PLUGIN_NOTIFY_WINDOWS notifyType) {
            try {
                GUIWindow guiWindow = GUIWindowManager.GetWindow((int)notifyType);
                switch (notifyType) {
                    default:
                    case Helper.PLUGIN_NOTIFY_WINDOWS.WINDOW_DIALOG_AUTO:
                        if (text.Length <= 60) {
                            ShowNotifyDialog(timeOut, header, icon, text, Helper.PLUGIN_NOTIFY_WINDOWS.WINDOW_DIALOG_NOTIFY);
                        } else {
                            ShowNotifyDialog(timeOut, header, icon, text, Helper.PLUGIN_NOTIFY_WINDOWS.WINDOW_DIALOG_TEXT);
                        }
                        break;
                    case Helper.PLUGIN_NOTIFY_WINDOWS.WINDOW_DIALOG_NOTIFY:
                        GUIDialogNotify notifyDialog = (GUIDialogNotify)guiWindow;
                        notifyDialog.Reset();
                        notifyDialog.TimeOut = timeOut;
                        notifyDialog.SetImage(icon);
                        notifyDialog.SetHeading(header);
                        notifyDialog.SetText(text);
                        notifyDialog.DoModal(GUIWindowManager.ActiveWindow);
                        break;
                    case Helper.PLUGIN_NOTIFY_WINDOWS.WINDOW_DIALOG_OK:
                        GUIDialogOK okDialog = (GUIDialogOK)guiWindow;
                        okDialog.Reset();
                        okDialog.SetHeading(header);
                        okDialog.SetLine(1, (text.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries))[0]);
                        okDialog.DoModal(GUIWindowManager.ActiveWindow);
                        break;
                    case Helper.PLUGIN_NOTIFY_WINDOWS.WINDOW_DIALOG_TEXT:
                        GUIDialogText textDialog = (GUIDialogText)guiWindow;
                        textDialog.Reset();
                        try {
                            textDialog.SetImage(icon);
                        } catch { }
                        textDialog.SetHeading(header);
                        textDialog.SetText(text);
                        textDialog.DoModal(GUIWindowManager.ActiveWindow);
                        break;
                }
            } catch (Exception ex) {
                Log.Error(ex);
            }
        }
        public void ShowNotifyDialog(string text) {
            ShowNotifyDialog(Settings.notifyTimeOut, Helper.PLUGIN_NAME, Helper.MEDIA_ICON_DEFAULT, text, Helper.PLUGIN_NOTIFY_WINDOWS.WINDOW_DIALOG_NOTIFY);
        }

        public void ShowErrorDialog(nJim.Enums.ErrorType type, string message) {
            Dialog.Instance.ShowNotifyDialog(3 * Settings.notifyTimeOut,
                Helper.PLUGIN_NAME + " Error!",
                Helper.MEDIA_ICON_ERROR,
                type.ToString() + "\n" + message,
                Helper.PLUGIN_NOTIFY_WINDOWS.WINDOW_DIALOG_NOTIFY
            );
        }

        public string GetKeyBoardInput(string defaultText, KeyBoardTypes keyboardType) {
            VirtualKeyboard keyboard;
            switch (keyboardType) {
                case KeyBoardTypes.SMS:
                    keyboard = (VirtualKeyboard)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_VIRTUAL_SMS_KEYBOARD);
                    break;
                case KeyBoardTypes.Web:
                    keyboard = (VirtualKeyboard)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_VIRTUAL_WEB_KEYBOARD);
                    break;
                default:
                case KeyBoardTypes.Normal:
                    keyboard = (VirtualKeyboard)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_VIRTUAL_KEYBOARD);
                    break;
            }
            if (keyboard == null) {
                return String.Empty;
            }
            keyboard.Reset();
            keyboard.IsSearchKeyboard = true;
            keyboard.Text = defaultText;
            keyboard.DoModal(GUIWindowManager.ActiveWindow);
            if (keyboard.IsConfirmed) {
                return keyboard.Text;
            } else {
                return String.Empty;
            }
        }
        public string GetKeyBoardInput() {
            return GetKeyBoardInput(String.Empty, Helper.CurrentKeyboardType);
        }
    }

    public struct DialogResult {
        internal int selectedIndex;
        internal string selectedLabelText;
        internal string message;

        public DialogResult(int index, string text, string message) {
            this.selectedIndex = index;
            this.selectedLabelText = text;
            this.message = message;
        }
    }





}
