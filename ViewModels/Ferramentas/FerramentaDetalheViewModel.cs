using System.Collections.ObjectModel;
using System.Windows.Input;
using LOCATEM_DESKTOP.Helpers;
using LOCATEM_DESKTOP.Helpers.Avaliacao;
using LOCATEM_DESKTOP.Helpers.Conta;
using LOCATEM_DESKTOP.Helpers.Formatacao;
using LOCATEM_DESKTOP.Models.Auth;
using LOCATEM_DESKTOP.Models.Ferramentas;
using LOCATEM_DESKTOP.Models.Locacoes;
using LOCATEM_DESKTOP.Services.Auth;
using LOCATEM_DESKTOP.Services.Ferramentas;
using LOCATEM_DESKTOP.Services.Locacoes;
using LOCATEM_DESKTOP.ViewModels.Base;

namespace LOCATEM_DESKTOP.ViewModels.Ferramentas;

// Uma miniatura e seu estado de seleção no carrossel.
public class FotoFerramentaDetalhe : BaseViewModel
{
    private bool _selecionada;

    public ImageSource? Imagem { get; init; }
    public int Indice { get; init; }
    public bool Selecionada
    {
        get => _selecionada;
        set => SetProperty(ref _selecionada, value);
    }
}

// Mantém a alternância de fundo das linhas da tabela do Web.
public class EspecificacaoFerramentaDetalhe
{
    public string Label { get; init; } = string.Empty;
    public string Valor { get; init; } = string.Empty;
    public Color Fundo { get; init; } = Colors.White;
}

public class FerramentaDetalheViewModel : BaseViewModel
{
    private readonly IAuthSessionService _auth;
    private readonly ICatalogoService _catalogo;
    private readonly ILocacaoService _locacoes;
    private Produto? _produto;
    private int _indiceFoto;
    private string _titulo = string.Empty;
    private string _subtitulo = string.Empty;
    private string _mensagemRemocao = string.Empty;
    private string _nomeUsuario = string.Empty;
    private ImageSource? _fotoUsuario;
    private ImageSource? _fotoAtual;
    private StatusFerramenta _status;
    private bool _temVariasFotos;
    private bool _semFotos;
    private bool _podePausar;
    private bool _pausado;
    private bool _confirmandoRemocao;

    public FerramentaDetalheViewModel(IAuthSessionService auth, ICatalogoService catalogo, ILocacaoService locacoes)
    {
        _auth = auth;
        _catalogo = catalogo;
        _locacoes = locacoes;

        NavegarCommand = new AsyncRelayCommand(async p =>
        {
            if (p is not string rota) return;
            if (rota == "homeLocador") await Shell.Current.GoToAsync("//homeLocador");
            else if (rota == "minhasFerramentas") await VoltarAsync();
            else if (rota == "perfil") await Shell.Current.GoToAsync("perfil");
        });
        FotoAnteriorCommand = new RelayCommand(() => SelecionarFoto(_indiceFoto - 1));
        ProximaFotoCommand = new RelayCommand(() => SelecionarFoto(_indiceFoto + 1));
        SelecionarFotoCommand = new RelayCommand(p =>
        {
            if (p is FotoFerramentaDetalhe foto) SelecionarFoto(foto.Indice);
        });
        EditarCommand = new AsyncRelayCommand(async () =>
        {
            if (_produto is null) return;
            _catalogo.FerramentaSelecionadaId = _produto.Id;
            await Shell.Current.GoToAsync("cadastroFerramenta");
        });
        PausarOuReativarCommand = new RelayCommand(AlternarDisponibilidade);
        PedirRemocaoCommand = new RelayCommand(() => ConfirmandoRemocao = true);
        CancelarRemocaoCommand = new RelayCommand(() => ConfirmandoRemocao = false);
        ConfirmarRemocaoCommand = new AsyncRelayCommand(RemoverAsync);
    }

