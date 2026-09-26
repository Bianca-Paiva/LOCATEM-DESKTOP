using System.Collections.ObjectModel;
using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Windows.Input;
using LOCATEM_DESKTOP.Helpers;
using LOCATEM_DESKTOP.Helpers.Auth;
using LOCATEM_DESKTOP.Helpers.Conta;
using LOCATEM_DESKTOP.Models.Auth;
using LOCATEM_DESKTOP.Services.Auth;
using LOCATEM_DESKTOP.Validation.Auth;
using LOCATEM_DESKTOP.Validation.Perfil;
using LOCATEM_DESKTOP.ViewModels.Base;
using MauiIcons.Material;

namespace LOCATEM_DESKTOP.ViewModels.Conta
{
    /// <summary>
    /// Migração da tela Perfil do React (Perfil.tsx + hooks de Perfil). Concentra dados da sessão,
    /// completude, edição, upload de foto, consulta de CEP, logout e navegação.
    /// </summary>
    public class PerfilViewModel : BaseViewModel
    {
        private const string ApiBase = "http://localhost:5033";

        // Apenas rotas realmente existentes neste estágio do MAUI. Os cards correspondentes a
        // páginas ainda não migradas permanecem visíveis, porém inativos, como no React quando
        // uma opção de Painel de Controle não possui route.
        private static readonly HashSet<string> RotasMigradas =
            new(StringComparer.OrdinalIgnoreCase)
            {
                "login",
                "homeLocador",
                "minhasFerramentas",
                "gerenciarLocacoes",
                "historicoLocacoes",
                "perfil"
            };

        private readonly IAuthSessionService _authSession;
        private readonly IAuthService _authService;
        private readonly HttpClient _cepClient = new();

        private Usuario? _usuario;
        private FileResult? _fotoSelecionada;
        private string? _fotoLocalPreviewPath;
        private PerfilFieldErrors _campoErros = new();
        private CancellationTokenSource? _ocultarCompletudeCts;

        public PerfilViewModel(
            IAuthSessionService authSession,
            IAuthService authService)
        {
            _authSession = authSession;
            _authService = authService;

            NavegarCommand = new AsyncRelayCommand(
                async parametro => await NavegarAsync(parametro as string));

            EditarPerfilCommand = new RelayCommand(AbrirEdicao);
            CancelarEdicaoCommand = new RelayCommand(FecharEdicao);
            SalvarPerfilCommand = new AsyncRelayCommand(SalvarPerfilAsync);
            SelecionarFotoCommand = new AsyncRelayCommand(SelecionarFotoAsync);
            BuscarCepCommand = new AsyncRelayCommand(BuscarCepAsync);
            AbrirBuscaCepCommand = new AsyncRelayCommand(async () =>
                await Launcher.Default.OpenAsync("https://buscacepinter.correios.com.br/app/endereco/index.php"));
            FecharAlertaCommand = new RelayCommand(() => Alerta = null);

            SolicitarLogoutCommand = new RelayCommand(() => IsConfirmandoLogout = true);
            CancelarLogoutCommand = new RelayCommand(() => IsConfirmandoLogout = false);
            ConfirmarLogoutCommand = new AsyncRelayCommand(ConfirmarLogoutAsync);

            AbrirOpcaoPainelCommand = new AsyncRelayCommand(
                async parametro =>
                {
                    if (parametro is PainelPerfilItem item && item.Disponivel)
                        await NavegarAsync(item.Rota);
                });

            MontarPainelControle();
        }

        public ObservableCollection<PainelPerfilItem> OpcoesPainel { get; } = new();

        // ===================== COMMANDS =====================

        public ICommand NavegarCommand { get; }
        public ICommand EditarPerfilCommand { get; }
        public ICommand CancelarEdicaoCommand { get; }
        public ICommand SalvarPerfilCommand { get; }
        public ICommand SelecionarFotoCommand { get; }
        public ICommand BuscarCepCommand { get; }
        public ICommand AbrirBuscaCepCommand { get; }
        public ICommand FecharAlertaCommand { get; }
        public ICommand SolicitarLogoutCommand { get; }
        public ICommand CancelarLogoutCommand { get; }
        public ICommand ConfirmarLogoutCommand { get; }
        public ICommand AbrirOpcaoPainelCommand { get; }

