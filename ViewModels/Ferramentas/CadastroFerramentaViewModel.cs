using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;
using LOCATEM_DESKTOP.Helpers;
using LOCATEM_DESKTOP.Helpers.Conta;
using LOCATEM_DESKTOP.Models.Auth;
using LOCATEM_DESKTOP.Models.Ferramentas;
using LOCATEM_DESKTOP.Services.Auth;
using LOCATEM_DESKTOP.Services.Ferramentas;
using LOCATEM_DESKTOP.ViewModels.Base;

namespace LOCATEM_DESKTOP.ViewModels.Ferramentas;

// Mantém a origem da foto e fornece a miniatura para a lista da página.
public class FotoCadastro
{
    public string Nome { get; init; } = string.Empty;
    public string? CaminhoExistente { get; init; }
    public byte[]? Bytes { get; init; }
    public string ContentType { get; init; } = "image/jpeg";
    public ImageSource Preview => Bytes is null
        ? ImageSource.FromFile(CaminhoExistente ?? string.Empty)
        : ImageSource.FromStream(() => new MemoryStream(Bytes!));
}

// Representa uma linha editável da seção de especificações técnicas.
public class EspecificacaoCadastro
{
    public string Label { get; set; } = string.Empty;
    public string Valor { get; set; } = string.Empty;
}

// Expõe o estado visual de cada célula do calendário ao XAML.
public class DiaCadastro
{
    public string Texto { get; init; } = string.Empty;
    public DateTime Data { get; init; }
    public bool Ativo { get; init; }
    public bool Indisponivel { get; init; }
    // Cores equivalentes aos estados em CalendarioDisponibilidade.module.css.
    public Color CorFundo => !Ativo ? Colors.Transparent : Indisponivel ? Color.FromArgb("#FFF5F5") : Color.FromArgb("#F2FBF5");
    public Color CorBorda => !Ativo ? Colors.Transparent : Indisponivel ? Color.FromArgb("#FF4D4D") : Color.FromArgb("#BFE8CF");
    public Color CorTexto => !Ativo ? Color.FromArgb("#C8C8C8") : Indisponivel ? Color.FromArgb("#FF4D4D") : Color.FromArgb("#1A8A4A");
}

public class CadastroFerramentaViewModel : BaseViewModel
{
    // Os serviços continuam responsáveis por sessão, catálogo local e cadastro remoto.
    private readonly IAuthSessionService _auth;
    private readonly ICatalogoService _catalogo;
    private readonly ICadastroFerramentaService _api;
    private Produto? _edicao;
    private bool _carregado;
    private DateTime _mes = new(DateTime.Today.Year, DateTime.Today.Month, 1);
    private readonly HashSet<string> _indisponiveis = new();

