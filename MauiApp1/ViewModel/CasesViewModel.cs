using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AdvocaPro.Models;
using AdvocaPro.Services;
using System.Collections.ObjectModel;

namespace AdvocaPro.ViewModel;

public partial class CasesViewModel : ObservableObject
{
    #region Constants
    private const string ERROR_LOADING_CASES = "Não foi possível carregar os casos.";
    private const string ERROR_ADDING_CASE = "Não foi possível adicionar o caso.";
    private const string ERROR_DELETING_CASE = "Não foi possível excluir o caso.";
    private const string ERROR_SAVING_CASE = "Não foi possível salvar os detalhes do caso.";
    private const string ERROR_LOADING_PROCESS_TYPES = "Não foi possível carregar os tipos de processo.";
    private const string DELETE_BUTTON_ACTIVE_COLOR = "#8c1c13";
    private const string DELETE_BUTTON_INACTIVE_COLOR = "#10FFFFFF";
    private const string DETAILS_BUTTON_ACTIVE_COLOR = "#0582ca";
    private const string DETAILS_BUTTON_INACTIVE_COLOR = "#10FFFFFF";
    private const string TEXT_COLOR_ACTIVE = "#FFFFFF";
    private const string TEXT_COLOR_INACTIVE = "#40FFFFFF";
    #endregion

    #region Fields
    private readonly CaseService _caseService; 
    private readonly ProcessTypeService _processTypeService;
    private readonly ClientService _clientService;
    private readonly LoginService _loginService;
    private readonly string _currentUser;
    #endregion

    #region Properties
    public ObservableCollection<Case> Cases { get; } = new();
    public ObservableCollection<ClientPickerItem> ClientPickerItems { get; } = new();
    public ObservableCollection<string> IsCompletedOptions { get; } = new() { "Sim", "Não" };
    public ObservableCollection<string> ProcessTypes { get; } = new();

    private ClientPickerItem? _selectedClientPickerItem;
    public ClientPickerItem? SelectedClientPickerItem
    {
        get => _selectedClientPickerItem;
        set
        {
            if (SetProperty(ref _selectedClientPickerItem, value) && value != null)
            {
                NewCase.ClientId = value.Id;
            }
        }
    }

    private Case _newCase = new();
    public Case NewCase
    {
        get => _newCase;
        set => SetProperty(ref _newCase, value);
    }

    private Case? _selectedCase;
    public Case? SelectedCase
    {
        get => _selectedCase;
        set
        {
            if (SetProperty(ref _selectedCase, value))
            {
                if (_selectedCase != null)
                {
                    IsCompletedString = _selectedCase.CaseCompleted ? "Sim" : "Não";
                }
                OnPropertyChanged(nameof(CanExecuteCaseRelatedCommands));
                OnPropertyChanged(nameof(DeleteButtonBackgroundColor));
                OnPropertyChanged(nameof(DeleteButtonTextColor));
                OnPropertyChanged(nameof(DetailsButtonBackgroundColor));
                OnPropertyChanged(nameof(DetailsButtonTextColor));
            }
        }
    }

    private bool _isAddCaseVisible;
    public bool IsAddCaseVisible
    {
        get => _isAddCaseVisible;
        set
        {
            if (SetProperty(ref _isAddCaseVisible, value))
            {
                OnPropertyChanged(nameof(IsScrollViewVisible));
                OnPropertyChanged(nameof(DataGridColumnSpan));
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
            }
        }
    }

