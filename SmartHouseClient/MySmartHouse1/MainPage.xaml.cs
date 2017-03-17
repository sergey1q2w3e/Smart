/*
 * To add Offline Sync Support:
 *  1) Add the NuGet package Microsoft.Azure.Mobile.Client.SQLiteStore (and dependencies) to all client projects
 *  2) Uncomment the #define OFFLINE_SYNC_ENABLED
 *
 * For more information, see: http://go.microsoft.com/fwlink/?LinkId=717898
 */
//#define OFFLINE_SYNC_ENABLED

using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using MySmartHouse1.Common;

#if OFFLINE_SYNC_ENABLED
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;  // offline sync
using Microsoft.WindowsAzure.MobileServices.Sync;         // offline sync
#endif

namespace MySmartHouse1
{
    public sealed partial class MainPage : Page
    {
        private MobileServiceCollection<Parameters, Parameters> items;
#if OFFLINE_SYNC_ENABLED
        private IMobileServiceSyncTable<TodoItem> todoTable = App.MobileService.GetSyncTable<TodoItem>(); // offline sync
#else
        private IMobileServiceTable<Parameters> parameters = App.MobileService.GetTable<Parameters>();
#endif
        private ViewHouseEntity currentHouseEntity;

        

        public MainPage()
        {
            this.InitializeComponent();
            currentHouseEntity = new ViewHouseEntity();
            ContentPanel.DataContext = currentHouseEntity;
            //currentHouseEntity.Humidity = 30;
            //currentHouseEntity.Temperature = 25;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
#if OFFLINE_SYNC_ENABLED
            await InitLocalStoreAsync(); // offline sync
#endif
            ButtonRefresh_Click(this, null);

        }

        private async Task InsertParameters(Parameters todoItem)
        {
            // This code inserts a new Parameters into the database. After the operation completes
            // and the mobile app backend has assigned an id, the item is added to the CollectionView.
            await parameters.InsertAsync(todoItem);
            items.Add(todoItem);
            
#if OFFLINE_SYNC_ENABLED
            await App.MobileService.SyncContext.PushAsync(); // offline sync
#endif
        }

        private async Task RefreshTodoItems()
        {
            MobileServiceInvalidOperationException exception = null;
            try
            {
                // This code refreshes the entries in the list view by querying the TodoItems table.
                // The query excludes completed TodoItems.
                items = await parameters
                    .ToCollectionAsync();
            }
            catch (MobileServiceInvalidOperationException e)
            {
                exception = e;
            }

            if (exception != null)
            {
                await new MessageDialog(exception.Message, "Error loading items").ShowAsync();
            }
            else
            {
                ListItems.ItemsSource = items;
                
                
                currentHouseEntity.HouseEntity.Temperature = items.FirstOrDefault(i => i.Name == "Temperature")?.Value;
                currentHouseEntity.HouseEntity.Humidity = items.FirstOrDefault(i => i.Name == "Humidity")?.Value;
                currentHouseEntity.HouseEntity.FanMode = items.FirstOrDefault(i => i.Name == "FanMode")?.Value;
                currentHouseEntity.HouseEntity.FanPower = items.FirstOrDefault(i => i.Name == "FanPower")?.Value;

                //this.ButtonSave.IsEnabled = true;

            }
        }

        private async Task UpdateCheckedTodoItem(Parameters item)
        {
            // This code takes a freshly completed TodoItem and updates the database.
			// After the MobileService client responds, the item is removed from the list.
            await parameters.UpdateAsync(item);
            items.Remove(item);
            ListItems.Focus(Windows.UI.Xaml.FocusState.Unfocused);

#if OFFLINE_SYNC_ENABLED
            await App.MobileService.SyncContext.PushAsync(); // offline sync
#endif
        }

        private async void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            //ButtonRefresh.IsEnabled = false;
#if OFFLINE_SYNC_ENABLED
            await SyncAsync(); // offline sync
#endif
            imgAnimated.Visibility= Visibility.Visible;
            imgNotAnimated.Visibility= Visibility.Collapsed;
            await RefreshTodoItems();

            ButtonRefresh.IsEnabled = true;

            imgAnimated.Visibility = Visibility.Collapsed;
            imgNotAnimated.Visibility = Visibility.Visible;
        }

        //private async void ButtonSave_Click(object sender, RoutedEventArgs e)
        //{
        //    var todoItem = new Parameters { Name = NameInput.Text, Value = Int32.Parse(ValueInput.Text)};
        //    ValueInput.Text = "";
        //    NameInput.Text = "";
        //    await InsertParameters(todoItem);
        //}


        //private void TextInput_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        //{
        //    if (e.Key == Windows.System.VirtualKey.Enter) {
        //        ButtonSave.Focus(FocusState.Programmatic);
        //    }
        //}

        #region Offline sync
#if OFFLINE_SYNC_ENABLED
        private async Task InitLocalStoreAsync()
        {
           if (!App.MobileService.SyncContext.IsInitialized)
           {
               var store = new MobileServiceSQLiteStore("localstore.db");
               store.DefineTable<TodoItem>();
               await App.MobileService.SyncContext.InitializeAsync(store);
           }

           await SyncAsync();
        }

        private async Task SyncAsync()
        {
           await App.MobileService.SyncContext.PushAsync();
           await todoTable.PullAsync("todoItems", todoTable.CreateQuery());
        }
#endif
        #endregion

        private async void TestButton_Click(object sender, RoutedEventArgs e)
        {
            var forUpdate = await parameters.Where(i => i.Name == "Humidity").ToListAsync();
            foreach(var item in forUpdate)
            {
                item.Value = 9113;
                await parameters.UpdateAsync(item);
            }
        }

        private async void ComboFan_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var combo = sender as ComboBox;
            if (combo == null) return;
            var forUpdate = await parameters.Where(i => i.Name == combo.Name ).ToListAsync();
            if (forUpdate.Count == 0)
            {
                Parameters fanModeRow = new Parameters
                {
                    Name = combo.Name,
                    Value = combo.SelectedIndex
                };
                await InsertParameters(fanModeRow);
            }
            else
            {
                foreach (var item in forUpdate)
                {
                    item.Value = combo.SelectedIndex;
                    await parameters.UpdateAsync(item);
                }
            }
        }

        private void IncrementParameters(object sender, RoutedEventArgs e)
        {
            //((ViewHouseEntity)ContentPanel.DataContext).HouseEntity.Humidity++;
            //((ViewHouseEntity)ContentPanel.DataContext).HouseEntity.Temperature++;
            currentHouseEntity.HouseEntity.Humidity++;
            currentHouseEntity.HouseEntity.Temperature++;
        }
    }
}