        // ===================== DADOS EXIBIDOS =====================

        private string _nomeUsuario = string.Empty;
        public string NomeUsuario
        {
            get => _nomeUsuario;
            private set => SetProperty(ref _nomeUsuario, value);
        }

        private string _avatarIniciais = "?";
        public string AvatarIniciais
        {
            get => _avatarIniciais;
            private set => SetProperty(ref _avatarIniciais, value);
        }

        private ImageSource? _fotoPerfilSource;
        public ImageSource? FotoPerfilSource
        {
            get => _fotoPerfilSource;
            private set => SetProperty(ref _fotoPerfilSource, value);
        }

        private bool _temFotoPerfil;
        public bool TemFotoPerfil
        {
            get => _temFotoPerfil;
            private set
            {
                if (SetProperty(ref _temFotoPerfil, value))
                    OnPropertyChanged(nameof(SemFotoPerfil));
            }
        }

        public bool SemFotoPerfil => !TemFotoPerfil;

        private bool _ehLocador;
        public bool EhLocador
        {
            get => _ehLocador;
            private set
            {
                if (SetProperty(ref _ehLocador, value))
                    OnPropertyChanged(nameof(EhLocatario));
            }
        }

        public bool EhLocatario => !EhLocador;

        private string _tipoBadgeTexto = string.Empty;
        public string TipoBadgeTexto
        {
            get => _tipoBadgeTexto;
            private set => SetProperty(ref _tipoBadgeTexto, value);
        }

        private string _documentoRotulo = "CNPJ";
        public string DocumentoRotulo
        {
            get => _documentoRotulo;
            private set => SetProperty(ref _documentoRotulo, value);
        }

        public string DocumentoPlaceholder => EhLocador ? "00.000.000/0000-00" : "000.000.000-00";
        public int DocumentoMaxLength => EhLocador ? 18 : 14;

        private string _documentoExibicao = string.Empty;
        public string DocumentoExibicao
        {
            get => _documentoExibicao;
            private set => SetProperty(ref _documentoExibicao, value);
        }

        private string _emailExibicao = string.Empty;
        public string EmailExibicao
        {
            get => _emailExibicao;
            private set => SetProperty(ref _emailExibicao, value);
        }

        private string _telefoneExibicao = string.Empty;
        public string TelefoneExibicao
        {
            get => _telefoneExibicao;
            private set => SetProperty(ref _telefoneExibicao, value);
        }

        private string _enderecoExibicao = string.Empty;
        public string EnderecoExibicao
        {
            get => _enderecoExibicao;
            private set => SetProperty(ref _enderecoExibicao, value);
        }

        private double _reputacaoNota;
        public double ReputacaoNota
        {
            get => _reputacaoNota;
            private set => SetProperty(ref _reputacaoNota, value);
        }

        private string _reputacaoNotaTexto = "0.0";
        public string ReputacaoNotaTexto
        {
            get => _reputacaoNotaTexto;
            private set => SetProperty(ref _reputacaoNotaTexto, value);
        }

        private string _avaliacoesTexto = "0 avaliações";
        public string AvaliacoesTexto
        {
            get => _avaliacoesTexto;
            private set => SetProperty(ref _avaliacoesTexto, value);
        }

        private string _locacoesTexto = "0 locações";
        public string LocacoesTexto
        {
            get => _locacoesTexto;
            private set => SetProperty(ref _locacoesTexto, value);
        }

        private string _entregasNoPrazoTexto = string.Empty;
        public string EntregasNoPrazoTexto
        {
            get => _entregasNoPrazoTexto;
            private set => SetProperty(ref _entregasNoPrazoTexto, value);
        }

        private bool _mostrarEntregasNoPrazo;
        public bool MostrarEntregasNoPrazo
        {
            get => _mostrarEntregasNoPrazo;
            private set => SetProperty(ref _mostrarEntregasNoPrazo, value);
        }

        // ===================== COMPLETUDE =====================

