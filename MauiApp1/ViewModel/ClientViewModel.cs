using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AdvocaPro.Models;
using AdvocaPro.Services;
using AdvocaPro.View;
using System.Collections.ObjectModel;

namespace AdvocaPro.ViewModel
{
    public partial class ClientViewModel : ObservableObject
    {
        #region Constants
        private const string ERROR_LOADING_CLIENTS = "Não foi possível carregar os clientes.";
        private const string ERROR_ADDING_CLIENT = "Não foi possível adicionar o cliente.";
        private const string ERROR_DELETING_CLIENT = "Não foi possível excluir o cliente.";
        private const string ERROR_SAVING_CLIENT = "Não foi possível salvar os detalhes do cliente.";
        private const string REPORT_NOT_IMPLEMENTED = "Funcionalidade não implementada";
        private const string ACTIVE_YES = "Sim";
        private const string ACTIVE_NO = "Não";
        private const string DELETE_BUTTON_ACTIVE_COLOR = "#8c1c13";
        private const string DELETE_BUTTON_INACTIVE_COLOR = "#10FFFFFF";
        private const string DETAILS_BUTTON_ACTIVE_COLOR = "#0582ca";
        private const string DETAILS_BUTTON_INACTIVE_COLOR = "#10FFFFFF";
        private const string TEXT_COLOR_ACTIVE = "#FFFFFF";
        private const string TEXT_COLOR_INACTIVE = "#40FFFFFF";
        #endregion

        #region Fields
        private readonly ClientService _clientService;
        private readonly LoginService _loginService;
        private readonly string _currentUser;
        #endregion

        #region Properties
        public ObservableCollection<Client> Clients { get; } = new();
        public ObservableCollection<string> IsActiveOptions { get; } = new() { ACTIVE_YES, ACTIVE_NO };
        public ObservableCollection<string> Uf { get; } = new()
        {
            "AC","AL","AP","AM","BA","CE","DF","ES","GO","MA","MT","MS","MG","PA","PB","PR","PE",
            "PI","RJ","RN","RS","RO","RR","SC","SP","SE","TO"
        };
        public ObservableCollection<string> ReminderOptions { get; } = new() { ACTIVE_YES, ACTIVE_NO };
        public ObservableCollection<string> ActionOptions { get; } = new() { "Retornar", "Enviar Proposta", "Enviar E-mail", "Telefonar", "Visitar" };

        private Client _newClient = new();
        public Client NewClient
        {
            get => _newClient;
            set => SetProperty(ref _newClient, value);
        }

        private Client? _selectedClient;
        public Client? SelectedClient
        {
            get => _selectedClient;
            set
            {
                if (SetProperty(ref _selectedClient, value))
                {
                    if (_selectedClient != null)
                    {
                        _selectedClient.Date = null;
                        ReminderString = _selectedClient.Reminder ? ACTIVE_YES : ACTIVE_NO;
                    }
                    OnPropertyChanged(nameof(CanExecuteClientRelatedCommands));
                    OnPropertyChanged(nameof(DeleteButtonBackgroundColor));
                    OnPropertyChanged(nameof(DeleteButtonTextColor));
                    OnPropertyChanged(nameof(DetailsButtonBackgroundColor));
                    OnPropertyChanged(nameof(DetailsButtonTextColor));
                }
            }
        }

