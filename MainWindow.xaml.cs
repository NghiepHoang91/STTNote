using STTNote.ActionMessages;
using STTNote.Const;
using STTNote.Enums;
using STTNote.Extensions;
using STTNote.Helpers;
using STTNote.Models;
using STTNote.Services;
using STTNote.Views;
using STTNote.Wrappers;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using MessageBox = System.Windows.Forms.MessageBox;
using ModifierKeys = NonInvasiveKeyboardHookLibrary.ModifierKeys;

namespace STTNote
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : BaseWindow
    {
        private NotifyIcon notifyIcon;
        private string? _selectedProfileId;
        private string? _selectedNoteId;

        public ObservableCollection<Note> AllNotes { get; set; }
        public ObservableCollection<Note> ProfileNotes { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            ApplyApplicationConfigs();

            SetDefaultSize();
            SetDefaultLocation();
            InitReferrenceMessage();
            InitVariables();
            LoadExistingNotes();
            InitSystemTrayIcon();
            InitHotKeys();

            InitSelectedProfile();
        }

        private void InitSelectedProfile()
        {
            windowHeader.SetSelectedProfileValue();
        }

        private void ApplyApplicationConfigs()
        {
            ApplicationHelper.CreateShortcutInStartupFolder();
        }

        private void SetDefaultSize()
        {
            Width = Consts.GLOBAL_FORM_WIDTH;
            Height = Consts.GLOBAL_FORM_HEIGHT;
        }

        private void SetDefaultLocation()
        {
            var taskbarHeight = Screen.PrimaryScreen.Bounds.Bottom - Screen.PrimaryScreen.WorkingArea.Bottom;

            WindowStartupLocation = WindowStartupLocation.Manual;
            Left = SystemParameters.PrimaryScreenWidth - this.Width;
            Top = SystemParameters.PrimaryScreenHeight - this.Height - taskbarHeight;
        }

        private void InitReferrenceMessage()
        {
            ReferrencesHelper.RegisterAction<InsertNoteMessage>(InsertNote);
            ReferrencesHelper.RegisterAction<CloseFormMessage>(CloseForm);
            ReferrencesHelper.RegisterAction<SaveNoteMessage>(SaveContent);
            ReferrencesHelper.RegisterAction<SaveAppSettingMessage>(SaveAppSetting);
            ReferrencesHelper.RegisterAction<SaveSelectedProfileMessage>(SaveSelectedProfile);
            ReferrencesHelper.RegisterAction<ApplySelectProfileMessage>(SelectProfile);
        }

        private void InitSystemTrayIcon()
        {
            notifyIcon = new NotifyIcon();
            notifyIcon.Icon = new System.Drawing.Icon("Note.ico");
            notifyIcon.Visible = true;
            notifyIcon.DoubleClick += SystemTrayIconDoubleClick;
            notifyIcon.ContextMenuStrip = new ContextMenuStrip();
            notifyIcon.ContextMenuStrip.Items.Add("Add New", ResourceHelper.GetImage("Add_16.png"), SystemTrayAddNewClick);
            notifyIcon.ContextMenuStrip.Items.Add("Exit", ResourceHelper.GetImage("Close_16.png"), SystemTrayExitClick);
        }

        private void SystemTrayIconDoubleClick(object sender, EventArgs args)
        {
            if (this.IsVisible)
            {
                return;
            }

            if (!ApplicationHelper.AskForAppPassword())
            {
                return;
            }

            ShowMainWindow();
        }

        private void ShowMainWindow()
        {
            ApplicationHelper.RunAsMainThread(() =>
            {
                this.Show();
            });
        }

        private void SystemTrayExitClick(object sender, EventArgs args)
        {
            notifyIcon.Visible = false;
            ApplicationHelper.ForceExitApp();
        }

        private void SystemTrayAddNewClick(object sender, EventArgs args)
        {
            InsertNewNote();
        }

        private void InsertNewNote()
        {
            ApplicationHelper.RunAsMainThread(() =>
            {
                if (System.Windows.Forms.Control.ModifierKeys == Keys.Control)
                {
                    this.Hide();
                }

                NoteEditor noteEditor = new NoteEditor();
                noteEditor.Mode = Enums.EditingMode.AddNew;
                noteEditor.Show();
            });
        }

        private void InitVariables()
        {
            ProfileNotes = new ObservableCollection<Note>();
            AllNotes = new ObservableCollection<Note>();
        }

        private void InitHotKeys()
        {
            KeyboardHelper.RegisterKey(InsertNewNote, Keys.N, ModifierKeys.Control, ModifierKeys.Alt);
        }

        private void LoadExistingNotes()
        {
            var loadedNotes = DatabaseService.Instance.GetAll<Note>();
            if (loadedNotes?.ReturnValue?.Count > 0)
            {
                if (AllNotes != null) AllNotes.Clear();
                loadedNotes.ReturnValue.ForEach(note => { AllNotes.Add(note); });
            }
        }

        private void PopulateNoteListToUI()
        {
            if (AllNotes != null)
            {
                var noteIds = NoteProfileWrapper.Instance.GetNoteIdsByProfile(_selectedProfileId).ReturnValue;
                ProfileNotes = new ObservableCollection<Note>();

                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    AllNotes.Where(n => noteIds.Contains(n.Id))
                    .ToList()
                    .ForEach(n => ProfileNotes.Add(n));
                });

                ApplicationHelper.RunAsMainThread(() =>
                {
                    icNoteList.ItemsSource = null;
                    icNoteList.ItemsSource = ProfileNotes;
                });
                ProfileNotes.Refresh();
            }
        }

        public void DuplicatedNote(object sender, RoutedEventArgs e)
        {
            ApplicationHelper.RunAsMainThread(() =>
            {
                var image = sender as System.Windows.Controls.Image;
                var noteId = image?.Tag?.ToString() ?? string.Empty;

                if (!string.IsNullOrEmpty(noteId))
                {
                    var note = ProfileNotes.FirstOrDefault(n => n.Id.Equals(noteId));
                    if (note != null)
                    {
                        var newNote = new Note
                        {
                            Id = Guid.NewGuid().ToString(),
                            Content = note.Content,
                            Title = note.Title,
                            Password = note.Password,
                            NoteStatusCode = note.NoteStatusCode,
                            CreateDate = note.CreateDate,
                        };

                        var actionResult = DatabaseService.Instance.Insert(newNote);
                        if (actionResult.IsSuccess)
                        {
                            ProfileNotes.Add(newNote);
                            AllNotes.Add(newNote);

                            NoteProfileWrapper.Instance.Insert(new NoteProfile
                            {
                                Id = Guid.NewGuid().ToString(),
                                NoteId = newNote.Id,
                                ProfileId = _selectedProfileId
                            });

                            if (ProfileNotes.Count == 1)
                            {
                                PopulateNoteListToUI();
                            }
                            else
                            {
                                ProfileNotes.Refresh();
                            }
                        }
                        SortNote();
                    }
                }
            });
        }

        public void DeleteNote(object sender, RoutedEventArgs e)
        {
            ApplicationHelper.RunAsMainThread(() =>
            {
                var image = sender as System.Windows.Controls.Image;
                var noteId = image?.Tag?.ToString() ?? string.Empty;

                if (!string.IsNullOrEmpty(noteId))
                {
                    var note = ProfileNotes.FirstOrDefault(n => n.Id.Equals(noteId));
                    if (note != null)
                    {
                        var actionResult = DatabaseService.Instance.Delete<Note>(noteId);
                        if (actionResult.IsSuccess)
                        {
                            ProfileNotes.Remove(note);
                            AllNotes.Remove(note);
                        }
                    }
                }
            });
        }

        public void InsertNote(InsertNoteMessage message)
        {
            InsertNewNote();
        }

        public void CloseForm(CloseFormMessage message)
        {
            WindowsHelper.HideWindow(WindowId);
        }

        public void SaveContent(SaveNoteMessage message)
        {
            switch (message.Mode)
            {
                case Enums.EditingMode.AddNew:
                    {
                        if (string.IsNullOrEmpty(message.Note.Content) && string.IsNullOrEmpty(message.Note.Title)) return;

                        var actionResult = DatabaseService.Instance.Insert(message.Note);
                        if (actionResult.IsSuccess)
                        {
                            ProfileNotes.Add(message.Note);
                            AllNotes.Add(message.Note);

                            if (ProfileNotes.Count == 1)
                            {
                                PopulateNoteListToUI();
                            }
                            else
                            {
                                ProfileNotes.Refresh();
                            }

                            NoteProfileWrapper.Instance.Insert(new NoteProfile
                            {
                                Id = Guid.NewGuid().ToString(),
                                NoteId = message.Note.Id,
                                ProfileId = _selectedProfileId
                            });
                        }
                    }
                    break;

                case Enums.EditingMode.Edit:
                    {
                        var actionResult = DatabaseService.Instance.Update(message.Note);
                        if (actionResult.IsSuccess)
                        {
                            for (int i = 0; i < ProfileNotes.Count; i++)
                            {
                                if (ProfileNotes[i].Id.Equals(message.Note.Id))
                                {
                                    ProfileNotes[i] = message.Note;
                                    break;
                                }
                            }

                            for (int i = 0; i < AllNotes.Count; i++)
                            {
                                if (AllNotes[i].Id.Equals(message.Note.Id))
                                {
                                    AllNotes[i] = message.Note;
                                    break;
                                }
                            }

                            ProfileNotes.Refresh();
                        }
                    }
                    break;
            }
            SortNote();
        }

        public void SaveAppSetting(SaveAppSettingMessage message)
        {
            foreach (var config in message.Configs)
            {
                DatabaseService.Instance.Update(config);
            }
            ApplyLatestSetting();
        }

        public void SelectProfile(ApplySelectProfileMessage message)
        {
            _selectedProfileId = message.ProfileId;

            PopulateNoteListToUI();
            SortNote();
        }

        public void SaveSelectedProfile(SaveSelectedProfileMessage message)
        {
            var appConfigLoad = DatabaseService.Instance.GetById<AppConfig>(Const.Consts.ConfigIds.SELECTED_PROFILE);
            var appConfig = new AppConfig();
            if (appConfigLoad?.IsSuccess != true)
            {
                appConfig = new AppConfig
                {
                    Id = Consts.ConfigIds.SELECTED_PROFILE,
                    Name = "Selected Profile",
                    SettingType = Enums.SettingType.SingleText,
                    Value = message.ProfileId,
                    IsShowOnUI = false,
                };
                DatabaseService.Instance.Insert(appConfig);
            }
            else
            {
                appConfig = appConfigLoad.ReturnValue;
                appConfig!.Value = message.ProfileId;
                DatabaseService.Instance.Update(appConfig);
            }
        }

        private void ApplyLatestSetting()
        {
            ApplicationHelper.CreateShortcutInStartupFolder();

            var config = AppConfigWrapper.Instance.GetById(Const.Consts.ConfigIds.CREATE_DESKTOP_SHORTCUT);
            if (config?.Value != null && (bool)config.Value)
            {
                ApplicationHelper.CreateShortcutInDesktop();
            }
        }

        private void HeaderControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.ChangedButton == MouseButton.Left)
                    this.DragMove();
            }
            catch { }
        }

        public void CheckBoxMouseClick(object sender, RoutedEventArgs e)
        {
            if (!icNoteList.IsLoaded) return;

            var checkbox = sender as System.Windows.Controls.CheckBox;
            var noteId = checkbox?.Tag?.ToString() ?? string.Empty;

            if (!string.IsNullOrEmpty(noteId))
            {
                var note = ProfileNotes.FirstOrDefault(n => n.Id.Equals(noteId));
                if (note != null)
                {
                    var status = checkbox?.IsChecked == true ? NoteStatus.New.ToString() : NoteStatus.Archived.ToString();
                    note.NoteStatusCode = status;

                    DatabaseService.Instance.Update(note);
                }
            }

            SortNote();
        }

        private void SortNote()
        {
            if (AllNotes == null) return;
            var newNotes = AllNotes
                .Where(note => note.NoteStatusCode.Equals(NoteStatus.New.ToString()))
                .OrderByDescending(note => note.CreateDate)
                .ToList();
            var archivedNotes = AllNotes
                .Where(note => note.NoteStatusCode.Equals(NoteStatus.Archived.ToString()))
                .OrderByDescending(note => note.CreateDate)
                .ToList();
            AllNotes = new ObservableCollection<Note>();

            newNotes.ForEach(note => AllNotes.Add(note));
            archivedNotes.ForEach(note => AllNotes.Add(note));

            PopulateNoteListToUI();
            _selectedNoteId = null;
        }

        public void NoteTitleClicked(object sender, MouseButtonEventArgs e)
        {
            var textBlock = sender as System.Windows.Controls.TextBlock;
            _selectedNoteId = textBlock?.Tag?.ToString() ?? string.Empty;

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                EditNote(_selectedNoteId);
            }
        }

        public void EditIconClicked(object sender, RoutedEventArgs e)
        {
            var image = sender as System.Windows.Controls.Image;
            var noteId = image?.Tag?.ToString() ?? string.Empty;

            EditNote(noteId);
        }

        private void EditNote(string noteId)
        {
            if (System.Windows.Forms.Control.ModifierKeys == Keys.Control)
            {
                this.Hide();
            }

            if (!string.IsNullOrEmpty(noteId))
            {
                var note = ProfileNotes.FirstOrDefault(n => n.Id.Equals(noteId));
                if (note != null)
                {
                    if (!string.IsNullOrEmpty(note.Password))
                    {
                        var passwordInput = new TextInput(FormInputMode.Password);
                        var isOkClicked = passwordInput.ShowDialog();
                        if (isOkClicked == false) return;

                        var ecryptedPassword = CryptoHelper.Encrypt(passwordInput.txtPassword.Password);
                        if (!ecryptedPassword.Equals(note.Password))
                        {
                            MessageBox.Show("Password is incorrect!");
                            return;
                        }
                    }

                    NoteEditor noteEditor = new NoteEditor();
                    noteEditor.Mode = Enums.EditingMode.Edit;
                    noteEditor.OriginalNote = note;
                    noteEditor.Show();
                }
            }
        }

        private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Escape)
                    this.Hide();
            }
            catch { }
        }

        private void TextBlockContextMenuEdit_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_selectedNoteId)) return;
            EditNote(_selectedNoteId);
        }

        private void TextBlockContextMenuExport_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Tính năng này dev lười chưa làm");
        }

        private void TextBlockContextMenuMoveProfile_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Tính năng này dev lười chưa làm");
        }

        private void TextBlockContextMenuMarkCompleted_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Tính năng này dev lười chưa làm");
        }

        private void TextBlockContextMenuDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Tính năng này dev lười chưa làm");
        }
    }
}