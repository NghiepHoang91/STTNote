using STTNote.ActionMessages;
using STTNote.Const;
using STTNote.Extensions;
using STTNote.Helpers;
using STTNote.Models;
using STTNote.Views;
using STTNote.Wrappers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows.Controls;

namespace STTNote.Controls
{
    /// <summary>
    /// Interaction logic for MainWindowHeaderControl.xaml
    /// </summary>
    public partial class MainWindowHeaderControl : BaseUserControl
    {
        private ObservableCollection<Profile> _profileList;
        private bool _isInInitProcess = true;

        public MainWindowHeaderControl()
        {
            InitializeComponent();
            SetDefaultSize();
            SetControlPosition();
            LoadProfileData();
            InitProfileBox();
        }

        private void LoadProfileData()
        {
            _profileList = new ObservableCollection<Profile>();

            var appConfigLoad = ProfileWrapper.Instance.Getall();
            if (appConfigLoad?.IsSuccess == true)
            {
                appConfigLoad.ReturnValue.ForEach(p => _profileList.Add(p));
            }
            _profileList.Add(new Profile
            {
                Id = "",
                Title = "..."
            });
        }

        private void InitProfileBox()
        {
            boxProfile.ItemsSource = _profileList;
            boxProfile.DisplayMemberPath = nameof(Profile.Title);
            boxProfile.SelectedValuePath = nameof(Profile.Id);
            boxProfile.SelectionChanged += ProfileSelectChanged;
        }

        public void SetSelectedProfileValue()
        {
            Profile? selected = null;
            var selectedProfile = AppConfigWrapper.Instance.GetById(Const.Consts.ConfigIds.SELECTED_PROFILE);
            if (selectedProfile != null)
            {
                selected = _profileList.FirstOrDefault(n => n.Id.Equals(selectedProfile.Value.ToString()));
            }

            if (selected == null)
            {
                selected = _profileList.FirstOrDefault();
                ReferrencesHelper.SendMessage(new SaveSelectedProfileMessage
                {
                    ProfileId = selected?.Id
                });
            }

            if (selected != null)
            {
                boxProfile.Dispatcher.Invoke(() =>
                {
                    boxProfile.SelectedItem = selected;
                    Thread.Sleep(50);
                });
            }
        }

        public void ProfileSelectChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isInInitProcess && e.RemovedItems.Count > 0)
            {
                ComboBox combo = (ComboBox)sender;
                combo.SelectedItem = e.RemovedItems[0];

                _isInInitProcess = false;
            }

            if (e.AddedItems.Count == 0) { return; }
            var selectedProfile = e.AddedItems[0] as Profile;

            if (string.IsNullOrEmpty(selectedProfile?.Id))
            {
                var inputForm = new TextInput(Enums.FormInputMode.PLainText, "Enter profile name:");
                var result = inputForm.ShowDialog();
                if (result == true && !string.IsNullOrEmpty(inputForm.InputValue))
                {
                    selectedProfile = new Profile
                    {
                        Id = Guid.NewGuid().ToString(),
                        Title = inputForm.InputValue,
                    };
                    ProfileWrapper.Instance.Insert(selectedProfile);
                    var functionalProfile = _profileList.First(n => string.IsNullOrEmpty(n.Id));
                    _profileList.Remove(functionalProfile);
                    _profileList.Add(selectedProfile);
                    _profileList.Add(new Profile
                    {
                        Id = "",
                        Title = "..."
                    });
                    boxProfile.SelectedValue = selectedProfile.Id;
                }
                else
                {
                    return;
                }
            }

            ReferrencesHelper.SendMessage(new SaveSelectedProfileMessage
            {
                ProfileId = selectedProfile?.Id
            });
            ReferrencesHelper.SendMessage(new ApplySelectProfileMessage
            {
                ProfileId = selectedProfile?.Id
            });
        }

        private void SetDefaultSize()
        {
            Width = Consts.GLOBAL_FORM_WIDTH;
            headerPanel.Width = Consts.GLOBAL_FORM_WIDTH;
        }

        private void SetControlPosition()
        {
            //Close button
            var cb_marginLeft = Consts.GLOBAL_FORM_WIDTH - Const.Consts.HEADER_HEIGHT + 8;
            labelClose.Margin = new System.Windows.Thickness(cb_marginLeft, 4, 0, 0);

            //Setting
            var st_marginLeft = labelClose.Margin.Left - 35;
            labelSetting.Margin = new System.Windows.Thickness(st_marginLeft, 0, 0, 0);

            //Profile box
            var pb_marginLeft = labelSetting.Margin.Left - boxProfile.Width - 10;
            boxProfile.Margin = new System.Windows.Thickness(pb_marginLeft, 4, 0, 0);
        }

        private void labelClose_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ReferrencesHelper.SendMessage(new CloseFormMessage());
        }

        private void labelAddNew_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ReferrencesHelper.SendMessage(new InsertNoteMessage());
        }

        private void labelSetting_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var setting = new AppSetting();
            var loadConfigs = AppConfigWrapper.Instance.GetAll();
            if (loadConfigs?.IsSuccess == true && loadConfigs?.ReturnValue != null)
            {
                var configs = loadConfigs.ReturnValue.Where(cf => cf.IsShowOnUI).ToList();

                setting.SetAppConfig(configs);
                setting.Show();
            }
        }
    }
}