    public ICommand NavegarCommand { get; }
    public ICommand FotoAnteriorCommand { get; }
    public ICommand ProximaFotoCommand { get; }
    public ICommand SelecionarFotoCommand { get; }
    public ICommand EditarCommand { get; }
    public ICommand PausarOuReativarCommand { get; }
    public ICommand PedirRemocaoCommand { get; }
    public ICommand CancelarRemocaoCommand { get; }
    public ICommand ConfirmarRemocaoCommand { get; }

    public ObservableCollection<FotoFerramentaDetalhe> Fotos { get; } = new();
    public ObservableCollection<EspecificacaoFerramentaDetalhe> Especificacoes { get; } = new();

    public string Titulo { get => _titulo; private set => SetProperty(ref _titulo, value); }
    public string Subtitulo { get => _subtitulo; private set => SetProperty(ref _subtitulo, value); }
    public string MensagemRemocao { get => _mensagemRemocao; private set => SetProperty(ref _mensagemRemocao, value); }
    public string NomeUsuario { get => _nomeUsuario; private set => SetProperty(ref _nomeUsuario, value); }
    public ImageSource? FotoUsuario { get => _fotoUsuario; private set => SetProperty(ref _fotoUsuario, value); }
    public ImageSource? FotoAtual { get => _fotoAtual; private set => SetProperty(ref _fotoAtual, value); }
    public StatusFerramenta Status { get => _status; private set => SetProperty(ref _status, value); }
    public bool TemVariasFotos { get => _temVariasFotos; private set => SetProperty(ref _temVariasFotos, value); }
    public bool SemFotos { get => _semFotos; private set => SetProperty(ref _semFotos, value); }
    public bool PodePausar { get => _podePausar; private set => SetProperty(ref _podePausar, value); }
    public bool Pausado { get => _pausado; private set => SetProperty(ref _pausado, value); }
    public bool ConfirmandoRemocao { get => _confirmandoRemocao; private set => SetProperty(ref _confirmandoRemocao, value); }

    private string _descricao = string.Empty;
    private string _diaria = string.Empty;
    private string _caucao = string.Empty;
    private string _estoque = string.Empty;
    private string _aprovacao = string.Empty;
    private string _locacoesRealizadas = "0";
    private string _receita = "R$ 0,00";
    private string _avaliacao = string.Empty;

    public string Descricao { get => _descricao; private set => SetProperty(ref _descricao, value); }
    public string Diaria { get => _diaria; private set => SetProperty(ref _diaria, value); }
    public string Caucao { get => _caucao; private set => SetProperty(ref _caucao, value); }
    public string Estoque { get => _estoque; private set => SetProperty(ref _estoque, value); }
    public string Aprovacao { get => _aprovacao; private set => SetProperty(ref _aprovacao, value); }
    public string LocacoesRealizadas { get => _locacoesRealizadas; private set => SetProperty(ref _locacoesRealizadas, value); }
    public string Receita { get => _receita; private set => SetProperty(ref _receita, value); }
    public string Avaliacao { get => _avaliacao; private set => SetProperty(ref _avaliacao, value); }

