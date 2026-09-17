using LOCATEM_DESKTOP.Models.Ferramentas;

namespace LOCATEM_DESKTOP.Services.Ferramentas
{
    //
    // Catálogo central de ferramentas — migrado de mocks/produtos.mock.ts. Segue o mesmo padrão já
    // adotado em Services/Auth/UsuariosMock.cs: fonte inicial de dados enquanto o projeto não tem
    // uma API de catálogo (no React o CatalogoProvider também parte deste mesmo mock).
    //
    // Os nomes de imagem seguem a sanitização do MAUI para recursos (minúsculas, sem caracteres
    // inválidos) sobre os arquivos já existentes em Resources/Images/ProdutosImg.
    //
    public static class ProdutosMock
    {
        public static List<Produto> Criar() => new()
        {
            new Produto
            {
                Id = 1,
                Title = "Furadeira Parafusadeira Sem Fio A Bateria The Black Tools",
                Marca = "The Black Tools",
                Price = "25,00",
                Images = new List<string> { "furadeiratheblacktools.png" },
                Locador = "MS Ferramentas",
                LocadorId = "loc-ms",
                Localizacao = "São Paulo - SP",
                Categoria = "Ferramentas Elétricas • Parafusadeira/Furadeira",
                Status = StatusFerramenta.Disponivel,
                CadastradoEm = "03/01/2026",
                Avaliacoes = new List<AvaliacaoProduto>
                {
                    new() { Nome = "Renata Alves", Nota = 5 },
                    new() { Nome = "Diego Martins", Nota = 5 },
                    new() { Nome = "Sandra Lima", Nota = 4 },
                }
            },
            new Produto
            {
                Id = 2,
                Title = "Pistola de Pintura Sucção The Black Tools",
                Marca = "The Black Tools",
                Price = "35,00",
                Images = new List<string> { "pistolapintura.png" },
                Locador = "WZ Ferramentas",
                LocadorId = "loc-wz",
                Localizacao = "São Paulo - SP",
                Categoria = "Ferramentas Elétricas • Pintura",
                Status = StatusFerramenta.Disponivel,
                CadastradoEm = "15/01/2026",
                Avaliacoes = new List<AvaliacaoProduto>
                {
                    new() { Nome = "Carlos Eduardo", Nota = 4 },
                    new() { Nome = "Patrícia Nogueira", Nota = 5 },
                    new() { Nome = "Fábio Ramos", Nota = 3 },
                }
            },
            new Produto
            {
                Id = 3,
                Title = "Parafusadeira Furadeira de Impacto Hanabi",
                Marca = "Hanabi",
                Price = "38,00",
                Images = new List<string> { "furadeirahanabi.png" },
                Locador = "JB Ferramentas",
                LocadorId = "loc-jb",
                Localizacao = "São Paulo - SP",
                Categoria = "Ferramentas Elétricas • Parafusadeira/Furadeira",
                Status = StatusFerramenta.Disponivel,
                CadastradoEm = "20/01/2026",
                Avaliacoes = new List<AvaliacaoProduto>
                {
                    new() { Nome = "Anderson Souza", Nota = 5 },
                    new() { Nome = "Juliana Prado", Nota = 5 },
                    new() { Nome = "Marcelo Tanaka", Nota = 4 },
                }
            },
            new Produto
            {
                Id = 4,
                Title = "Aparador De Grama Bipartido Tramontina",
                Marca = "Tramontina",
                Price = "40,00",
                Images = new List<string> { "aparadorgrama.png" },
                Locador = "JB Ferramentas",
                LocadorId = "loc-jb",
                Localizacao = "São Paulo - SP",
                Categoria = "Jardinagem e Paisagismo",
                Status = StatusFerramenta.Disponivel,
                CadastradoEm = "22/01/2026",
                Avaliacoes = new List<AvaliacaoProduto>
                {
                    new() { Nome = "Helena Bittencourt", Nota = 4 },
                    new() { Nome = "Roberto Cunha", Nota = 5 },
                    new() { Nome = "Vanessa Castro", Nota = 3 },
                }
            },
            new Produto
            {
                Id = 5,
                Title = "Cortador De Grama Tramontina",
                Marca = "Tramontina",
                Price = "70,00",
                Images = new List<string> { "aparedorgramacarrinho.png" },
                Locador = "JB Ferramentas",
                LocadorId = "loc-jb",
                Localizacao = "São Paulo - SP",
                Categoria = "Jardinagem e Paisagismo",
                Status = StatusFerramenta.Locada,
                CadastradoEm = "25/01/2026",
                Avaliacoes = new List<AvaliacaoProduto>
                {
                    new() { Nome = "Eduardo Fontoura", Nota = 5 },
                    new() { Nome = "Camila Duarte", Nota = 4 },
                    new() { Nome = "Thiago Nascimento", Nota = 4 },
                }
            },
            new Produto
            {
                Id = 6,
                Title = "Serra Circular Profissional 220v Desoon",
                Marca = "Desoon",
                Price = "55,00",
                Images = new List<string> { "serracircularprofissional.png" },
                Locador = "JB Ferramentas",
                LocadorId = "loc-jb",
                Localizacao = "São Paulo - SP",
                Categoria = "Ferramentas Elétricas • Corte e Desgaste",
                Status = StatusFerramenta.Manutencao,
                CadastradoEm = "28/01/2026",
                Avaliacoes = new List<AvaliacaoProduto>
                {
                    new() { Nome = "Paulo Ricardo", Nota = 5 },
                    new() { Nome = "Lucas Ferraz", Nota = 5 },
                    new() { Nome = "Bianca Rocha", Nota = 4 },
                }
            },
            new Produto
            {
                Id = 7,
                Title = "Parafusadeira A Bateria Wap Sem Fio + Maleta E Brocas",
                Marca = "WAP",
                Price = "20,00",
                Images = new List<string> { "furadeirawapcinza.png" },
                Locador = "JB Ferramentas",
                LocadorId = "loc-jb",
                Localizacao = "São Paulo - SP",
                Categoria = "Ferramentas Elétricas • Parafusadeira/Furadeira",
                Status = StatusFerramenta.Indisponivel,
                CadastradoEm = "30/01/2026",
                Avaliacoes = new List<AvaliacaoProduto>
                {
                    new() { Nome = "Marta Ferreira", Nota = 4 },
                    new() { Nome = "Gilberto Nunes", Nota = 4 },
                    new() { Nome = "Aline Bezerra", Nota = 3 },
                }
            },
            new Produto
            {
                Id = 8,
                Title = "Furadeira Industrial Impacto Rev Bosch + Kit",
                Marca = "Bosch",
                Price = "45,00",
                Images = new List<string> { "furadeirabosch.png" },
                Locador = "MS Ferramentas",
                LocadorId = "loc-ms",
                Localizacao = "São Paulo - SP",
                Categoria = "Ferramentas Elétricas • Parafusadeira/Furadeira",
                Status = StatusFerramenta.Locada,
                CadastradoEm = "02/02/2026",
                Avaliacoes = new List<AvaliacaoProduto>
                {
                    new() { Nome = "José Antônio", Nota = 5 },
                    new() { Nome = "Fernanda Aguiar", Nota = 5 },
                    new() { Nome = "Ricardo Peixoto", Nota = 4 },
                }
            },
            new Produto
            {
                Id = 9,
                Title = "Serra Mármore Makita",
                Marca = "Makita",
                Price = "65,00",
                Images = new List<string> { "serramarmoremakita.png" },
                Locador = "MS Ferramentas",
                LocadorId = "loc-ms",
                Localizacao = "São Paulo - SP",
                Categoria = "Ferramentas Elétricas • Corte e Desgaste",
                Status = StatusFerramenta.Manutencao,
                CadastradoEm = "05/02/2026",
                Avaliacoes = new List<AvaliacaoProduto>
                {
                    new() { Nome = "Marcos Vinícius", Nota = 5 },
                    new() { Nome = "Débora Salles", Nota = 4 },
                    new() { Nome = "Otávio Barros", Nota = 4 },
                }
            },
            new Produto
            {
                Id = 10,
                Title = "Lixadeira Orbital Deko",
                Marca = "Deko",
                Price = "25,00",
                Images = new List<string> { "lixadeira_orbital.png" },
                Locador = "WZ Ferramentas",
                LocadorId = "loc-wz",
                Localizacao = "São Paulo - SP",
                Categoria = "Ferramentas Elétricas • Corte e Desgaste",
                Status = StatusFerramenta.Disponivel,
                CadastradoEm = "08/02/2026",
                Avaliacoes = new List<AvaliacaoProduto>
                {
                    new() { Nome = "Simone Ávila", Nota = 4 },
                    new() { Nome = "Rodrigo Esteves", Nota = 3 },
                    new() { Nome = "Priscila Gouveia", Nota = 4 },
                }
            },
            new Produto
            {
                Id = 11,
                Title = "Lixadeira Teto E Parede Telescópica Profissional Com Led E Saco Coletor The Black Tools",
                Marca = "The Black Tools",
                Price = "50,00",
                Images = new List<string> { "lixadeirateto.png" },
                Locador = "WZ Ferramentas",
                LocadorId = "loc-wz",
                Localizacao = "São Paulo - SP",
                Categoria = "Ferramentas Elétricas • Corte e Desgaste",
                Status = StatusFerramenta.Indisponivel,
                CadastradoEm = "10/02/2026",
                Avaliacoes = new List<AvaliacaoProduto>
                {
                    new() { Nome = "Wagner Siqueira", Nota = 4 },
                    new() { Nome = "Cristiane Moraes", Nota = 4 },
                    new() { Nome = "Alexandre Prado", Nota = 3 },
                }
            },
            new Produto
            {
                Id = 12,
                Title = "Pistola Pintura Gravidade 600ml + 3 Bicos The Black Tools",
                Marca = "The Black Tools",
                Price = "30,00",
                Images = new List<string> { "pistolapintura2.png" },
                Locador = "WZ Ferramentas",
                LocadorId = "loc-wz",
                Localizacao = "São Paulo - SP",
                Categoria = "Ferramentas Elétricas • Pintura",
                Status = StatusFerramenta.Locada,
                CadastradoEm = "12/02/2026",
                Avaliacoes = new List<AvaliacaoProduto>
                {
                    new() { Nome = "Adriano Melo", Nota = 4 },
                    new() { Nome = "Letícia Farias", Nota = 5 },
                    new() { Nome = "Igor Damasceno", Nota = 3 },
                }
            },
        };
    }
}