        private bool _showSummary;
        public bool ShowSummary
        {
            get => _showSummary;
            set => SetProperty(ref _showSummary, value);
        }

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    _ = PerformSearchAsync();
                }
            }
        }

        private bool _isAddClientVisible;
        public bool IsAddClientVisible
        {
            get => _isAddClientVisible;
            set
            {
                if (SetProperty(ref _isAddClientVisible, value))
                {
                    OnPropertyChanged(nameof(IsScrollViewVisible));
                    OnPropertyChanged(nameof(DataGridColumnSpan));
                    OnPropertyChanged(nameof(ColumnWidth));
                }
            }
        }

        private bool _isDetailVisible;
        public bool IsDetailVisible
        {
            get => _isDetailVisible;
            set
            {
                if (SetProperty(ref _isDetailVisible, value))
                {
                    OnPropertyChanged(nameof(IsScrollViewVisible));
                    OnPropertyChanged(nameof(DataGridColumnSpan));
                    OnPropertyChanged(nameof(ColumnWidth));
                }
            }
        }

        private string _isActiveString = string.Empty;
        public string IsActiveString
        {
            get => _isActiveString;
            set => SetProperty(ref _isActiveString, value);
        }

        private string _reminderString = string.Empty;
        public string ReminderString
        {
            get => _reminderString;
            set => SetProperty(ref _reminderString, value);
        }

        public bool IsScrollViewVisible => IsAddClientVisible || IsDetailVisible;

        public int DataGridColumnSpan => IsScrollViewVisible ? 1 : 2;

        public GridLength ColumnWidth => IsScrollViewVisible ? new GridLength(600) : new GridLength(1, GridUnitType.Star);

        public int TotalClients => Clients.Count;

        public int TotalActive => Clients.Count(c => c.IsActive);

        public int PendingFollowUps => Clients.Count(c => c.Reminder);

        public int TodayFollowUps => Clients.Count(c => c.Reminder && c.Date.HasValue && c.Date.Value.Date == DateTime.Today);

        public bool CanExecuteClientRelatedCommands => SelectedClient != null;

        public string DeleteButtonBackgroundColor => CanExecuteClientRelatedCommands ? DELETE_BUTTON_ACTIVE_COLOR : DELETE_BUTTON_INACTIVE_COLOR;

        public string DeleteButtonTextColor => CanExecuteClientRelatedCommands ? TEXT_COLOR_ACTIVE : TEXT_COLOR_INACTIVE;

        public string DetailsButtonBackgroundColor => CanExecuteClientRelatedCommands ? DETAILS_BUTTON_ACTIVE_COLOR : DETAILS_BUTTON_INACTIVE_COLOR;

        public string DetailsButtonTextColor => CanExecuteClientRelatedCommands ? TEXT_COLOR_ACTIVE : TEXT_COLOR_INACTIVE;
        #endregion

        #region Constructor
        public ClientViewModel(ClientService clientService, LoginService loginService)
        {
            _clientService = clientService;
            _loginService = loginService;
            var loggedInUser = _loginService.GetLoggedInUser();
            _currentUser = loggedInUser?.UserName ?? "Desconhecido";

            Task.Run(async () =>
            {
                await LoadClientsAsync();
            });
        }
        #endregion

        #region Commands
        [RelayCommand]
        private async Task LoadClientsAsync()
        {
            try
            {
                var clients = await Task.Run(() => _clientService.GetClients());
                UpdateClientsList(clients);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(ex, ERROR_LOADING_CLIENTS);
            }
        }

        [RelayCommand]
        private void OpenAddClientForm()
        {
            NewClient = new Client();
            IsActiveString = string.Empty;
            IsAddClientVisible = true;
            IsDetailVisible = false;
        }

        [RelayCommand]
        private void CloseAddClientForm()
        {
            IsAddClientVisible = false;
            NewClient = new Client();
            IsActiveString = string.Empty;
        }

        [RelayCommand]
        private async Task AddClientAsync()
        {
            try
            {
                if (!ValidateNewClient())
                {
                    await Shell.Current.DisplayAlert("Erro", "Todos os campos devem ser preenchidos.", "OK");
                    return;
                }

                PrepareNewClientForAddition();
                _clientService.AddClient(NewClient);
                Clients.Add(NewClient);
                UpdateClientCounts();
                CloseAddClientForm();
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(ex, ERROR_ADDING_CLIENT);
            }
        }


        [RelayCommand(CanExecute = nameof(CanExecuteClientRelatedCommands))]
        private async Task DeleteClientAsync()
        {
            if (SelectedClient != null)
            {
                try
                {
                    _clientService.DeleteClient(SelectedClient.Id);
                    Clients.Remove(SelectedClient);
                    UpdateClientCounts();
                }
                catch (Exception ex)
                {
                    await HandleExceptionAsync(ex, ERROR_DELETING_CLIENT);
                }
            }
        }

        [RelayCommand]
        private async Task OpenProfileAsync()
        {
            if (SelectedClient != null)
            {
                var navigationParameters = new Dictionary<string, object>
                {
                    { "Client", SelectedClient }
                };
                await Shell.Current.GoToAsync(nameof(ClientProfileView), navigationParameters);
            }
        }


        [RelayCommand]
        private void OpenDetailForm(Client client)
        {
            SelectedClient = client;
            IsActiveString = client.IsActive ? ACTIVE_YES : ACTIVE_NO;
            ReminderString = client.Reminder ? ACTIVE_YES : ACTIVE_NO;
            IsDetailVisible = true;
            IsAddClientVisible = false;
        }

        [RelayCommand]
        private void CloseDetailForm()
        {
            IsDetailVisible = false;
            SelectedClient = null;
        }

        [RelayCommand]
        private async Task SaveDetailAsync()
        {
            try
            {
                if (SelectedClient == null)
                    return;

                UpdateSelectedClientDetails();
                _clientService.UpdateClient(SelectedClient);
                CloseDetailForm();
                await LoadClientsAsync();
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(ex, ERROR_SAVING_CLIENT);
            }
        }

        [RelayCommand]
        private void ToggleSummary()
        {
            if (SelectedClient != null)
            {
                OpenDetailForm(SelectedClient);
            }
            else
            {
                ShowSummary = !ShowSummary;
            }
        }

        [RelayCommand]
        private async Task GenerateReportAsync()
        {
            await Shell.Current.DisplayAlert("Info", REPORT_NOT_IMPLEMENTED, "OK");
        }
        #endregion

        #region Private Methods
        private void UpdateClientsList(IEnumerable<Client> clients)
        {
            Clients.Clear();
            foreach (var client in clients)
            {
                Clients.Add(client);
            }
            UpdateClientCounts();
        }

        private bool ValidateNewClient()
        {
            return !string.IsNullOrEmpty(NewClient.Name) &&
                   !string.IsNullOrEmpty(NewClient.CPF) &&
                   !string.IsNullOrEmpty(NewClient.CellPhone) &&
                   !string.IsNullOrEmpty(NewClient.Email) &&
                   !string.IsNullOrEmpty(NewClient.Street) &&
                   !string.IsNullOrEmpty(NewClient.City) &&
                   !string.IsNullOrEmpty(NewClient.State) &&
                   !string.IsNullOrEmpty(NewClient.ZipCode);
        }

        private void PrepareNewClientForAddition()
        {
            NewClient.IsActive = IsActiveString == ACTIVE_YES;
            NewClient.CreatedAt = DateTime.Now;
            NewClient.CreatedBy = _currentUser;
        }

        private void UpdateSelectedClientDetails()
        {
            if (SelectedClient != null)
            {
                SelectedClient.IsActive = IsActiveString == ACTIVE_YES;
                SelectedClient.Reminder = ReminderString == ACTIVE_YES;
                SelectedClient.UpdatedAt = DateTime.Now;
                SelectedClient.UpdatedBy = _currentUser;
            }
        }

        private async Task HandleExceptionAsync(Exception ex, string errorMessage)
        {
            Console.WriteLine($"Error: {ex.Message}");
            await Shell.Current.DisplayAlert("Erro", errorMessage, "OK");
        }

        private async Task PerformSearchAsync()
        {
            var filteredClients = await Task.Run(() =>
            {
                return _clientService.GetClients()
                    .Where(c => string.IsNullOrWhiteSpace(SearchText) ||
                                c.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                                c.ProcessNumber.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            });

            UpdateClientsList(filteredClients);
        }
        #endregion

        #region Public Methods
        public void UpdateClientCounts()
        {
            OnPropertyChanged(nameof(TotalClients));
            OnPropertyChanged(nameof(TotalActive));
            OnPropertyChanged(nameof(PendingFollowUps));
            OnPropertyChanged(nameof(TodayFollowUps));
        }
        #endregion
    }
}
