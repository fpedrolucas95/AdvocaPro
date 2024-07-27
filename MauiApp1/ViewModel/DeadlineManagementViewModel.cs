using AdvocaPro.Models;
using AdvocaPro.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace AdvocaPro.ViewModel;

public partial class DeadlineManagementViewModel : ObservableObject
{
    #region Constants
    private const string ERROR_LOADING_DEADLINES = "Não foi possível carregar os prazos.";
    private const string ERROR_ADDING_DEADLINE = "Não foi possível adicionar o prazo.";
    private const string ERROR_DELETING_DEADLINE = "Não foi possível excluir o prazo.";
    private const string ERROR_SAVING_DEADLINE = "Não foi possível salvar os detalhes do prazo.";
    private const string REPORT_NOT_IMPLEMENTED = "Funcionalidade não implementada";
    private const string BUSINESS_DAYS_YES = "Sim";
    private const string BUSINESS_DAYS_NO = "Não";
    private const string DELETE_BUTTON_ACTIVE_COLOR = "#8c1c13";
    private const string DELETE_BUTTON_INACTIVE_COLOR = "#10FFFFFF";
    private const string DETAILS_BUTTON_ACTIVE_COLOR = "#0582ca";
    private const string DETAILS_BUTTON_INACTIVE_COLOR = "#10FFFFFF";
    private const string TEXT_COLOR_ACTIVE = "#FFFFFF";
    private const string TEXT_COLOR_INACTIVE = "#40FFFFFF";
    #endregion

    #region Fields
    private readonly DeadlineService _deadlineService;
    private readonly ClientService _clientService;
    private readonly LoginService _loginService;
    private readonly string _currentUser;
    #endregion

    #region Properties
    public ObservableCollection<Deadline> Deadlines { get; } = new();
    public ObservableCollection<Client> Clients { get; } = new();
    public ObservableCollection<ClientPickerItem> ClientPickerItems { get; } = new();
    public ObservableCollection<string> IsBusinessDaysOptions { get; } = new() { BUSINESS_DAYS_YES, BUSINESS_DAYS_NO };
    public ObservableCollection<string> IsCompletedOptions { get; } = new() { BUSINESS_DAYS_YES, BUSINESS_DAYS_NO };

    private ClientPickerItem? _selectedClientPickerItem;
    public ClientPickerItem? SelectedClientPickerItem
    {
        get => _selectedClientPickerItem;
        set
        {
            if (SetProperty(ref _selectedClientPickerItem, value) && value != null)
            {
                NewDeadline.ClientId = value.Id;
            }
        }
    }

    private Deadline _newDeadline = new();
    public Deadline NewDeadline
    {
        get => _newDeadline;
        set => SetProperty(ref _newDeadline, value);
    }