    private string _isCompletedString = string.Empty;
    public string IsCompletedString
    {
        get => _isCompletedString;
        set => SetProperty(ref _isCompletedString, value);
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

    public bool IsScrollViewVisible => IsAddCaseVisible || IsDetailVisible;
    public int DataGridColumnSpan => IsScrollViewVisible ? 1 : 2;
    public int TotalCases => Cases.Count;
    public int TotalCompleted => Cases.Count(c => c.CaseCompleted);
    public int TotalNotCompleted => Cases.Count(c => !c.CaseCompleted);
    public bool CanExecuteCaseRelatedCommands => SelectedCase != null;
    public string DeleteButtonBackgroundColor => CanExecuteCaseRelatedCommands ? DELETE_BUTTON_ACTIVE_COLOR : DELETE_BUTTON_INACTIVE_COLOR;
    public string DeleteButtonTextColor => CanExecuteCaseRelatedCommands ? TEXT_COLOR_ACTIVE : TEXT_COLOR_INACTIVE;
    public string DetailsButtonBackgroundColor => CanExecuteCaseRelatedCommands ? DETAILS_BUTTON_ACTIVE_COLOR : DETAILS_BUTTON_INACTIVE_COLOR;
    public string DetailsButtonTextColor => CanExecuteCaseRelatedCommands ? TEXT_COLOR_ACTIVE : TEXT_COLOR_INACTIVE;
    #endregion

    #region Constructor
    public CasesViewModel(CaseService caseService, ClientService clientService, LoginService loginService, ProcessTypeService processTypeService)
    {
        _caseService = caseService;
        _clientService = clientService;
        _loginService = loginService; 
        _processTypeService = processTypeService;

        var loggedInUser = _loginService.GetLoggedInUser();
        _currentUser = loggedInUser?.UserName ?? "Desconhecido";

        _clientService.ClientAdded += OnClientAdded;

        Task.Run(async () =>
        {
            await LoadProcessTypesAsync();
            await LoadCasesAsync();
            await LoadClientsAsync();
        });
    }
    #endregion

    #region Commands
    [RelayCommand]
    private async Task LoadCasesAsync()
    {
        try
        {
            var cases = await Task.Run(() => _caseService.GetCases());
            UpdateCasesList(cases);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(ex, ERROR_LOADING_CASES);
        }
    }

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
            await HandleExceptionAsync(ex, ERROR_LOADING_CASES);
        }
    }

    [RelayCommand]
    private async Task LoadProcessTypesAsync()
    {
        try
        {
            var processTypes = await Task.Run(() => _processTypeService.GetProcessTypes().Select(pt => pt.Name));
            UpdateProcessTypesList(processTypes);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(ex, ERROR_LOADING_PROCESS_TYPES);
        }
    }

    [RelayCommand]
    private void OpenAddCaseForm()
    {
        NewCase = new Case { CreatedAt = DateTime.Now };
        IsAddCaseVisible = true;
        IsDetailVisible = false;
    }

    [RelayCommand]
    private void CloseAddCaseForm()
    {
        IsAddCaseVisible = false;
        NewCase = new Case();
    }

    [RelayCommand]
    private async Task AddCaseAsync()
    {
        try
        {
            if (!ValidateNewCase())
            {
                await Shell.Current.DisplayAlert("Erro", "Todos os campos devem ser preenchidos.", "OK");
                return;
            }

            PrepareNewCaseForAddition();
            _caseService.AddCase(NewCase);
            await LoadCasesAsync();
            UpdateCaseCounts();
            CloseAddCaseForm();
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(ex, ERROR_ADDING_CASE);
        }
    }

    [RelayCommand(CanExecute = nameof(CanExecuteCaseRelatedCommands))]
    private async Task DeleteCaseAsync()
    {
        if (SelectedCase != null)
        {
            try
            {
                _caseService.DeleteCase(SelectedCase.Id);
                Cases.Remove(SelectedCase);
                UpdateCaseCounts();
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(ex, ERROR_DELETING_CASE);
            }
        }
    }

    [RelayCommand]
    private void OpenDetailForm(Case caseRecord)
    {
        SelectedCase = caseRecord;
        IsCompletedString = caseRecord.CaseCompleted ? "Sim" : "Não";
        IsDetailVisible = true;
        IsAddCaseVisible = false;
    }

    [RelayCommand]
    private void CloseDetailForm()
    {
        IsDetailVisible = false;
        SelectedCase = null;
    }

    [RelayCommand]
    private async Task SaveDetailAsync()
    {
        try
        {
            if (SelectedCase == null)
                return;

            UpdateSelectedCaseDetails();
            _caseService.UpdateCase(SelectedCase);
            CloseDetailForm();
            await LoadCasesAsync();
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(ex, ERROR_SAVING_CASE);
        }
    }

    [RelayCommand]
    private void ToggleSummary()
    {
        if (SelectedCase != null)
        {
            OpenDetailForm(SelectedCase);
        }
        else
        {
            CloseDetailForm();
        }
    }
    #endregion

    #region Private Methods
    private void OnClientAdded(object sender, Client client)
    {
        ClientPickerItems.Add(new ClientPickerItem { Id = client.Id, Name = client.Name });
    }

    private void UpdateCasesList(IEnumerable<Case> cases)
    {
        Cases.Clear();
        foreach (var caseRecord in cases)
        {
            Cases.Add(caseRecord);
        }
        UpdateCaseCounts();
    }

    private void UpdateClientsList(IEnumerable<Client> clients)
    {
        ClientPickerItems.Clear();
        foreach (var client in clients)
        {
            ClientPickerItems.Add(new ClientPickerItem { Id = client.Id, Name = client.Name });
        }
    }

    private void UpdateProcessTypesList(IEnumerable<string> processTypes)
    {
        ProcessTypes.Clear();
        foreach (var processType in processTypes)
        {
            ProcessTypes.Add(processType);
        }
    }

    private bool ValidateNewCase()
    {
        return NewCase.ClientId > 0 &&
               !string.IsNullOrEmpty(NewCase.Court) &&
               !string.IsNullOrEmpty(NewCase.Opponent);
    }

    private void PrepareNewCaseForAddition()
    {
        NewCase.CreatedAt = DateTime.Now;
        NewCase.CreatedBy = _currentUser;
    }

    private void UpdateSelectedCaseDetails()
    {
        if (SelectedCase != null)
        {
            SelectedCase.CaseCompleted = IsCompletedString == "Sim";
            SelectedCase.UpdatedAt = DateTime.Now;
            SelectedCase.UpdatedBy = _currentUser;
        }
    }

    private void UpdateCaseCounts()
    {
        OnPropertyChanged(nameof(TotalCases));
        OnPropertyChanged(nameof(TotalCompleted));
        OnPropertyChanged(nameof(TotalNotCompleted));
    }

    private async Task HandleExceptionAsync(Exception ex, string errorMessage)
    {
        Console.WriteLine($"Error: {ex.Message}");
        await Shell.Current.DisplayAlert("Erro", errorMessage, "OK");
    }

    private async Task PerformSearchAsync()
    {
        var filteredCases = await Task.Run(() =>
        {
            return _caseService.GetCases()
                .Where(c => string.IsNullOrWhiteSpace(SearchText) ||
                            (c.Client != null &&
                            (c.Client.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                            c.Client.ProcessNumber.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                            c.Opponent.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                            c.Court.Contains(SearchText, StringComparison.OrdinalIgnoreCase))))
                .ToList();
        });

        UpdateCasesList(filteredCases);
    }
    #endregion
}
