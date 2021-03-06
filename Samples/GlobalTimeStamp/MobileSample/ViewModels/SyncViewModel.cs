﻿using System;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;
using MobileSample.Models;
using MobileSample.Services;
using NETCoreSync;
using System.Text;

namespace MobileSample.ViewModels
{
    public class SyncViewModel : BaseViewModel
    {
        private readonly INavigation navigation;
        private readonly DatabaseService databaseService;
        private readonly SyncConfiguration syncConfiguration;

        public SyncViewModel(INavigation navigation, DatabaseService databaseService, SyncConfiguration syncConfiguration)
        {
            this.navigation = navigation;
            this.databaseService = databaseService;
            this.syncConfiguration = syncConfiguration;
            Title = MainMenuItem.GetMenus().Where(w => w.Id == MenuItemType.Sync).First().Title;

            ServerUrl = databaseService.GetServerUrl();
        }

        private string serverUrl;
        public string ServerUrl
        {
            get { return serverUrl; }
            set { SetProperty(ref serverUrl, value); }
        }

        private bool isSynchronizing;
        public bool IsSynchronizing
        {
            get { return isSynchronizing; }
            set { SetProperty(ref isSynchronizing, value); }
        }

        private string log;
        public string Log
        {
            get { return log; }
            set { SetProperty(ref log, value); }
        }

        public ICommand SynchronizeCommand => new Command(async () =>
        {
            if (IsSynchronizing)
            {
                await Application.Current.MainPage.DisplayAlert("Sync is Running", "Synchronization is already running, please wait until it finished", "OK");
                return;
            }
            
            IsSynchronizing = true;

            try
            {
                if (string.IsNullOrEmpty(ServerUrl)) throw new Exception("Please specify Server URL");

                databaseService.SetServerUrl(ServerUrl);
                string synchronizationId = databaseService.GetSynchronizationId();
                if (string.IsNullOrEmpty(synchronizationId)) throw new NullReferenceException(nameof(synchronizationId));

                CustomSyncEngine customSyncEngine = new CustomSyncEngine(databaseService, syncConfiguration);
                SyncClient syncClient = new SyncClient(synchronizationId, customSyncEngine, ServerUrl);
                Log = "";

                SyncResult result = await syncClient.SynchronizeAsync(SyncClient.SynchronizationMethodEnum.PushThenPull);

                string tempLog = "";
                tempLog += $"Client Log: {Environment.NewLine}";
                tempLog += $"Sent Changes Count: {result.ClientLog.SentChanges.Count}{Environment.NewLine}";
                tempLog += $"Applied Changes Insert Count: {result.ClientLog.AppliedChanges.Inserts.Count}{Environment.NewLine}";
                tempLog += $"Applied Changes Updates Count: {result.ClientLog.AppliedChanges.Updates.Count}{Environment.NewLine}";
                tempLog += $"Applied Changes Deletes Count: {result.ClientLog.AppliedChanges.Deletes.Count}{Environment.NewLine}";
                tempLog += $"Applied Changes Conflicts Count: {result.ClientLog.AppliedChanges.Conflicts.Count}{Environment.NewLine}";
                tempLog += $"{Environment.NewLine}";
                tempLog += $"Server Log: {Environment.NewLine}";
                tempLog += $"Sent Changes Count: {result.ServerLog.SentChanges.Count}{Environment.NewLine}";
                tempLog += $"Applied Changes Insert Count: {result.ServerLog.AppliedChanges.Inserts.Count}{Environment.NewLine}";
                tempLog += $"Applied Changes Updates Count: {result.ServerLog.AppliedChanges.Updates.Count}{Environment.NewLine}";
                tempLog += $"Applied Changes Deletes Count: {result.ServerLog.AppliedChanges.Deletes.Count}{Environment.NewLine}";
                tempLog += $"Applied Changes Conflicts Count: {result.ServerLog.AppliedChanges.Conflicts.Count}{Environment.NewLine}";
                tempLog += $"{Environment.NewLine}";
                tempLog += $"Detail Log: {Environment.NewLine}";
                for (int i = 0; i < result.Log.Count; i++)
                {
                    tempLog += $"{result.Log[i]}{Environment.NewLine}";
                }
                Log = tempLog;

                if (!string.IsNullOrEmpty(result.ErrorMessage)) throw new Exception($"Synchronization Error: {result.ErrorMessage}");
                
                await Application.Current.MainPage.DisplayAlert("Finished", "Synchronization is finished", "OK");
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Error", e.Message, "OK");
            }
            finally
            {
                IsSynchronizing = false;
            }
        });
    }
}