        private int _percentualPerfil;
        public int PercentualPerfil
        {
            get => _percentualPerfil;
            private set
            {
                if (SetProperty(ref _percentualPerfil, value))
                {
                    OnPropertyChanged(nameof(PercentualTexto));
                    OnPropertyChanged(nameof(ProgressoPerfil));
                }
            }
        }

        public string PercentualTexto => $"{PercentualPerfil}% concluído";
        public double ProgressoPerfil => PercentualPerfil / 100d;

        private string _mensagemCompletude = string.Empty;
        public string MensagemCompletude
        {
            get => _mensagemCompletude;
            private set => SetProperty(ref _mensagemCompletude, value);
        }

        private bool _mostrarCompletudePerfil = true;
        public bool MostrarCompletudePerfil
        {
            get => _mostrarCompletudePerfil;
            private set => SetProperty(ref _mostrarCompletudePerfil, value);
        }

        // ===================== MODAIS =====================

        private bool _isEditando;
        public bool IsEditando
        {
            get => _isEditando;
            private set => SetProperty(ref _isEditando, value);
        }

        private bool _isConfirmandoLogout;
        public bool IsConfirmandoLogout
        {
            get => _isConfirmandoLogout;
            private set => SetProperty(ref _isConfirmandoLogout, value);
        }

        // ===================== FORMULÁRIO DE EDIÇÃO =====================

        private string _nomeEdicao = string.Empty;
        public string NomeEdicao
        {
            get => _nomeEdicao;
            set
            {
                if (SetProperty(ref _nomeEdicao, value ?? string.Empty))
                {
                    NomeError.Clear();
                    _campoErros.Nome = null;
                    OnPropertyChanged(nameof(NomeErrorText));
                }
            }
        }

        private string _telefoneEdicao = string.Empty;
        public string TelefoneEdicao
        {
            get => _telefoneEdicao;
            set
            {
                var mascarado = MaskHelper.MaskPhone(value ?? string.Empty);
                if (SetProperty(ref _telefoneEdicao, mascarado))
                {
                    TelefoneError.Clear();
                    _campoErros.Telefone = null;
                    OnPropertyChanged(nameof(TelefoneErrorText));
                }
                else if (value != mascarado)
                {
                    OnPropertyChanged(nameof(TelefoneEdicao));
                }
            }
        }

        private string _documentoEdicao = string.Empty;
        public string DocumentoEdicao
        {
            get => _documentoEdicao;
            set
            {
                var mascarado = EhLocador
                    ? MaskHelper.MaskCnpj(value ?? string.Empty)
                    : MaskHelper.MaskCpf(value ?? string.Empty);

                if (SetProperty(ref _documentoEdicao, mascarado))
                {
                    DocumentoError.Clear();
                    _campoErros.Documento = null;
                    OnPropertyChanged(nameof(DocumentoErrorText));
                }
                else if (value != mascarado)
                {
                    OnPropertyChanged(nameof(DocumentoEdicao));
                }
            }
        }

        private string _cepEdicao = string.Empty;
        public string CepEdicao
        {
            get => _cepEdicao;
            set
            {
                var mascarado = MaskHelper.MaskCep(value ?? string.Empty);
                if (SetProperty(ref _cepEdicao, mascarado))
                {
                    CepError.Clear();
                    _campoErros.Cep = null;
                    OnPropertyChanged(nameof(CepErrorText));
                }
                else if (value != mascarado)
                {
                    OnPropertyChanged(nameof(CepEdicao));
                }
            }
        }

        private string _logradouroEdicao = string.Empty;
        public string LogradouroEdicao
        {
            get => _logradouroEdicao;
            set
            {
                if (SetProperty(ref _logradouroEdicao, value ?? string.Empty))
                {
                    LogradouroError.Clear();
                    _campoErros.Logradouro = null;
                    OnPropertyChanged(nameof(LogradouroErrorText));
                }
            }
        }

        private string _numeroEdicao = string.Empty;
        public string NumeroEdicao
        {
            get => _numeroEdicao;
            set
            {
                if (SetProperty(ref _numeroEdicao, value ?? string.Empty))
                {
                    NumeroError.Clear();
                    _campoErros.Numero = null;
                    OnPropertyChanged(nameof(NumeroErrorText));
                }
            }
        }

