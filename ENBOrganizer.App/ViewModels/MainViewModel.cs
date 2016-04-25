﻿using ENBOrganizer.App.Messages;
using ENBOrganizer.Model.Entities;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System.ComponentModel;

namespace ENBOrganizer.App.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private PresetsOverviewViewModel _presetsOverviewViewModel;
        private PresetDetailViewModel _presetDetailViewModel;

        public GamesViewModel GamesViewModel { get; set; }

        private bool _isAddGameFlyoutOpen;

        public bool IsAddGameFlyoutOpen
        {
            get { return _isAddGameFlyoutOpen; }
            set
            {
                _isAddGameFlyoutOpen = value;
                RaisePropertyChanged("IsAddGameFlyoutOpen");
            }
        }
        
        private ViewModelBase _currentViewModel;

        public ViewModelBase CurrentViewModel
        {
            get { return _currentViewModel; }
            set
            {
                _currentViewModel = value;
                RaisePropertyChanged("CurrentViewModel");
            }
        }

        public MainViewModel(PresetsOverviewViewModel presetsOverviewViewModel, PresetDetailViewModel presetDetailViewModel, GamesViewModel gamesViewModel)
        {
            _presetsOverviewViewModel = presetsOverviewViewModel;
            _presetDetailViewModel = presetDetailViewModel;
            GamesViewModel = gamesViewModel;
            GamesViewModel.PropertyChanged += GamesViewModel_PropertyChanged;

            CurrentViewModel = _presetsOverviewViewModel;

            MessengerInstance.Register<PropertyChangedMessage<Game>>(this, (message) => CurrentViewModel = _presetsOverviewViewModel);
            MessengerInstance.Register<NavigationMessage>(this, OnNavigationMessage);
            MessengerInstance.Register<DialogMessage>(this, (message) => OnDialogMessage(message));
        }

        private void GamesViewModel_PropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == "CurrentGame")
                CurrentViewModel = _presetsOverviewViewModel;
        }

        private void OnPresetMessage(Preset preset)
        {
            _presetDetailViewModel.Preset = preset;
            CurrentViewModel = _presetDetailViewModel;
        }

        private void OnNavigationMessage(NavigationMessage navigationMessage)
        {
            switch (navigationMessage.ViewName)
            {
                case ViewNames.PresetDetail:
                    CurrentViewModel = _presetDetailViewModel;
                    break;
                case ViewNames.PresetsOverview:
                    CurrentViewModel = _presetsOverviewViewModel;
                    break;
                default:
                    break;
            }
        }

        private void OnDialogMessage(DialogMessage dialogMessage)
        {
            IsAddGameFlyoutOpen = dialogMessage.DialogAction == DialogActions.Open;
        }
    }
}