    private Deadline? _selectedDeadline;
    public Deadline? SelectedDeadline
    {
        get => _selectedDeadline;
        set
        {
            if (SetProperty(ref _selectedDeadline, value))
            {
                if (_selectedDeadline != null)
                {
                    _selectedDeadline.EndDate = _selectedDeadline.IsBusinessDays
                        ? CalculateBusinessDays(_selectedDeadline.StartDate, _selectedDeadline.DurationDays)
                        : _selectedDeadline.StartDate.AddDays(_selectedDeadline.DurationDays);
                    IsCompletedString = _selectedDeadline.IsCompleted ? BUSINESS_DAYS_YES : BUSINESS_DAYS_NO;
                    UpdateDeadlineStatus(_selectedDeadline);
                }
                OnPropertyChanged(nameof(CanExecuteDeadlineRelatedCommands));
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

    private bool _isAddDeadlineVisible;
    public bool IsAddDeadlineVisible
    {
        get => _isAddDeadlineVisible;
        set
        {
            if (SetProperty(ref _isAddDeadlineVisible, value))
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

    private string _isBusinessDaysString = string.Empty;
    public string IsBusinessDaysString
    {
        get => _isBusinessDaysString;
        set => SetProperty(ref _isBusinessDaysString, value);
    }

    private string _isCompletedString = string.Empty;
    public string IsCompletedString
    {
        get => _isCompletedString;
        set => SetProperty(ref _isCompletedString, value);
    }

    public bool IsScrollViewVisible => IsAddDeadlineVisible || IsDetailVisible;
    public int DataGridColumnSpan => IsScrollViewVisible ? 1 : 2;
    public int TotalDeadlines => Deadlines.Count;
    public int TotalCompleted => Deadlines.Count(d => d.IsCompleted);
    public int TotalDueToday => Deadlines.Count(d => d.EndDate.Date == DateTime.Today.Date);
    public int TotalDueThisWeek => Deadlines.Count(d => IsDateInCurrentWeek(d.EndDate));
    public int TotalOverdue => Deadlines.Count(d => d.EndDate.Date < DateTime.Today.Date && !d.IsCompleted);
    public bool CanExecuteDeadlineRelatedCommands => SelectedDeadline != null;
    public string DeleteButtonBackgroundColor => CanExecuteDeadlineRelatedCommands ? DELETE_BUTTON_ACTIVE_COLOR : DELETE_BUTTON_INACTIVE_COLOR;
    public string DeleteButtonTextColor => CanExecuteDeadlineRelatedCommands ? TEXT_COLOR_ACTIVE : TEXT_COLOR_INACTIVE;
    public string DetailsButtonBackgroundColor => CanExecuteDeadlineRelatedCommands ? DETAILS_BUTTON_ACTIVE_COLOR : DETAILS_BUTTON_INACTIVE_COLOR;
    public string DetailsButtonTextColor => CanExecuteDeadlineRelatedCommands ? TEXT_COLOR_ACTIVE : TEXT_COLOR_INACTIVE;
    #endregion

    #region Constructor
    public DeadlineManagementViewModel(DeadlineService deadlineService, ClientService clientService, LoginService loginService)
    {
        _deadlineService = deadlineService;
        _clientService = clientService;
        _loginService = loginService;
        var loggedInUser = _loginService.GetLoggedInUser();
        _currentUser = loggedInUser?.UserName ?? "Desconhecido";

        Task.Run(async () =>
        {
            await LoadDeadlinesAsync();
            await LoadClientsAsync();
        });
    }
    #endregion

    #region Commands
    [RelayCommand]
    private async Task LoadDeadlinesAsync()
    {
        try
        {
            var deadlines = await Task.Run(() => _deadlineService.GetDeadlines());
            UpdateDeadlinesList(deadlines);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(ex, ERROR_LOADING_DEADLINES);
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
            await HandleExceptionAsync(ex, ERROR_LOADING_DEADLINES);
        }
    }

    [RelayCommand]
    private void OpenAddDeadlineForm()
    {
        NewDeadline = new Deadline { StartDate = DateTime.Today };
        IsBusinessDaysString = BUSINESS_DAYS_NO;
        IsAddDeadlineVisible = true;
        IsDetailVisible = false;
    }

    [RelayCommand]
    private void CloseAddDeadlineForm()
    {
        IsAddDeadlineVisible = false;
        NewDeadline = new Deadline();
        IsBusinessDaysString = string.Empty;
    }

    [RelayCommand]
    private async Task AddDeadlineAsync()
    {
        try
        {
            if (!ValidateNewDeadline())
            {
                await Shell.Current.DisplayAlert("Erro", "Todos os campos devem ser preenchidos.", "OK");
                return;
            }

            PrepareNewDeadlineForAddition();
            _deadlineService.AddDeadline(NewDeadline);
            await LoadDeadlinesAsync();
            UpdateDeadlineCounts();
            CloseAddDeadlineForm();
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(ex, ERROR_ADDING_DEADLINE);
        }
    }

    [RelayCommand(CanExecute = nameof(CanExecuteDeadlineRelatedCommands))]
    private async Task DeleteDeadlineAsync()
    {
        if (SelectedDeadline != null)
        {
            try
            {
                _deadlineService.DeleteDeadline(SelectedDeadline.Id);
                Deadlines.Remove(SelectedDeadline);
                UpdateDeadlineCounts();
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(ex, ERROR_DELETING_DEADLINE);
            }
        }
    }

    [RelayCommand]
    private void OpenDetailForm(Deadline deadline)
    {
        SelectedDeadline = deadline;
        IsBusinessDaysString = deadline.IsBusinessDays ? BUSINESS_DAYS_YES : BUSINESS_DAYS_NO;
        IsCompletedString = deadline.IsCompleted ? BUSINESS_DAYS_YES : BUSINESS_DAYS_NO;
        IsDetailVisible = true;
        IsAddDeadlineVisible = false;
    }

    [RelayCommand]
    private void CloseDetailForm()
    {
        IsDetailVisible = false;
        SelectedDeadline = null;
    }

    [RelayCommand]
    private async Task SaveDetailAsync()
    {
        try
        {
            if (SelectedDeadline == null)
                return;

            UpdateSelectedDeadlineDetails();
            _deadlineService.UpdateDeadline(SelectedDeadline);
            CloseDetailForm();
            await LoadDeadlinesAsync();
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(ex, ERROR_SAVING_DEADLINE);
        }
    }

    [RelayCommand]
    private void ToggleSummary()
    {
        if (SelectedDeadline != null)
        {
            OpenDetailForm(SelectedDeadline);
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
    private void UpdateDeadlinesList(IEnumerable<Deadline> deadlines)
    {
        Deadlines.Clear();
        foreach (var deadline in deadlines)
        {
            UpdateDeadlineStatus(deadline);
            Deadlines.Add(deadline);
        }
        UpdateDeadlineCounts();
    }

    private void UpdateClientsList(IEnumerable<Client> clients)
    {
        Clients.Clear();
        ClientPickerItems.Clear();
        foreach (var client in clients)
        {
            Clients.Add(client);
            ClientPickerItems.Add(new ClientPickerItem { Id = client.Id, Name = client.Name });
        }
    }

    private bool ValidateNewDeadline()
    {
        return NewDeadline.ClientId > 0 &&
               NewDeadline.StartDate != DateTime.MinValue &&
               NewDeadline.DurationDays > 0 &&
               !string.IsNullOrEmpty(IsBusinessDaysString);
    }

    private void PrepareNewDeadlineForAddition()
    {
        NewDeadline.IsBusinessDays = IsBusinessDaysString == BUSINESS_DAYS_YES;
        NewDeadline.EndDate = NewDeadline.IsBusinessDays
            ? CalculateBusinessDays(NewDeadline.StartDate, NewDeadline.DurationDays)
            : NewDeadline.StartDate.AddDays(NewDeadline.DurationDays);
        NewDeadline.CreatedAt = DateTime.Now;
        NewDeadline.CreatedBy = _currentUser;
        UpdateDeadlineStatus(NewDeadline);
    }

    private void UpdateSelectedDeadlineDetails()
    {
        if (SelectedDeadline != null)
        {
            SelectedDeadline.IsCompleted = IsCompletedString == BUSINESS_DAYS_YES;
            SelectedDeadline.UpdatedAt = DateTime.Now;
            SelectedDeadline.UpdatedBy = _currentUser;
            SelectedDeadline.EndDate = SelectedDeadline.IsBusinessDays
                ? CalculateBusinessDays(SelectedDeadline.StartDate, SelectedDeadline.DurationDays)
                : SelectedDeadline.StartDate.AddDays(SelectedDeadline.DurationDays);
            UpdateDeadlineStatus(SelectedDeadline);
        }
    }

    private void UpdateDeadlineCounts()
    {
        OnPropertyChanged(nameof(TotalDeadlines));
        OnPropertyChanged(nameof(TotalCompleted));
        OnPropertyChanged(nameof(TotalDueToday));
        OnPropertyChanged(nameof(TotalDueThisWeek));
        OnPropertyChanged(nameof(TotalOverdue));
    }

    private async Task HandleExceptionAsync(Exception ex, string errorMessage)
    {
        Console.WriteLine($"Error: {ex.Message}");
        await Shell.Current.DisplayAlert("Erro", errorMessage, "OK");
    }

    private async Task PerformSearchAsync()
    {
        var filteredDeadlines = await Task.Run(() =>
        {
            return _deadlineService.GetDeadlines()
                .Where(d => string.IsNullOrWhiteSpace(SearchText) ||
                            (d.Client != null &&
                            (d.Client.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                            d.Client.ProcessNumber.Contains(SearchText, StringComparison.OrdinalIgnoreCase))))
                .ToList();
        });

        UpdateDeadlinesList(filteredDeadlines);
    }

    private DateTime CalculateBusinessDays(DateTime startDate, int durationDays)
    {
        DateTime endDate = startDate;
        int businessDaysCount = 0;

        while (businessDaysCount < durationDays)
        {
            endDate = endDate.AddDays(1);
            if (endDate.DayOfWeek != DayOfWeek.Saturday && endDate.DayOfWeek != DayOfWeek.Sunday)
            {
                businessDaysCount++;
            }
        }

        return endDate;
    }

    private bool IsDateInCurrentWeek(DateTime date)
    {
        DateTime startOfWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
        DateTime endOfWeek = startOfWeek.AddDays(7).AddSeconds(-1);
        return date >= startOfWeek && date <= endOfWeek;
    }

    private void UpdateDeadlineStatus(Deadline deadline)
    {
        if (deadline.IsCompleted)
        {
            deadline.Status = "Concluído";
        }
        else if (DateTime.Now > deadline.EndDate)
        {
            deadline.Status = $"Atrasado a {(DateTime.Now - deadline.EndDate).Days} dias";
        }
        else if (DateTime.Now.Date == deadline.EndDate.Date)
        {
            deadline.Status = "Vence hoje";
        }
        else
        {
            deadline.Status = deadline.IsBusinessDays
                ? $"{CalculateRemainingBusinessDays(DateTime.Now, deadline.EndDate)} dias úteis restantes"
                : $"{(deadline.EndDate - DateTime.Now).Days} dias restantes";
        }
    }

    private int CalculateRemainingBusinessDays(DateTime startDate, DateTime endDate)
    {
        int remainingDays = 0;
        DateTime currentDate = startDate;

        while (currentDate < endDate)
        {
            if (currentDate.DayOfWeek != DayOfWeek.Saturday && currentDate.DayOfWeek != DayOfWeek.Sunday)
            {
                remainingDays++;
            }
            currentDate = currentDate.AddDays(1);
        }

        return remainingDays;
    }
    #endregion
}