        private string _enderecoResumo = string.Empty;
        public string EnderecoResumo
        {
            get => _enderecoResumo;
            private set
            {
                if (SetProperty(ref _enderecoResumo, value))
                    OnPropertyChanged(nameof(TemEnderecoResumo));
            }
        }

        public bool TemEnderecoResumo => !string.IsNullOrWhiteSpace(EnderecoResumo);

        private ImageSource? _fotoEdicaoSource;
        public ImageSource? FotoEdicaoSource
        {
            get => _fotoEdicaoSource;
            private set => SetProperty(ref _fotoEdicaoSource, value);
        }

        private bool _temFotoEdicao;
        public bool TemFotoEdicao
        {
            get => _temFotoEdicao;
            private set
            {
                if (SetProperty(ref _temFotoEdicao, value))
                    OnPropertyChanged(nameof(SemFotoEdicao));
            }
        }

        public bool SemFotoEdicao => !TemFotoEdicao;

        public FieldErrorState NomeError { get; } = new();
        public FieldErrorState TelefoneError { get; } = new();
        public FieldErrorState DocumentoError { get; } = new();
        public FieldErrorState CepError { get; } = new();
        public FieldErrorState LogradouroError { get; } = new();
        public FieldErrorState NumeroError { get; } = new();

        public string NomeErrorText => _campoErros.Nome ?? string.Empty;
        public string TelefoneErrorText => _campoErros.Telefone ?? string.Empty;
        public string DocumentoErrorText => _campoErros.Documento ?? string.Empty;
        public string CepErrorText => _campoErros.Cep ?? string.Empty;
        public string LogradouroErrorText => _campoErros.Logradouro ?? string.Empty;
        public string NumeroErrorText => _campoErros.Numero ?? string.Empty;

        // ===================== ALERTA =====================

        private AlertaMessage? _alerta;
        public AlertaMessage? Alerta
        {
            get => _alerta;
            private set
            {
                if (SetProperty(ref _alerta, value))
                {
                    OnPropertyChanged(nameof(TemAlerta));
                    OnPropertyChanged(nameof(AlertaTitulo));
                    OnPropertyChanged(nameof(AlertaMensagem));
                }
            }
        }

        public bool TemAlerta => Alerta is not null;
        public string AlertaTitulo => Alerta?.Titulo ?? string.Empty;
        public string AlertaMensagem => Alerta?.Mensagem ?? string.Empty;

        // ===================== CICLO DA PÁGINA =====================

        public async Task<bool> GarantirAcessoAsync()
        {
            if (_authSession.IsAuthenticated)
                return true;

            await Shell.Current.GoToAsync("//login");
            return false;
        }

        public void Carregar()
        {
            var usuario = _authSession.UsuarioAtual;
            if (usuario is null)
                return;

            _usuario = usuario;
            AtualizarDadosExibidos(usuario);
            RecalcularCompletude(usuario);
        }

        private void AtualizarDadosExibidos(Usuario usuario)
        {
            EhLocador = usuario.Tipo == TipoUsuario.Locador;
            MontarPainelControle();
            OnPropertyChanged(nameof(DocumentoPlaceholder));
            OnPropertyChanged(nameof(DocumentoMaxLength));

            NomeUsuario = usuario.Nome;
            AvatarIniciais = AvatarHelper.ExtrairIniciais(usuario.Nome);
            DocumentoRotulo = EhLocador ? "CNPJ" : "CPF";
            TipoBadgeTexto = $"{(EhLocador ? "Locador" : "Locatário")} desde {usuario.Desde}";

            DocumentoExibicao = ValorOuNaoInformado(usuario.Documento);
            EmailExibicao = string.IsNullOrWhiteSpace(usuario.Email) ? "—" : usuario.Email;
            TelefoneExibicao = ValorOuNaoInformado(usuario.Telefone);
            EnderecoExibicao = ValorOuNaoInformado(usuario.Endereco);

            FotoPerfilSource = AvatarHelper.CriarImageSource(usuario.FotoUrl, ApiBase);
            TemFotoPerfil = FotoPerfilSource is not null;

            var reputacao = usuario.Reputacao ?? new ReputacaoUsuario();
            ReputacaoNota = reputacao.Rating;
            ReputacaoNotaTexto = reputacao.Rating.ToString("0.0", CultureInfo.InvariantCulture);
            AvaliacoesTexto = $"{reputacao.TotalAvaliacoes} avaliações";
            LocacoesTexto = EhLocador
                ? $"{reputacao.LocacoesConcluidas} locações concluídas"
                : $"{reputacao.LocacoesConcluidas} locações";

            MostrarEntregasNoPrazo =
                EhLocador && reputacao.EntregasNoPrazoPercentual.HasValue;

            EntregasNoPrazoTexto = MostrarEntregasNoPrazo
                ? $"{reputacao.EntregasNoPrazoPercentual:0.#}% entregas no prazo"
                : string.Empty;
        }

