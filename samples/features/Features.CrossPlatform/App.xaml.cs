﻿using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Activation;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Caliburn.Micro;
using Features.CrossPlatform.ViewModels;
using Features.CrossPlatform.Views;

namespace Features.CrossPlatform
{
    public sealed partial class App
    {
        private WinRTContainer container;

        public App()
        {
            InitializeComponent();
        }

        protected override void Configure()
        {
            MessageBinder.SpecialValues.Add("$clickeditem", c => ((ItemClickEventArgs)c.EventArgs).ClickedItem);

            container = new WinRTContainer();

            container
                .PerRequest<MenuViewModel>()
                .PerRequest<BindingsViewModel>()
                .PerRequest<ActionsViewModel>()
                .PerRequest<CoroutineViewModel>()
                .PerRequest<ExecuteViewModel>();
        }

        protected override void PrepareViewFirst(Frame rootFrame)
        {
            var navigationService = container.RegisterNavigationService(rootFrame);
            var navigationManager = SystemNavigationManager.GetForCurrentView();

            navigationService.Navigated += (s, e) =>
            {
                navigationManager.AppViewBackButtonVisibility = navigationService.CanGoBack ?
                    AppViewBackButtonVisibility.Visible : 
                    AppViewBackButtonVisibility.Collapsed;
            };
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            if (args.PreviousExecutionState == ApplicationExecutionState.Running)
                return;

            DisplayRootView<MenuView>();
        }

        protected override object GetInstance(Type service, string key)
        {
            return container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            container.BuildUp(instance);
        }

        protected override async void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            e.Handled = true;

            var dialog = new MessageDialog(e.Message, "An error has occurred");

            await dialog.ShowAsync();
        }
    }
}