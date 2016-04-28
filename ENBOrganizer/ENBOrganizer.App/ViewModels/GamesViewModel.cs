﻿using ENBOrganizer.Domain;
using ENBOrganizer.Domain.Services;
using ENBOrganizer.Domain.Entities;
using ENBOrganizer.Util;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace ENBOrganizer.App.ViewModels
{
    public class GamesViewModel : ViewModelBase
    {
        private readonly GameService _gameService;
        private readonly PresetService _presetService;

        public ICommand ShowAddGameDialogCommand { get; set; }
        public ICommand DeleteGameCommand { get; set; }

        private ObservableCollection<Game> _games;

        public ObservableCollection<Game> Games
        {
            get { return _games; }
            set
            {
                _games = value;
                RaisePropertyChanged("Games");
            }
        }

        public Game CurrentGame
        {
            get { return Properties.Settings.Default.CurrentGame; }
            set
            {
                Properties.Settings.Default.CurrentGame = value;
                Properties.Settings.Default.Save();

                RaisePropertyChanged("CurrentGame");
            }
        }

        public GamesViewModel(GameService gameService, PresetService presetService)
        {
            _gameService = gameService;
            _gameService.ItemsChanged += _gameService_ItemsChanged;

            _presetService = presetService;

            ShowAddGameDialogCommand = new RelayCommand(() => DialogService.ShowAddGameDialog());
            DeleteGameCommand = new RelayCommand(DeleteGame, CanDelete);

            Games = _gameService.GetAll().ToObservableCollection();

            // HACK: populates PresetsOverview on load
            // RaisePropertyChanged("CurrentGame", null, CurrentGame, true);
        }

        private void _gameService_ItemsChanged(object sender, RepositoryChangedEventArgs repositoryChangedEventArgs)
        {
            Game game = repositoryChangedEventArgs.Entity as Game;

            if (repositoryChangedEventArgs.RepositoryActionType == RepositoryActionType.Added)
            {
                Games.Add(game);
                CurrentGame = game;
            }
            else
                Games.Remove(game);
        }

        private void DeleteGame()
        {
            _presetService.DeleteByGame(CurrentGame);
            _gameService.Delete(CurrentGame);

            CurrentGame = Games.FirstOrDefault();
        }

        private bool CanDelete()
        {
            return CurrentGame != null;
        }
    }
}