    public CadastroFerramentaViewModel(IAuthSessionService auth, ICatalogoService catalogo, ICadastroFerramentaService api)
    {
        _auth = auth; _catalogo = catalogo; _api = api;
        NavegarCommand = new AsyncRelayCommand(async p =>
        {
            if (p is string rota && rota == "homeLocador") await Shell.Current.GoToAsync("//homeLocador");
            else if (p is string destino && destino == "minhasFerramentas") await VoltarAsync();
            else if (p is string gerenciar && gerenciar == "gerenciarLocacoes") await Shell.Current.GoToAsync("gerenciarLocacoes");
            else if (p is string perfil && perfil == "perfil") await Shell.Current.GoToAsync("perfil");
        });
        // Comandos ligados aos controles de fotos e especificações da página.
        AdicionarFotosCommand = new AsyncRelayCommand(AdicionarFotosAsync);
        RemoverFotoCommand = new RelayCommand(p => { if (p is FotoCadastro f) Fotos.Remove(f); });
        MoverFotoCommand = new RelayCommand(p => { if (p is FotoCadastro f && Fotos.IndexOf(f) > 0) Fotos.Move(Fotos.IndexOf(f), Fotos.IndexOf(f) - 1); });
        AdicionarEspecificacaoCommand = new RelayCommand(() =>
        {
            if (Especificacoes.Any(e => string.IsNullOrWhiteSpace(e.Label) || string.IsNullOrWhiteSpace(e.Valor)))
                Erro = "Preencha a especificação anterior antes de adicionar outra.";
            else { Erro = string.Empty; Especificacoes.Add(new()); }
        });
        RemoverEspecificacaoCommand = new RelayCommand(p => { if (p is EspecificacaoCadastro e) Especificacoes.Remove(e); });
        // Acessórios são adicionados pelo Enter e removidos pela própria lista.
        AdicionarAcessorioCommand = new RelayCommand(AdicionarAcessorio);
        RemoverAcessorioCommand = new RelayCommand(p => { if (p is string s) Acessorios.Remove(s); });
        // Navega entre meses e alterna apenas os dias futuros do mês exibido.
        MesAnteriorCommand = new RelayCommand(() => { _mes = _mes.AddMonths(-1); MontarCalendario(); });
        ProximoMesCommand = new RelayCommand(() => { _mes = _mes.AddMonths(1); MontarCalendario(); });
        AlternarDiaCommand = new RelayCommand(p =>
        {
            if (p is not DiaCadastro dia || !dia.Ativo) return;
            var iso = dia.Data.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            if (!_indisponiveis.Add(iso)) _indisponiveis.Remove(iso);
            MontarCalendario();
        });
        // Os chips e cartões atualizam os mesmos campos já usados na publicação.
        SelecionarFonteCommand = new RelayCommand(p => { if (p is string fonte) FonteAlimentacao = fonte; });
        SelecionarAprovacaoCommand = new RelayCommand(p => { if (p is string opcao) TipoAprovacao = opcao; });
        PublicarCommand = new AsyncRelayCommand(PublicarAsync);
        CancelarCommand = new AsyncRelayCommand(CancelarAsync);
        Especificacoes.Add(new());
        MontarCalendario();
    }

    // Opções fixas compartilhadas pelos controles e pela validação do formulário.
    public static IReadOnlyList<string> Categorias { get; } = new[]
    {
        "Ferramentas Elétricas • Parafusadeira/Furadeira", "Ferramentas Elétricas • Corte e Desgaste",
        "Ferramentas Elétricas • Pintura", "Ferramentas Manuais", "Jardinagem e Paisagismo",
        "Construção e Alvenaria", "Elevação e Transporte", "Limpeza e Lavagem"
    };
    public static IReadOnlyList<string> Estados { get; } = new[] { "Novo", "Seminovo", "Usado - Bom estado", "Usado - Estado regular" };
    public static IReadOnlyList<string> Fontes { get; } = new[] { "127V", "220V", "Bivolt", "À bateria", "Pneumática", "Manual", "Trifásica (380V)" };
    public static IReadOnlyList<string> Aprovacoes { get; } = new[] { "Aprovação manual", "Aprovação automática" };
    public IReadOnlyList<string> OpcoesCategorias => Categorias;
    public IReadOnlyList<string> OpcoesEstados => Estados;
    public IReadOnlyList<string> OpcoesFontes => Fontes;
    public IReadOnlyList<string> OpcoesAprovacoes => Aprovacoes;
    // Coleções observáveis atualizam miniaturas, linhas e calendário no XAML.
    public ObservableCollection<FotoCadastro> Fotos { get; } = new();
    public ObservableCollection<EspecificacaoCadastro> Especificacoes { get; } = new();
    public ObservableCollection<string> Acessorios { get; } = new();
    public ObservableCollection<DiaCadastro> Dias { get; } = new();

    private string _nome = "", _marca = "", _modelo = "", _categoria = "", _estado = "", _fonte = "", _descricao = "", _diaria = "", _caucao = "", _acessorio = "", _aprovacao = "", _cep = "", _rua = "", _numero = "", _complemento = "", _erro = "", _nomeUsuario = "";
    private int _quantidade = 1;
    private ImageSource? _fotoUsuario;
    private string _titulo = "Cadastrar Ferramenta", _botaoPublicar = "Publicar Ferramenta", _mesTexto = "";
    private int _alturaCalendario = 256;

