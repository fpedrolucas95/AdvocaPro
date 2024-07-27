using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AdvocaPro.Models;
using AdvocaPro.Services;
using System.Collections.ObjectModel;

namespace AdvocaPro.ViewModel
{
    [QueryProperty(nameof(Client), "Client")]
    public partial class ClientProfileViewModel : ObservableObject
    {
        #region Fields
        private readonly ClientService _clientService;
        private readonly LoginService _loginService;
        private readonly string _currentUser;
        #endregion

        #region Constructor
        public ClientProfileViewModel(ClientService clientService, LoginService loginService)
        {
            _clientService = clientService;
            _loginService = loginService;
            var loggedInUser = _loginService.GetLoggedInUser();
            _currentUser = loggedInUser?.UserName ?? "Desconhecido";

            IsActiveOptions = new ObservableCollection<string> { "Sim", "Não" };
            Uf = new ObservableCollection<string>
            {
                "AC","AL","AP","AM","BA","CE","DF","ES","GO","MA","MT","MS","MG","PA","PB","PR","PE",
                "PI","RJ","RN","RS","RO","RR","SC","SP","SE","TO"
            };
            MaritalStatusOptions = new ObservableCollection<string>
            {
                "Solteiro(a)", "Casado(a)", "Separado(a)", "Divorciado(a)", "Viúvo(a)"
            };
        }
        #endregion

        #region Properties
        private Client _client = new();
        public Client Client
        {
            get => _client;
            set => SetProperty(ref _client, value);
        }

        public ObservableCollection<string> IsActiveOptions { get; }
        public ObservableCollection<string> Uf { get; }
        public ObservableCollection<string> MaritalStatusOptions { get; }

        public string IsActiveString
        {
            get => Client.IsActive ? "Sim" : "Não";
            set => Client.IsActive = value == "Sim";
        }
        #endregion

        #region Private Methods
        [RelayCommand]
        private async Task SaveAsync()
        {
            try
            {
                Client.UpdatedBy = _currentUser;
                Client.UpdatedAt = DateTime.Now;

                _clientService.UpdateClient(Client);
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", "Não foi possível salvar o cliente.", "OK");
            }
        }

        [RelayCommand]
        private async Task CancelAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
        #endregion

        #region Public Methods
        public void Initialize(Client client)
        {
            Client = client ?? new Client();
        }
        #endregion
    }
}
