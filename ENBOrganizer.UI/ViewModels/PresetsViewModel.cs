﻿using ENBOrganizer.Domain;
using ENBOrganizer.Domain.Services;
using ENBOrganizer.Model.Entities;
using ENBOrganizer.UI.Views;
using ENBOrganizer.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;

namespace ENBOrganizer.UI.ViewModels
{
    // TODO: (UI) presets TreeView formatting with icons
    // TODO: install preset
    // TODO: on add directory/file, return a preset item so the repository event args can be used (requires change in architecture)
    public class PresetsViewModel : ObservableObject, IDataErrorInfo
    {
        private string _name;
        private Preset _selectedPreset;
        private readonly PresetService _presetService;
        private readonly GameService _gameService;
        private readonly PresetItemsService _presetItemsService;
        
        public PresetItem SelectedPresetItem { get; set; }
        public ICommand AddPresetCommand { get; set; }
        public ICommand ImportPresetCommand { get; set; }
        public ICommand DeletePresetCommand { get; set; }
        public ICommand DeleteItemCommand { get; set; }
        public ICommand AddDirectoryCommand { get; set; }
        public ICommand AddFileCommand { get; set; }

        public Preset SelectedPreset
        {
            get { return _selectedPreset; }
            set
            {
                _selectedPreset = value;
                Name = value != null ? value.Name : string.Empty;

                RaisePropertyChanged("SelectedPreset");
                RaisePropertyChanged("Items");
            }
        }
        
        public List<Preset> Presets
        {
            get { return _presetService.GetByGame(_gameService.ActiveGame); }
        }

        public List<PresetItem> Items
        {
            get
            {
                if (_gameService.ActiveGame == null || _selectedPreset == null)
                    return new List<PresetItem>();

                return _presetItemsService.GetPresetItems(Path.Combine(_gameService.ActiveGame.PresetsDirectory.FullName, _selectedPreset.Name));
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value.Trim();
                RaisePropertyChanged("Name");
            }
        }

        public string Error
        {
            get { return null; }
        }

        // TODO: (UI) only validate control when trying to add new item
        public string this[string columnName]
        {
            get
            {
                string errorMessage = string.Empty;

                switch (columnName)
                {
                    case "Name":
                        if (string.IsNullOrEmpty(Name))
                            errorMessage = "Name is required.";
                        else if (_gameService.ActiveGame == null)
                            errorMessage = "No game selected.";
                        break;
                }

                return errorMessage;
            }
        }

        public PresetsViewModel()
        {
            _presetService = ServiceSingletons.PresetService;
            _gameService = ServiceSingletons.GameService;
            _presetItemsService = new PresetItemsService();

            _gameService.PropertyChanged += OnActiveGameChanged;
            _presetService.PresetsChanged += OnPresetsChanged;

            AddPresetCommand = new ActionCommand(AddPreset, CanAddPreset);
            ImportPresetCommand = new ActionCommand(ImportPreset, CanImport);
            DeletePresetCommand = new ActionCommand(DeletePreset, () => true); // TODO: validation
            DeleteItemCommand = new ActionCommand(DeleteItem, () => true);
            AddDirectoryCommand = new ActionCommand(AddDirectory, () => true);
            AddFileCommand = new ActionCommand(AddFile, () => true);
        }
        
        private void OnActiveGameChanged(object sender, PropertyChangedEventArgs eventArgs)
        {            
            RaisePropertyChanged("Presets");

            SelectedPreset = Presets.FirstOrDefault();
        }

        private void AddPreset()
        {
            try
            {
                _presetService.Add(new Preset(Name, _gameService.ActiveGame));
            }
            catch (InvalidOperationException exception)
            {
                MessageBoxUtil.ShowError(exception.Message);
            }
        }

        private bool CanAddPreset()
        {
            return _gameService.ActiveGame != null;
        }

        // TODO: (UI) status bar
        private void ImportPreset()
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog { Description = "Please select the preset folder..." };

            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    _presetService.Import(folderBrowserDialog.SelectedPath, _gameService.ActiveGame);
                }
                catch (InvalidOperationException exception)
                {
                    MessageBoxUtil.ShowError(exception.Message);
                }
                catch (UnauthorizedAccessException exception)
                {
                    MessageBoxUtil.ShowError("Not all files were imported." + Environment.NewLine + Environment.NewLine + exception.Message);
                }
                catch (PathTooLongException exception)
                {
                    MessageBoxUtil.ShowError(exception.Message);
                }
                catch (IOException exception)
                {
                    MessageBoxUtil.ShowError("Not all files were imported." + Environment.NewLine + Environment.NewLine + exception.Message);
                }
            }
        }

        private bool CanImport()
        {
            return _gameService.ActiveGame != null;
        }

        private void AddDirectory()
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog { Description = "Please select a folder to add..." };

            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                _presetItemsService.CopyDirectoryAsPresetItem(SelectedPresetItem, folderBrowserDialog.SelectedPath);

            RaisePropertyChanged("Items");
        }

        private void AddFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Please select a file(s) to add...",
                Filter = "All Files (*.*)|*.*",
                Multiselect = true
            };

            foreach (string filePath in openFileDialog.FileNames)
                _presetItemsService.CopyFileAsPresetItem(SelectedPresetItem, filePath);

            RaisePropertyChanged("Items");
        }

        private void DeleteItem()
        {
            SelectedPresetItem.Delete();

            OnPresetItemsChanged(this, new RepositoryChangedEventArgs(RepositoryActionType.Deleted, SelectedPresetItem));
        }

        private void DeletePreset()
        {
            _presetService.Delete(SelectedPreset);

            OnPresetItemsChanged(this, new RepositoryChangedEventArgs(RepositoryActionType.Deleted, SelectedPresetItem));
        }

        private void OnPresetsChanged(object sender, RepositoryChangedEventArgs repositoryChangedEventArgs)
        {
            Preset preset = repositoryChangedEventArgs.Entity as Preset;

            RaisePropertyChanged("Presets");

            SelectedPreset = repositoryChangedEventArgs.RepositoryActionType == RepositoryActionType.Added ? preset : Presets.FirstOrDefault();
        }

        private void OnPresetItemsChanged(object sender, RepositoryChangedEventArgs repositoryChangedEventArgs)
        {
            PresetItem presetItem = repositoryChangedEventArgs.Entity as PresetItem;

            RaisePropertyChanged("Items");

            SelectedPresetItem = repositoryChangedEventArgs.RepositoryActionType == RepositoryActionType.Added ? presetItem : Items.FirstOrDefault();
        }
    }
}