    // SetProperty notifica a interface sempre que um campo editável muda.
    public string Nome { get => _nome; set => SetProperty(ref _nome, value); }
    public string Marca { get => _marca; set => SetProperty(ref _marca, value); }
    public string Modelo { get => _modelo; set => SetProperty(ref _modelo, value); }
    public string Categoria { get => _categoria; set => SetProperty(ref _categoria, value); }
    public string EstadoConservacao { get => _estado; set => SetProperty(ref _estado, value); }
    public string FonteAlimentacao { get => _fonte; set => SetProperty(ref _fonte, value); }
    public string Descricao { get => _descricao; set => SetProperty(ref _descricao, value); }
    public string ValorDiaria { get => _diaria; set => SetProperty(ref _diaria, value); }
    public string Caucao { get => _caucao; set => SetProperty(ref _caucao, value); }
    public string AcessorioNovo { get => _acessorio; set => SetProperty(ref _acessorio, value); }
    public string TipoAprovacao { get => _aprovacao; set => SetProperty(ref _aprovacao, value); }
    public string Cep { get => _cep; set => SetProperty(ref _cep, value); }
    public string RuaAvenida { get => _rua; set => SetProperty(ref _rua, value); }
    public string Numero { get => _numero; set => SetProperty(ref _numero, value); }
    public string Complemento { get => _complemento; set => SetProperty(ref _complemento, value); }
    // A visibilidade da mensagem acompanha o texto de erro.
    public string Erro { get => _erro; set { if (SetProperty(ref _erro, value)) OnPropertyChanged(nameof(TemErro)); } }
    public bool TemErro => !string.IsNullOrEmpty(Erro);
    // O limite também vale quando o usuário usa os botões de incremento.
    public int QuantidadeDisponivel { get => _quantidade; set => SetProperty(ref _quantidade, Math.Clamp(value, 1, 999)); }
    public string NomeUsuario { get => _nomeUsuario; private set => SetProperty(ref _nomeUsuario, value); }
    public ImageSource? FotoUsuario { get => _fotoUsuario; private set => SetProperty(ref _fotoUsuario, value); }
    public string Titulo { get => _titulo; private set => SetProperty(ref _titulo, value); }
    public string BotaoPublicar { get => _botaoPublicar; private set => SetProperty(ref _botaoPublicar, value); }
    public string MesTexto { get => _mesTexto; private set => SetProperty(ref _mesTexto, value); }
    // A grade ocupa apenas a quantidade de semanas necessária ao mês.
    public int AlturaCalendario { get => _alturaCalendario; private set => SetProperty(ref _alturaCalendario, value); }

    public ICommand NavegarCommand { get; }
    public ICommand AdicionarFotosCommand { get; }
    public ICommand RemoverFotoCommand { get; }
    public ICommand MoverFotoCommand { get; }
    public ICommand AdicionarEspecificacaoCommand { get; }
    public ICommand RemoverEspecificacaoCommand { get; }
    public ICommand AdicionarAcessorioCommand { get; }
    public ICommand RemoverAcessorioCommand { get; }
    public ICommand MesAnteriorCommand { get; }
    public ICommand ProximoMesCommand { get; }
    public ICommand AlternarDiaCommand { get; }
    public ICommand SelecionarFonteCommand { get; }
    public ICommand SelecionarAprovacaoCommand { get; }
    public ICommand PublicarCommand { get; }
    public ICommand CancelarCommand { get; }

