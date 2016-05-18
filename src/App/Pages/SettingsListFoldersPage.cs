﻿using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Bit.App.Abstractions;
using Bit.App.Controls;
using Bit.App.Models.Page;
using Bit.App.Resources;
using Xamarin.Forms;
using XLabs.Ioc;

namespace Bit.App.Pages
{
    public class SettingsListFoldersPage : ContentPage
    {
        private readonly IFolderService _folderService;
        private readonly IUserDialogs _userDialogs;
        public SettingsListFoldersPage()
        {
            _folderService = Resolver.Resolve<IFolderService>();
            _userDialogs = Resolver.Resolve<IUserDialogs>();

            Init();
        }

        public ObservableCollection<SettingsFolderPageModel> Folders { get; private set; } = new ObservableCollection<SettingsFolderPageModel>();

        private void Init()
        {
            ToolbarItems.Add(new AddFolderToolBarItem(this));

            var listView = new ListView
            {
                ItemsSource = Folders,
                SeparatorColor = Color.FromHex("d2d6de")
            };
            listView.ItemSelected += FolderSelected;
            listView.ItemTemplate = new DataTemplate(() => new SettingsFolderListViewCell(this));

            Title = "Folders";
            Content = listView;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadFoldersAsync().Wait();
        }

        private async Task LoadFoldersAsync()
        {
            Folders.Clear();

            var folders = await _folderService.GetAllAsync();
            foreach(var folder in folders)
            {
                var f = new SettingsFolderPageModel(folder);
                Folders.Add(f);
            }
        }

        private void FolderSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var folder = e.SelectedItem as SettingsFolderPageModel;
            var page = new ExtendedNavigationPage(new SettingsEditFolderPage(folder.Id));
            Navigation.PushModalAsync(page);
        }

        private class AddFolderToolBarItem : ToolbarItem
        {
            private readonly SettingsListFoldersPage _page;

            public AddFolderToolBarItem(SettingsListFoldersPage page)
            {
                _page = page;
                Text = AppResources.Add;
                Icon = "ion-plus";
                Clicked += ClickedItem;
            }

            private async void ClickedItem(object sender, EventArgs e)
            {
                var page = new ExtendedNavigationPage(new SettingsAddFolderPage());
                await _page.Navigation.PushModalAsync(page);
            }
        }

        private class SettingsFolderListViewCell : ExtendedTextCell
        {
            public SettingsFolderListViewCell(SettingsListFoldersPage page)
            {
                this.SetBinding<SettingsFolderPageModel>(TextProperty, s => s.Name);
                TextColor = Color.FromHex("333333");
            }
        }
    }
}
