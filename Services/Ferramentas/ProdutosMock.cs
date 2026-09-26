using LOCATEM_DESKTOP.Models.Ferramentas;

namespace LOCATEM_DESKTOP.Services.Ferramentas
{
    // Catálogo central de ferramentas — migrado de mocks/produtos.mock.ts. Segue o mesmo padrão já adotado em Services/Auth/UsuariosMock.cs: fonte inicial de dados enquanto o projeto não tem uma API de catálogo (no React o CatalogoProvider também parte deste mesmo mock).
    //
    // Os nomes de imagem seguem a sanitização do MAUI para recursos (minúsculas, sem caracteres inválidos) sobre os arquivos já existentes em Resources/Images/ProdutosImg.
    // Descrições e especificações dos anúncios iniciais são as mesmas de produtos.mock.ts.
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
                Images = new List<string> { "furadeira_theblack_tools.png" },
                Locador = "MS Ferramentas",
                LocadorId = "loc-ms",
                Localizacao = "São Paulo - SP",
                Categoria = "Ferramentas Elétricas • Parafusadeira/Furadeira",
                Status = StatusFerramenta.Disponivel,
                CadastradoEm = "03/01/2026",
                Descricao = "Furadeira parafusadeira compacta a bateria 12V, ideal para montagem de móveis, fixação de prateleiras e pequenos reparos domésticos. Leve e de fácil manuseio, entrega torque suficiente para madeira e metais finos sem o peso das furadeiras profissionais.",
                Especificacoes = new List<EspecificacaoFerramenta>
                {
                    new() { Label = "Tensão da bateria", Valor = "12V Li-íon" },
                    new() { Label = "Torque máximo", Valor = "18 Nm" },
                    new() { Label = "Velocidade sem carga", Valor = "0 – 1350 rpm" },
                    new() { Label = "Mandril", Valor = "10mm (aperto rápido)" },
                    new() { Label = "Peso", Valor = "1,1 kg" },
                },
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
                Images = new List<string> { "pistola_pintura.png" },
                Locador = "WZ Ferramentas",
                LocadorId = "loc-wz",
                Localizacao = "São Paulo - SP",
                Categoria = "Ferramentas Elétricas • Pintura",
                Status = StatusFerramenta.Disponivel,
                CadastradoEm = "15/01/2026",
                Descricao = "Pistola de pintura por sucção com reservatório de 1000ml, indicada para pintura de portões, muros, móveis e superfícies grandes. Acompanha 3 bicos intercambiáveis para ajustar a viscosidade da tinta. Requer compressor de ar (não incluso).",
                Especificacoes = new List<EspecificacaoFerramenta>
                {
                    new() { Label = "Capacidade do reservatório", Valor = "1000 ml" },
                    new() { Label = "Sistema de alimentação", Valor = "Sucção (gravidade inferior)" },
                    new() { Label = "Bicos inclusos", Valor = "1.2 / 1.5 / 1.8 mm" },
                    new() { Label = "Pressão de trabalho recomendada", Valor = "3 – 4 bar" },
                    new() { Label = "Consumo de ar", Valor = "~120 L/min" },
                },
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
                Images = new List<string> { "furadeira_hanabi.png" },
                Locador = "JB Ferramentas",
                LocadorId = "loc-jb",
                Localizacao = "São Paulo - SP",
                Categoria = "Ferramentas Elétricas • Parafusadeira/Furadeira",
                Status = StatusFerramenta.Disponivel,
                CadastradoEm = "20/01/2026",
                Descricao = "Parafusadeira/furadeira de impacto profissional com motor brushless (sem escovas), 45N·m de torque e 25 níveis de ajuste. Acompanha 2 baterias para uso contínuo. Perfura madeira, metal e alvenaria com função de impacto, ideal para reformas e montagem de móveis planejados.",
                Especificacoes = new List<EspecificacaoFerramenta>
                {
                    new() { Label = "Motor", Valor = "Brushless (sem escovas)" },
                    new() { Label = "Tensão nominal", Valor = "21V" },
                    new() { Label = "Torque máximo", Valor = "45 N·m" },
                    new() { Label = "Velocidade sem carga", Valor = "0–400 / 0–1450 rpm" },
                    new() { Label = "Mandril", Valor = "1,0 – 10mm (aço)" },
                    new() { Label = "Ajuste de torque", Valor = "25 posições" },
                },
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
                Images = new List<string> { "aparador_grama.png" },
                Locador = "JB Ferramentas",
                LocadorId = "loc-jb",
                Localizacao = "São Paulo - SP",
                Categoria = "Jardinagem e Paisagismo",
                Status = StatusFerramenta.Disponivel,
                CadastradoEm = "22/01/2026",
                Descricao = "Aparador de grama elétrico bipartido, ideal para acabamentos precisos em cantos e áreas de difícil acesso que o cortador tradicional não alcança. Braço bipartido facilita o armazenamento, e o abastecimento automático de fio evita interrupções durante o uso.",
                Especificacoes = new List<EspecificacaoFerramenta>
                {
                    new() { Label = "Potência", Valor = "1500W" },
                    new() { Label = "Rotação", Valor = "11.000 rpm" },
                    new() { Label = "Diâmetro de corte", Valor = "28 cm" },
                    new() { Label = "Fio de nylon", Valor = "1,8mm com abastecimento automático" },
                    new() { Label = "Comprimento do cabo", Valor = "8 metros" },
                    new() { Label = "Peso", Valor = "3,1 kg" },
                },
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
                Images = new List<string> { "aparedor_grama_carrinho.png" },
                Locador = "JB Ferramentas",
                LocadorId = "loc-jb",
                Localizacao = "São Paulo - SP",
                Categoria = "Jardinagem e Paisagismo",
                Status = StatusFerramenta.Locada,
                CadastradoEm = "25/01/2026",
                Descricao = "Cortador de grama elétrico com chassi metálico e motor de 2500W, indicado para gramados de até 2.500m². Possui 4 alturas de corte reguláveis, rodas revestidas de borracha e lâmina posicionada acima do chassi para maior segurança do operador.",
                Especificacoes = new List<EspecificacaoFerramenta>
                {
                    new() { Label = "Potência", Valor = "2500W (2,0 HP)" },
                    new() { Label = "Rotação", Valor = "3.420 rpm" },
                    new() { Label = "Diâmetro de corte", Valor = "450 mm" },
                    new() { Label = "Alturas de corte", Valor = "28 / 42 / 55 / 68 mm" },
                    new() { Label = "Chassi", Valor = "Metálico, pintura eletrostática" },
                    new() { Label = "Peso", Valor = "~24,9 kg" },
                },
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
                Images = new List<string> { "serra_circular_profissional.png" },
                Locador = "JB Ferramentas",
                LocadorId = "loc-jb",
                Localizacao = "São Paulo - SP",
                Categoria = "Ferramentas Elétricas • Corte e Desgaste",
                Status = StatusFerramenta.Manutencao,
                CadastradoEm = "28/01/2026",
                Descricao = "Serra circular profissional com motor de cobre puro 1800W e disco de 185mm com 24 dentes, ideal para cortes retos e angulados em madeira, compensado e MDF. Guia a laser auxilia a precisão do corte e a base de aço garante estabilidade em bancada.",
                Especificacoes = new List<EspecificacaoFerramenta>
                {
                    new() { Label = "Potência", Valor = "1800W" },
                    new() { Label = "Diâmetro do disco", Valor = "185mm (24 dentes)" },
                    new() { Label = "Rotação sem carga", Valor = "~4.700 rpm" },
                    new() { Label = "Profundidade de corte a 90°", Valor = "63,5 mm" },
                    new() { Label = "Profundidade de corte a 45°", Valor = "45 mm" },
                    new() { Label = "Guia", Valor = "Laser integrado" },
                },
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
                Images = new List<string> { "furadeira_wap_cinza.png" },
                Locador = "JB Ferramentas",
                LocadorId = "loc-jb",
                Localizacao = "São Paulo - SP",
                Categoria = "Ferramentas Elétricas • Parafusadeira/Furadeira",
                Status = StatusFerramenta.Indisponivel,
                CadastradoEm = "30/01/2026",
                Descricao = "Parafusadeira e furadeira compacta a bateria 12V, indicada para pequenas reformas e manutenção doméstica. Vem com maleta organizadora e kit de brocas e bits, com seletor de torque de 18 níveis para parafusar e 1 nível para perfurar.",
                Especificacoes = new List<EspecificacaoFerramenta>
                {
                    new() { Label = "Bateria", Valor = "Li-íon 12V 1500mAh removível" },
                    new() { Label = "Torque máximo", Valor = "17 Nm" },
                    new() { Label = "Níveis de torque", Valor = "18 (parafusar) + 1 (perfurar)" },
                    new() { Label = "Velocidade sem carga", Valor = "0 – 1400 rpm" },
                    new() { Label = "Mandril", Valor = "3/8\" – 10mm" },
                },
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
                Images = new List<string> { "furadeira_bosch.png" },
                Locador = "MS Ferramentas",
                LocadorId = "loc-ms",
                Localizacao = "São Paulo - SP",
                Categoria = "Ferramentas Elétricas • Parafusadeira/Furadeira",
                Status = StatusFerramenta.Locada,
                CadastradoEm = "02/02/2026",
                Descricao = "Furadeira de impacto Bosch GSB 450 RE, compacta e reversível, perfura com e sem impacto em concreto, madeira e metal. Botão comutador permite alternar entre furação e parafusamento, com botão-trava para trabalhos contínuos sem fadiga.",
                Especificacoes = new List<EspecificacaoFerramenta>
                {
                    new() { Label = "Potência", Valor = "450W" },
                    new() { Label = "Mandril", Valor = "3/8\" – 10mm" },
                    new() { Label = "Velocidade sem carga", Valor = "0 – 3.100 rpm" },
                    new() { Label = "Torque máximo", Valor = "14,73 Nm" },
                    new() { Label = "Taxa de impacto", Valor = "0 – 49.600 ipm" },
                    new() { Label = "Peso", Valor = "1,6 kg" },
                },
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
                Images = new List<string> { "serra_marmore_makita.png" },
                Locador = "MS Ferramentas",
                LocadorId = "loc-ms",
                Localizacao = "São Paulo - SP",
                Categoria = "Ferramentas Elétricas • Corte e Desgaste",
                Status = StatusFerramenta.Manutencao,
                CadastradoEm = "05/02/2026",
                Descricao = "Serra mármore Makita 4100NH3ZX2, compacta e leve, indicada para corte de mármore, granito, porcelanato, concreto e tijolos. Dupla isolação e estrutura reforçada garantem segurança em trabalhos de marmoraria e construção civil.",
                Especificacoes = new List<EspecificacaoFerramenta>
                {
                    new() { Label = "Potência", Valor = "1.300W" },
                    new() { Label = "Diâmetro do disco", Valor = "110mm" },
                    new() { Label = "Rotação", Valor = "13.800 rpm" },
                    new() { Label = "Capacidade máxima de corte", Valor = "32mm" },
                    new() { Label = "Diâmetro do furo do disco", Valor = "20mm" },
                    new() { Label = "Peso", Valor = "~2,9 kg" },
                },
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
                Descricao = "Lixadeira roto-orbital Deko DKOS32G125, combina rotação e vibração para um acabamento mais uniforme em madeira, funilaria e pequenos reparos de pintura. Troca de lixas por velcro e coletor de pó para manter o ambiente mais limpo.",
                Especificacoes = new List<EspecificacaoFerramenta>
                {
                    new() { Label = "Potência", Valor = "320W" },
                    new() { Label = "Rotação", Valor = "7.000 – 14.000 rpm" },
                    new() { Label = "Base da lixa", Valor = "125mm (5\")" },
                    new() { Label = "Fixação da lixa", Valor = "Velcro" },
                    new() { Label = "Níveis de velocidade", Valor = "6" },
                },
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
                Images = new List<string> { "lixadeira_teto.png" },
                Locador = "WZ Ferramentas",
                LocadorId = "loc-wz",
                Localizacao = "São Paulo - SP",
                Categoria = "Ferramentas Elétricas • Corte e Desgaste",
                Status = StatusFerramenta.Indisponivel,
                CadastradoEm = "10/02/2026",
                Descricao = "Lixadeira telescópica para teto e parede, com iluminação LED integrada e saco coletor de pó, indicada para lixamento de massa corrida e gesso antes da pintura. Haste extensível dispensa o uso de escadas em pés-direitos altos.",
                Especificacoes = new List<EspecificacaoFerramenta>
                {
                    new() { Label = "Potência", Valor = "750W" },
                    new() { Label = "Disco de lixa", Valor = "225mm" },
                    new() { Label = "Iluminação", Valor = "LED integrado" },
                    new() { Label = "Haste", Valor = "Telescópica extensível" },
                    new() { Label = "Coleta de pó", Valor = "Saco coletor acoplado" },
                },
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
                Images = new List<string> { "pistola_pintura_second.png" },
                Locador = "WZ Ferramentas",
                LocadorId = "loc-wz",
                Localizacao = "São Paulo - SP",
                Categoria = "Ferramentas Elétricas • Pintura",
                Status = StatusFerramenta.Locada,
                CadastradoEm = "12/02/2026",
                Descricao = "Pistola de pintura por gravidade com reservatório de 600ml, indicada para pintura de móveis, portas e peças menores que exigem mais precisão que a pistola de sucção. Acompanha 3 bicos para diferentes viscosidades de tinta. Requer compressor de ar (não incluso).",
                Especificacoes = new List<EspecificacaoFerramenta>
                {
                    new() { Label = "Capacidade do reservatório", Valor = "600 ml" },
                    new() { Label = "Sistema de alimentação", Valor = "Gravidade (reservatório superior)" },
                    new() { Label = "Bicos inclusos", Valor = "1.2 / 1.5 / 1.8 mm" },
                    new() { Label = "Pressão de trabalho recomendada", Valor = "3 – 4 bar" },
                },
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
