﻿/*
 * To add Offline Sync Support:
 *  1) Add the NuGet package Microsoft.Azure.Mobile.Client.SQLiteStore (and dependencies) to all client projects
 *  2) Uncomment the #define OFFLINE_SYNC_ENABLED
 *
 * For more information, see: http://go.microsoft.com/fwlink/?LinkId=717898
 */
//#define OFFLINE_SYNC_ENABLED

using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.Net.Http;
using MySmartHouse1.DataModel;
using OxyPlot;

#if OFFLINE_SYNC_ENABLED
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;  // offline sync
using Microsoft.WindowsAzure.MobileServices.Sync;         // offline sync
#endif

namespace MySmartHouse1
{
    public sealed partial class MainPage : Page
    {
        private MobileServiceCollection<Parameters, Parameters> items;
        private MobileServiceCollection<StatisticsHTs, StatisticsHTs> stats; 
#if OFFLINE_SYNC_ENABLED
        private IMobileServiceSyncTable<TodoItem> todoTable = App.MobileService.GetSyncTable<TodoItem>(); // offline sync
#else
        private IMobileServiceTable<Parameters> parameters = App.MobileService.GetTable<Parameters>();
        private IMobileServiceTable<StatisticsHTs> statistics = App.MobileService.GetTable<StatisticsHTs>();  
#endif
        private ViewHouseEntity currentHouseEntity;

        public MainPage()
        {
            this.InitializeComponent();
            currentHouseEntity = new ViewHouseEntity();
            LayoutRoot.DataContext = currentHouseEntity;
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
                items = await parameters.ToCollectionAsync();
                stats = await statistics.OrderBy<DateTime>(item => item.ValueDateTime).ToCollectionAsync();
            }
            catch (MobileServiceInvalidOperationException e)
            {
                exception = e;
            }
            catch(HttpRequestException e)
            {
                await new MessageDialog(String.Format("{0}\nService is not available", e.Message), "Error loading items").ShowAsync();
                return;
            }

            if (exception != null)
            {
                await new MessageDialog(exception.Message, "Error loading items").ShowAsync();
            }
            else
            {
                currentHouseEntity.HouseEntity.Temperature = items.FirstOrDefault(i => i.Name == "Temperature")?.Value;
                currentHouseEntity.HouseEntity.Humidity = items.FirstOrDefault(i => i.Name == "Humidity")?.Value;
                currentHouseEntity.HouseEntity.FanMode = items.FirstOrDefault(i => i.Name == "FanMode").Value;
                currentHouseEntity.HouseEntity.FanPower = items.FirstOrDefault(i => i.Name == "FanPower").Value;
                currentHouseEntity.HouseEntity.Door = items.FirstOrDefault(i => i.Name == "Door").Value;

                //List<StatisticsHTs> list = new List<StatisticsHTs>()
                //{
                //    new StatisticsHTs() {ValueDateTime = new DateTime(2017,12,20,12,0,0), Humidity = 80, Temperature = 24},
                //    new StatisticsHTs() {ValueDateTime = new DateTime(2017,12,20,12,2,0), Humidity = 82, Temperature = 24},
                //    new StatisticsHTs() {ValueDateTime = new DateTime(2017,12,20,12,4,0), Humidity = 90, Temperature = 25},
                //    new StatisticsHTs() {ValueDateTime = new DateTime(2017,12,20,12,6,0), Humidity = 99, Temperature = 25},
                //    new StatisticsHTs() {ValueDateTime = new DateTime(2017,12,20,12,8,0), Humidity = 99, Temperature = 25},
                //    new StatisticsHTs() {ValueDateTime = new DateTime(2017,12,20,12,10,0), Humidity = 91, Temperature = 26},
                //    new StatisticsHTs() {ValueDateTime = new DateTime(2017,12,20,12,12,0), Humidity = 77, Temperature = 26},
                //    new StatisticsHTs() {ValueDateTime = new DateTime(2017,12,20,12,14,0), Humidity = 64, Temperature = 24}
                //};
                currentHouseEntity.Statistics = stats.ToList();
            }
        }

        private async Task UpdateCheckedTodoItem(Parameters item)
        {
            // This code takes a freshly completed TodoItem and updates the database.
            // After the MobileService client responds, the item is removed from the list.
            await parameters.UpdateAsync(item);
            items.Remove(item);
            //ListItems.Focus(Windows.UI.Xaml.FocusState.Unfocused);

#if OFFLINE_SYNC_ENABLED
            await App.MobileService.SyncContext.PushAsync(); // offline sync
#endif
        }

        private async void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            currentHouseEntity.IsRefreshBusy = true;
#if OFFLINE_SYNC_ENABLED
            await SyncAsync(); // offline sync
#endif
            FanMode.Toggled -= ToggleSwitch_Toggled;
            FanPower.Toggled -= ToggleSwitch_Toggled;
            await RefreshTodoItems();
            currentHouseEntity.IsRefreshBusy = false;
            FanMode.Toggled += ToggleSwitch_Toggled;
            FanPower.Toggled += ToggleSwitch_Toggled;
        }
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


        private async void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            var toggle = sender as ToggleSwitch;
            if (toggle == null) return;
            Debug.WriteLine(String.Format("Called Toggle_switch({0})", toggle.Name));
            SetProgressRing(toggle.Name, true);
            currentHouseEntity.IsParameterBusy++;
            var forUpdate = await parameters.Where(i => i.Name == toggle.Name).ToListAsync();
            if (forUpdate.Count == 0)
            {
                Parameters fanRow = new Parameters
                {
                    Name = toggle.Name,
                    Value = toggle.IsOn ? 1 : 0
                };
                await InsertParameters(fanRow);
            }
            else
            {
                foreach (var item in forUpdate)
                {
                    item.Value = toggle.IsOn ? 1 : 0;
                    try
                    {
                        await parameters.UpdateAsync(item);
                    }
                    catch (MobileServiceInvalidOperationException)
                    {
                        
                    }
                }
            }
            SetProgressRing(toggle.Name, false);
            currentHouseEntity.IsParameterBusy--;
        }

        private void SetProgressRing(string name, bool value)
        {
            if (String.Equals(name, "FanMode"))
            {
                ProgressRingMode.IsActive  = value;
            }
            else if (String.Equals(name, "FanPower"))
            {
                ProgressRingPower.IsActive = value;
            }
        }
    }
}