    public async Task<bool> CarregarAsync()
    {
        var usuario = _auth.UsuarioAtual;
        if (usuario is null || usuario.Tipo != TipoUsuario.Locador)
        {
            await Shell.Current.GoToAsync("//login");
            return false;
        }

        // O ID já foi definido pelo botão Ver. Impede a leitura de anúncios de outro locador.
        // O cadastro limpa a seleção ao salvar; ao voltar da edição, conserva o anúncio já aberto.
        var id = _catalogo.FerramentaSelecionadaId ?? _produto?.Id;
        _produto = id is null ? null : _catalogo.Produtos.FirstOrDefault(p => p.Id == id && p.LocadorId == usuario.LocadorId);
        if (_produto is null)
        {
            await VoltarAsync();
            return false;
        }

        NomeUsuario = usuario.Nome;
        FotoUsuario = AvatarHelper.CriarImageSource(usuario.FotoUrl);
        Titulo = _produto.Title;
        MensagemRemocao = $"Tem certeza que deseja remover {_produto.Title} do seu catálogo? Essa ação não pode ser desfeita.";
        Subtitulo = $"Cadastrada em {_produto.CadastradoEm ?? "—"} · Aprovação {(_produto.TipoAprovacao == "automatica" ? "automática" : "manual")}";
        Descricao = _produto.Descricao;
        Diaria = $"R$ {_produto.Price}";
        Caucao = string.IsNullOrWhiteSpace(_produto.Caucao) ? "Não exigida" : $"R$ {_produto.Caucao}";
        Estoque = $"{_produto.QuantidadeDisponivel} unidades";
        Aprovacao = _produto.TipoAprovacao == "automatica" ? "Automática" : "Manual";
        AtualizarStatus();

        Especificacoes.Clear();
        foreach (var (especificacao, indice) in _produto.Especificacoes.Select((item, indice) => (item, indice)))
            Especificacoes.Add(new EspecificacaoFerramentaDetalhe
            {
                Label = especificacao.Label,
                Valor = especificacao.Valor,
                Fundo = indice % 2 == 0 ? Colors.White : Color.FromArgb("#F9FAFB")
            });

        Fotos.Clear();
        foreach (var (caminho, indice) in _produto.Images.Select((imagem, indice) => (imagem, indice)))
        {
            Fotos.Add(new FotoFerramentaDetalhe { Indice = indice, Imagem = FonteDaFoto(caminho) });
        }
        SemFotos = Fotos.Count == 0;
        TemVariasFotos = Fotos.Count > 1;
        _indiceFoto = 0;
        if (Fotos.Count > 0) SelecionarFoto(0);
        else FotoAtual = null;

        // Desempenho deriva apenas das locações desta ferramenta, como no Web.
        var locacoes = _locacoes.ObterPorLocador(usuario.LocadorId).Where(l => l.ProdutoId == _produto.Id).ToList();
        LocacoesRealizadas = locacoes.Count(l => l.Status is StatusLocacao.Finalizada or StatusLocacao.EmAndamento or StatusLocacao.AguardandoDevolucao or StatusLocacao.DevolucaoEmTransporte).ToString();
        Receita = ValorMonetario.Formatar(locacoes.Where(l => l.Status == StatusLocacao.Finalizada).Sum(l => ValorMonetario.ParaNumero(l.Valor.Replace("R$", "", StringComparison.OrdinalIgnoreCase).Trim())));
        var avaliacoes = AvaliacoesResumo.Calcular(_produto.Avaliacoes);
        Avaliacao = $"{avaliacoes.Media:0.0} ({avaliacoes.Quantidade} avaliações)";
        return true;
    }

    private static ImageSource FonteDaFoto(string caminho)
    {
        if (Uri.TryCreate(caminho, UriKind.Absolute, out var uri) &&
            (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
            return ImageSource.FromUri(uri);
        return ImageSource.FromFile(caminho);
    }

    private void SelecionarFoto(int indice)
    {
        if (indice < 0 || indice >= Fotos.Count) return;
        _indiceFoto = indice;
        FotoAtual = Fotos[indice].Imagem;
        foreach (var foto in Fotos) foto.Selecionada = foto.Indice == indice;
    }

    private void AtualizarStatus()
    {
        if (_produto is null) return;
        Status = _produto.Status;
        Pausado = Status == StatusFerramenta.Indisponivel;
        PodePausar = Status is StatusFerramenta.Disponivel or StatusFerramenta.Indisponivel;
    }

    private void AlternarDisponibilidade()
    {
        if (_produto is null || !PodePausar) return;
        _produto.Status = Pausado ? StatusFerramenta.Disponivel : StatusFerramenta.Indisponivel;
        _catalogo.Atualizar(_produto.Id, _produto);
        AtualizarStatus();
    }

    private async Task RemoverAsync()
    {
        if (_produto is null) return;
        ConfirmandoRemocao = false;
        _catalogo.Remover(_produto.Id);
        _catalogo.FerramentaSelecionadaId = null;
        await VoltarAsync();
    }

    private static async Task VoltarAsync()
    {
        if (Shell.Current.Navigation.NavigationStack.Count > 1) await Shell.Current.GoToAsync("..");
        else await Shell.Current.GoToAsync("minhasFerramentas");
    }
}