    public async Task<bool> CarregarAsync()
    {
        // Confere a sessão e preenche o mesmo formulário em modo cadastro ou edição.
        var usuario = _auth.UsuarioAtual;
        if (usuario is null || usuario.Tipo != TipoUsuario.Locador)
        {
            await Shell.Current.GoToAsync("//login");
            return false;
        }
        if (_carregado) return true;
        _carregado = true;
        NomeUsuario = usuario.Nome;
        FotoUsuario = AvatarHelper.CriarImageSource(usuario.FotoUrl);
        var id = _catalogo.FerramentaSelecionadaId;
        _edicao = id is null ? null : _catalogo.Produtos.FirstOrDefault(p => p.Id == id && p.LocadorId == usuario.LocadorId);
        if (id is not null && _edicao is null) { await Shell.Current.GoToAsync(".."); return false; }
        if (_edicao is not null)
        {
            Titulo = "Editar Ferramenta"; BotaoPublicar = "Salvar Alterações";
            Nome = _edicao.Title; Marca = _edicao.Marca; Modelo = _edicao.Modelo;
            Categoria = _edicao.Categoria; EstadoConservacao = _edicao.EstadoConservacao;
            QuantidadeDisponivel = _edicao.QuantidadeDisponivel; FonteAlimentacao = _edicao.FonteAlimentacao;
            Descricao = _edicao.Descricao; ValorDiaria = _edicao.Price; Caucao = _edicao.Caucao;
            TipoAprovacao = _edicao.TipoAprovacao == "manual" ? Aprovacoes[0] : _edicao.TipoAprovacao == "automatica" ? Aprovacoes[1] : "";
            Cep = _edicao.Cep; RuaAvenida = _edicao.RuaAvenida; Numero = _edicao.Numero; Complemento = _edicao.Complemento;
            Fotos.Clear(); foreach (var foto in _edicao.Images) Fotos.Add(new() { Nome = Path.GetFileName(foto), CaminhoExistente = foto });
            Especificacoes.Clear(); foreach (var esp in _edicao.Especificacoes) Especificacoes.Add(new() { Label = esp.Label, Valor = esp.Valor });
            if (Especificacoes.Count == 0) Especificacoes.Add(new());
            Acessorios.Clear(); foreach (var item in _edicao.Acessorios) Acessorios.Add(item);
            _indisponiveis.Clear(); foreach (var dia in _edicao.DiasIndisponiveis) _indisponiveis.Add(dia);
            MontarCalendario();
        }
        return true;
    }

    private async Task AdicionarFotosAsync()
    {
        // O seletor nativo aceita até oito imagens e guarda seus bytes para prévia e envio.
        if (Fotos.Count >= 8) return;
        try
        {
            var arquivos = await FilePicker.Default.PickMultipleAsync(new PickOptions { PickerTitle = "Fotos da ferramenta", FileTypes = FilePickerFileType.Images });
            if (arquivos is null) return;
            foreach (var arquivo in arquivos.Take(8 - Fotos.Count))
            {
                await using var stream = await arquivo.OpenReadAsync();
                using var buffer = new MemoryStream();
                await stream.CopyToAsync(buffer);
                var extension = Path.GetExtension(arquivo.FileName).ToLowerInvariant();
                Fotos.Add(new FotoCadastro
                {
                    Nome = arquivo.FileName,
                    Bytes = buffer.ToArray(),
                    ContentType = extension switch { ".png" => "image/png", ".webp" => "image/webp", ".gif" => "image/gif", _ => "image/jpeg" }
                });
            }
        }
        catch (Exception ex) { Erro = $"Não foi possível carregar as fotos: {ex.Message}"; }
    }

    private void AdicionarAcessorio()
    {
        // Evita itens vazios ou duplicados e limpa o campo após a inclusão.
        var valor = AcessorioNovo.Trim();
        if (valor.Length > 0 && !Acessorios.Contains(valor)) Acessorios.Add(valor);
        AcessorioNovo = string.Empty;
    }