        // ===================== COMPLETUDE =====================

        private void RecalcularCompletude(Usuario usuario)
        {
            var criterios = new (int Peso, string Acao, bool Atendido)[]
            {
                (17, "complete seu nome", !string.IsNullOrWhiteSpace(usuario.Nome)),
                (17, "informe seu e-mail", !string.IsNullOrWhiteSpace(usuario.Email)),
                (17, "informe seu telefone", !string.IsNullOrWhiteSpace(usuario.Telefone)),
                (17, "informe seu documento", !string.IsNullOrWhiteSpace(usuario.Documento)),
                (17, "informe seu endereço", !string.IsNullOrWhiteSpace(usuario.Endereco)),
                (15, "adicione uma foto", !string.IsNullOrWhiteSpace(usuario.FotoUrl))
            };

            PercentualPerfil = criterios
                .Where(c => c.Atendido)
                .Sum(c => c.Peso);

            var faltando = criterios
                .Where(c => !c.Atendido)
                .Take(2)
                .Select(c => c.Acao)
                .ToArray();

            if (faltando.Length == 0)
            {
                MensagemCompletude = "Seu perfil está completo!";
            }
            else
            {
                var frase = faltando.Length == 2
                    ? $"{faltando[0]} e {faltando[1]}"
                    : faltando[0];

                MensagemCompletude =
                    $"{char.ToUpper(frase[0], CultureInfo.CurrentCulture)}{frase[1..]} para chegar a 100%.";
            }

            _ocultarCompletudeCts?.Cancel();
            MostrarCompletudePerfil = true;

            if (PercentualPerfil == 100)
            {
                _ocultarCompletudeCts = new CancellationTokenSource();
                _ = OcultarCompletudeDepoisAsync(_ocultarCompletudeCts.Token);
            }
        }