    private void MontarCalendario()
    {
        // Alinha a primeira célula ao domingo e gera semanas completas para a grade.
        MesTexto = _mes.ToString("MMMM yyyy", CultureInfo.GetCultureInfo("pt-BR"));
        Dias.Clear();
        var inicio = _mes.AddDays(-(int)_mes.DayOfWeek);
        var semanas = (int)Math.Ceiling(((int)_mes.DayOfWeek + DateTime.DaysInMonth(_mes.Year, _mes.Month)) / 7d);
        AlturaCalendario = semanas * 52 - 4;
        for (var i = 0; i < semanas * 7; i++)
        {
            var dia = inicio.AddDays(i);
            Dias.Add(new DiaCadastro
            {
                Data = dia,
                Texto = dia.Day.ToString(CultureInfo.InvariantCulture),
                Ativo = dia.Month == _mes.Month && dia.Date >= DateTime.Today,
                Indisponivel = _indisponiveis.Contains(dia.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture))
            });
        }
    }

    private static bool Moeda(string input, out decimal valor)
    {
        var texto = input.Trim();
        // Vírgula decimal segue a Web; ponto decimal também funciona no teclado numérico.
        texto = texto.Contains(',') ? texto.Replace(".", "").Replace(',', '.') : texto;
        return decimal.TryParse(texto, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out valor);
    }

    private string Validar()
    {
        // Reúne os erros antes de publicar; a página exibe a mensagem resultante.
        var erros = new List<string>();
        if (Fotos.Count == 0) erros.Add("Adicione ao menos 1 foto da ferramenta.");
        if (string.IsNullOrWhiteSpace(Nome)) erros.Add("Informe o nome da ferramenta.");
        if (string.IsNullOrWhiteSpace(Marca)) erros.Add("Informe a marca.");
        if (string.IsNullOrWhiteSpace(Modelo)) erros.Add("Informe o modelo.");
        if (!Categorias.Contains(Categoria)) erros.Add("Selecione uma categoria.");
        if (!Estados.Contains(EstadoConservacao)) erros.Add("Selecione o estado de conservação.");
        if (!Fontes.Contains(FonteAlimentacao)) erros.Add("Selecione a fonte de alimentação.");
        if (Descricao.Trim().Length < 50 || Descricao.Length > 1000) erros.Add("A descrição deve ter de 50 a 1000 caracteres.");
        if (Especificacoes.Any(e => string.IsNullOrWhiteSpace(e.Label) || string.IsNullOrWhiteSpace(e.Valor))) erros.Add("Preencha todas as especificações técnicas.");
        if (!Moeda(ValorDiaria, out var diaria) || diaria <= 0) erros.Add("Informe um valor de diária maior que zero.");
        if (Caucao.Trim().Length > 0 && (!Moeda(Caucao, out var caucao) || caucao < 0)) erros.Add("Informe uma caução válida.");
        if (!Aprovacoes.Contains(TipoAprovacao)) erros.Add("Selecione a aprovação da locação.");
        if (new string(Cep.Where(char.IsDigit).ToArray()).Length != 8) erros.Add("Informe um CEP válido.");
        if (string.IsNullOrWhiteSpace(RuaAvenida)) erros.Add("Informe a rua/avenida.");
        if (string.IsNullOrWhiteSpace(Numero)) erros.Add("Informe o número.");
        return string.Join("\n", erros);
    }

    private async Task PublicarAsync()
    {
        // Valida, envia ao serviço quando aplicável e atualiza o catálogo local.
        if (IsBusy) return;
        AdicionarAcessorio();
        Erro = Validar();
        if (TemErro) return;
        var usuario = _auth.UsuarioAtual;
        if (usuario is null || usuario.Tipo != TipoUsuario.Locador || string.IsNullOrEmpty(usuario.LocadorId))
        { Erro = "Entre com uma conta de locador para cadastrar a ferramenta."; return; }

        IsBusy = true;
        try
        {
            Moeda(ValorDiaria, out var diaria);
            Moeda(Caucao, out var caucao);
            if (_edicao is null && !string.IsNullOrWhiteSpace(usuario.Token) && !usuario.Token.StartsWith("mock-token-", StringComparison.Ordinal))
            {
                var fotos = Fotos.Where(f => f.Bytes is not null).Select(f => new FotoFerramentaDados(f.Nome, f.ContentType, f.Bytes!)).ToArray();
                await _api.CadastrarAsync(usuario.Token, new CadastroFerramentaDados(Nome.Trim(), Marca.Trim(), Modelo.Trim(), Descricao.Trim(), Acessorios.ToArray(), diaria, caucao, Array.IndexOf(Categorias.ToArray(), Categoria) + 1), fotos);
            }
            // O catálogo local usa caminhos em AppData para as fotos recém-selecionadas.
            var imagens = new List<string>();
            foreach (var foto in Fotos)
            {
                if (foto.Bytes is null) { imagens.Add(foto.CaminhoExistente!); continue; }
                var extension = Path.GetExtension(foto.Nome).ToLowerInvariant();
                if (extension is not (".jpg" or ".jpeg" or ".png" or ".webp" or ".gif")) extension = ".jpg";
                var caminho = Path.Combine(FileSystem.AppDataDirectory, $"ferramenta_{Guid.NewGuid():N}{extension}");
                await File.WriteAllBytesAsync(caminho, foto.Bytes);
                imagens.Add(caminho);
            }
            var produto = new Produto
            {
                Title = Nome.Trim(),
                Marca = Marca.Trim(),
                Modelo = Modelo.Trim(),
                Categoria = Categoria,
                EstadoConservacao = EstadoConservacao,
                QuantidadeDisponivel = QuantidadeDisponivel,
                FonteAlimentacao = FonteAlimentacao,
                Descricao = Descricao.Trim(),
                Price = ValorDiaria,
                Caucao = Caucao,
                Images = imagens,
                Locador = _edicao?.Locador ?? usuario.Nome,
                LocadorId = usuario.LocadorId,
                Localizacao = _edicao?.Localizacao ?? "São Paulo - SP",
                Status = _edicao?.Status ?? StatusFerramenta.Disponivel,
                CadastradoEm = _edicao?.CadastradoEm ?? DateTime.Today.ToString("dd/MM/yyyy", CultureInfo.GetCultureInfo("pt-BR")),
                Avaliacoes = _edicao?.Avaliacoes ?? new(),
                Especificacoes = Especificacoes.Where(e => !string.IsNullOrWhiteSpace(e.Label) && !string.IsNullOrWhiteSpace(e.Valor)).Select(e => new EspecificacaoFerramenta { Label = e.Label.Trim(), Valor = e.Valor.Trim() }).ToList(),
                Acessorios = Acessorios.ToList(),
                DiasIndisponiveis = _indisponiveis.ToList(),
                TipoAprovacao = TipoAprovacao == Aprovacoes[0] ? "manual" : "automatica",
                Cep = Cep,
                RuaAvenida = RuaAvenida,
                Numero = Numero,
                Complemento = Complemento
            };
            if (_edicao is null) _catalogo.Adicionar(produto);
            else _catalogo.Atualizar(_edicao.Id, produto);
            await Shell.Current.DisplayAlert("Ferramenta salva", "Sua ferramenta está em Minhas Ferramentas.", "Ver minhas ferramentas");
            _catalogo.FerramentaSelecionadaId = null;
            await VoltarAsync();
        }
        catch (Exception ex) { Erro = ex.Message; }
        finally { IsBusy = false; }
    }

    private async Task CancelarAsync()
    {
        // Confirma a perda dos dados antes de sair do cadastro.
        if (IsBusy) return;
        if (await Shell.Current.DisplayAlert("Cancelar cadastro", "As informações preenchidas serão perdidas. Deseja cancelar?", "Sim, cancelar", "Continuar editando"))
        { _catalogo.FerramentaSelecionadaId = null; await VoltarAsync(); }
    }

    private static async Task VoltarAsync()
    {
        // Retorna à tela anterior ou abre Minhas Ferramentas quando não há histórico.
        if (Shell.Current.Navigation.NavigationStack.Count > 1) await Shell.Current.GoToAsync("..");
        else await Shell.Current.GoToAsync("minhasFerramentas");
    }
}