        private async Task OcultarCompletudeDepoisAsync(CancellationToken token)
        {
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(3), token);
                if (!token.IsCancellationRequested)
                    MostrarCompletudePerfil = false;
            }
            catch (TaskCanceledException)
            {
                // Novo cálculo substituiu o timer anterior.
            }
        }

        // ===================== EDIÇÃO =====================

        private void AbrirEdicao()
        {
            if (_usuario is null)
                return;

            ResetarErros();
            Alerta = null;
            _fotoSelecionada = null;
            _fotoLocalPreviewPath = null;

            NomeEdicao = _usuario.Nome;
            TelefoneEdicao = _usuario.Telefone;
            DocumentoEdicao = _usuario.Documento;

            PreencherEnderecoEdicao(_usuario.Endereco);

            FotoEdicaoSource = AvatarHelper.CriarImageSource(_usuario.FotoUrl, ApiBase);
            TemFotoEdicao = FotoEdicaoSource is not null;

            IsEditando = true;
        }

        private void FecharEdicao()
        {
            IsEditando = false;
            Alerta = null;
            _fotoSelecionada = null;
            _fotoLocalPreviewPath = null;
            ResetarErros();
        }

        private async Task SelecionarFotoAsync()
        {
            try
            {
                var arquivo = await FilePicker.Default.PickAsync(new PickOptions
                {
                    PickerTitle = "Selecione uma foto de perfil",
                    FileTypes = FilePickerFileType.Images
                });

                if (arquivo is null)
                    return;

                _fotoSelecionada = arquivo;

                var extensao = Path.GetExtension(arquivo.FileName);
                if (string.IsNullOrWhiteSpace(extensao))
                    extensao = ".jpg";

                _fotoLocalPreviewPath = Path.Combine(
                    FileSystem.CacheDirectory,
                    $"locatem-perfil-{Guid.NewGuid():N}{extensao}");

                await using (var origem = await arquivo.OpenReadAsync())
                await using (var destino = File.Create(_fotoLocalPreviewPath))
                {
                    await origem.CopyToAsync(destino);
                }

                FotoEdicaoSource = ImageSource.FromFile(_fotoLocalPreviewPath);
                TemFotoEdicao = true;
            }
            catch (Exception)
            {
                Alerta = new AlertaMessage(
                    "Não foi possível abrir a foto",
                    "Selecione outra imagem e tente novamente.");
            }
        }

        private async Task BuscarCepAsync()
        {
            if (!MaskHelper.ValidateCep(CepEdicao))
            {
                EnderecoResumo = string.Empty;
                return;
            }

            try
            {
                var cepLimpo = Regex.Replace(CepEdicao, @"\D", string.Empty);
                var resposta = await _cepClient.GetFromJsonAsync<ViaCepResponse>(
                    $"https://viacep.com.br/ws/{cepLimpo}/json/");

                if (resposta is null || resposta.Erro)
                {
                    EnderecoResumo = string.Empty;
                    Alerta = PerfilMessages.CepNaoEncontrado;
                    return;
                }

                if (!string.IsNullOrWhiteSpace(resposta.Logradouro))
                    LogradouroEdicao = resposta.Logradouro;

                var partes = new[] { resposta.Uf, resposta.Localidade, resposta.Bairro }
                    .Where(p => !string.IsNullOrWhiteSpace(p));

                EnderecoResumo = string.Join(", ", partes);
            }
            catch
            {
                Alerta = PerfilMessages.CepErro;
            }
        }

        private async Task SalvarPerfilAsync()
        {
            if (IsBusy || _usuario is null)
                return;

            var snapshot = new PerfilFormSnapshot
            {
                Tipo = _usuario.Tipo,
                Nome = NomeEdicao,
                Telefone = TelefoneEdicao,
                Documento = DocumentoEdicao,
                Cep = CepEdicao,
                Logradouro = LogradouroEdicao,
                Numero = NumeroEdicao
            };

            _campoErros = PerfilValidator.Validate(snapshot);
            NotificarErrosDeCampo();

            if (TratarSubmitInvalido(snapshot))
                return;

            IsBusy = true;
            Alerta = null;

            try
            {
                var enderecoCompleto =
                    $"{snapshot.Logradouro.Trim()}, {snapshot.Numero.Trim()} - CEP: {snapshot.Cep.Trim()}";

                var fotoUrlFinal = _usuario.FotoUrl;

                // Para sessões reais, mantém o mesmo upload multipart usado pelo React.
                // Em uma eventual sessão mock/offline sem token, a prévia local continua válida.
                if (_fotoSelecionada is not null)
                {
                    if (!string.IsNullOrWhiteSpace(_usuario.Token))
                    {
                        await using var stream = await _fotoSelecionada.OpenReadAsync();
                        fotoUrlFinal = await _authService.UploadFotoPerfilAsync(
                            _usuario.Token,
                            _usuario.Id,
                            stream,
                            _fotoSelecionada.FileName,
                            _fotoSelecionada.ContentType);
                    }
                    else
                    {
                        fotoUrlFinal = _fotoLocalPreviewPath;
                    }
                }

                if (!string.IsNullOrWhiteSpace(_usuario.Token))
                {
                    await _authService.AtualizarPerfilAsync(
                        _usuario.Token,
                        snapshot.Nome.Trim(),
                        snapshot.Telefone.Trim(),
                        snapshot.Documento.Trim(),
                        enderecoCompleto);
                }

                var atualizado = _usuario.CopiarCom(
                    nome: snapshot.Nome.Trim(),
                    telefone: snapshot.Telefone.Trim(),
                    documento: snapshot.Documento.Trim(),
                    endereco: enderecoCompleto,
                    fotoUrl: fotoUrlFinal);

                _authSession.AtualizarUsuario(atualizado);
                _usuario = atualizado;

                AtualizarDadosExibidos(atualizado);
                RecalcularCompletude(atualizado);

                IsEditando = false;
                _fotoSelecionada = null;
                _fotoLocalPreviewPath = null;
                ResetarErros();
            }
            catch
            {
                Alerta = PerfilMessages.SaveError;
            }
            finally
            {
                IsBusy = false;
            }
        }

        private bool TratarSubmitInvalido(PerfilFormSnapshot data)
        {
            var possuiVazio = false;

            void Checar(string valor, FieldErrorState estado, string? erro)
            {
                if (string.IsNullOrWhiteSpace(valor))
                {
                    estado.Trigger();
                    possuiVazio = true;
                }
                else if (erro is not null)
                {
                    estado.Trigger();
                }
            }

            Checar(data.Nome, NomeError, _campoErros.Nome);
            Checar(data.Telefone, TelefoneError, _campoErros.Telefone);
            Checar(data.Documento, DocumentoError, _campoErros.Documento);
            Checar(data.Cep, CepError, _campoErros.Cep);
            Checar(data.Logradouro, LogradouroError, _campoErros.Logradouro);
            Checar(data.Numero, NumeroError, _campoErros.Numero);

            if (possuiVazio)
            {
                Alerta = PerfilMessages.Required;
                return true;
            }

            if (_campoErros.Nome is not null)
            {
                Alerta = PerfilMessages.InvalidName;
                return true;
            }

            if (_campoErros.Telefone is not null)
            {
                Alerta = PerfilMessages.InvalidPhone;
                return true;
            }

            if (_campoErros.Documento is not null)
            {
                Alerta = EhLocador
                    ? PerfilMessages.InvalidCnpj
                    : PerfilMessages.InvalidCpf;
                return true;
            }

            if (_campoErros.Cep is not null)
            {
                Alerta = PerfilMessages.InvalidCep;
                return true;
            }

            return _campoErros.HasErrors;
        }

        private void ResetarErros()
        {
            _campoErros = new PerfilFieldErrors();
            NomeError.Reset();
            TelefoneError.Reset();
            DocumentoError.Reset();
            CepError.Reset();
            LogradouroError.Reset();
            NumeroError.Reset();
            NotificarErrosDeCampo();
        }

        private void NotificarErrosDeCampo()
        {
            OnPropertyChanged(nameof(NomeErrorText));
            OnPropertyChanged(nameof(TelefoneErrorText));
            OnPropertyChanged(nameof(DocumentoErrorText));
            OnPropertyChanged(nameof(CepErrorText));
            OnPropertyChanged(nameof(LogradouroErrorText));
            OnPropertyChanged(nameof(NumeroErrorText));
        }

        private void PreencherEnderecoEdicao(string? endereco)
        {
            CepEdicao = string.Empty;
            LogradouroEdicao = string.Empty;
            NumeroEdicao = string.Empty;
            EnderecoResumo = string.Empty;

            if (string.IsNullOrWhiteSpace(endereco))
                return;

            var cepMatch = Regex.Match(endereco, @"\b\d{5}-?\d{3}\b");
            if (cepMatch.Success)
                CepEdicao = MaskHelper.MaskCep(cepMatch.Value);

            var antesCep = Regex.Replace(endereco, @"\s*(?:-|·)?\s*CEP\s*:\s*\d{5}-?\d{3}.*$", string.Empty,
                RegexOptions.IgnoreCase).Trim();

            // Endereços antigos do mock seguem "Rua..., 247 – ... · 01310-100" sem "CEP:".
            antesCep = Regex.Replace(antesCep, @"\s*[·]\s*\d{5}-?\d{3}\s*$", string.Empty).Trim();

            var partes = antesCep.Split(',', 2, StringSplitOptions.TrimEntries);
            LogradouroEdicao = partes[0];

            if (partes.Length > 1)
            {
                var numeroMatch = Regex.Match(partes[1], @"\b\d+[A-Za-z]?\b");
                if (numeroMatch.Success)
                    NumeroEdicao = numeroMatch.Value;
            }

            // Se o endereço ainda não estiver no formato estruturado, preserva o texto como rua
            // em vez de apagar informação existente do usuário.
            if (string.IsNullOrWhiteSpace(LogradouroEdicao))
                LogradouroEdicao = endereco;
        }

        // ===================== LOGOUT / NAVEGAÇÃO =====================

        private async Task ConfirmarLogoutAsync()
        {
            IsConfirmandoLogout = false;
            _authSession.Logout();

            //a saída volta para a rota raiz de Login.
            await Shell.Current.GoToAsync("//login");
        }

        private static async Task NavegarAsync(string? rota)
        {
            if (string.IsNullOrWhiteSpace(rota) || !RotasMigradas.Contains(rota))
                return;

            if (Shell.Current.CurrentState.Location.OriginalString.EndsWith(
                    rota,
                    StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (string.Equals(rota, "login", StringComparison.OrdinalIgnoreCase))
                await Shell.Current.GoToAsync("//login");
            else
                await Shell.Current.GoToAsync(rota);
        }

        // ===================== PAINEL DE CONTROLE =====================

        private void MontarPainelControle()
        {
            OpcoesPainel.Clear();

            AdicionarOpcao("Aluguéis Ativos", "Visualize seus equipamentos alugados atualmente.", MaterialIcons.CalendarToday, "minhasLocacoes");
            AdicionarOpcao(
                "Histórico de Locações",
                "Consulte todas as suas locações anteriores.",
                MaterialIcons.History,
                EhLocador ? "historicoLocacoes" : null);
            AdicionarOpcao("Favoritos", "Ferramentas e equipamentos salvos.", MaterialIcons.Star, null);
            AdicionarOpcao("Pagamentos", "Visualize pagamentos, cauções e reembolsos.", MaterialIcons.Payments, null);
            AdicionarOpcao("Contratos", "Acesse todos os contratos digitais.", MaterialIcons.Assignment, null);
            AdicionarOpcao("Endereços", "Gerencie seus endereços cadastrados.", MaterialIcons.Home, null);
            AdicionarOpcao("Notificações", "Confira atualizações importantes.", MaterialIcons.Notifications, "notificacoes");
            AdicionarOpcao("Configurações", "Altere senha, dados pessoais e preferências.", MaterialIcons.Settings, null);
            AdicionarOpcao("Suporte", "Central de ajuda e atendimento.", MaterialIcons.HeadsetMic, null);
        }

        private void AdicionarOpcao(
            string titulo,
            string descricao,
            MaterialIcons icone,
            string? rota)
        {
            OpcoesPainel.Add(new PainelPerfilItem
            {
                Titulo = titulo,
                Descricao = descricao,
                Icone = icone,
                Rota = rota,
                Disponivel = rota is not null && RotasMigradas.Contains(rota)
            });
        }

        // ===================== HELPERS =====================

        private static string ValorOuNaoInformado(string? valor) =>
            string.IsNullOrWhiteSpace(valor) ? "Não informado" : valor;

        private sealed class ViaCepResponse
        {
            [JsonPropertyName("logradouro")]
            public string Logradouro { get; set; } = string.Empty;

            [JsonPropertyName("bairro")]
            public string Bairro { get; set; } = string.Empty;

            [JsonPropertyName("localidade")]
            public string Localidade { get; set; } = string.Empty;

            [JsonPropertyName("uf")]
            public string Uf { get; set; } = string.Empty;

            [JsonPropertyName("erro")]
            public bool Erro { get; set; }
        }
    }

    /// <summary>Item data-driven do Painel de Controle da tela Perfil.</summary>
    public sealed class PainelPerfilItem
    {
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public MaterialIcons Icone { get; set; }
        public string? Rota { get; set; }
        public bool Disponivel { get; set; }
        public double Opacidade => Disponivel ? 1d : 0.65d;
    }
}
