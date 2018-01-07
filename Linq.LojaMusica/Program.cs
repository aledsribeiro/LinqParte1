using Linq.LojaMusica.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Linq.LojaMusica
{
    class Program
    {
        static void Main(string[] args)
        {
            Aula13();
        }

        /// <summary>
        /// Select - Linq to objects
        /// </summary>
        private static void Aula1()
        {
            var generos = new List<Genero>()
            {
                new Genero { Id = 1, Nome = "Rock"},
                new Genero { Id = 2, Nome =  "Reggae"},
                new Genero { Id = 3, Nome =  "Rock Progressivo"},
                new Genero { Id = 4, Nome =  "Punk"},
                new Genero { Id = 5, Nome =  "Clássica"}
            };


            //foreach (var item in generos)
            //{
            //    if (item.Nome.Contains("Rock"))
            //        Console.WriteLine("{0}\t{1}", item.Id, item.Nome);
            //}

            //select * from generos

            var query = from g in generos
                        where g.Nome.Contains("Rock")
                        select g;

            foreach (var item in query)
            {
                Console.WriteLine("{0}\t{1}", item.Id, item.Nome);
            }

            Console.ReadKey();
        }

        /// <summary>
        /// Select com join - Linq to objects
        /// </summary>
        private static void Aula2()
        {
            var generos = new List<Genero>()
            {
                new Genero { Id = 1, Nome = "Rock"},
                new Genero { Id = 2, Nome =  "Reggae"},
                new Genero { Id = 3, Nome =  "Rock Progressivo"},
                new Genero { Id = 4, Nome =  "Punk"},
                new Genero { Id = 5, Nome =  "Clássica"}
            };

            var musicas = new List<Musica>()
            {
                new Musica { Id = 1, Nome = "Sweet Child O'Mine", GeneroId = 1 },
                new Musica { Id = 2, Nome = "I Shoot The Sheriff", GeneroId = 2},
                new Musica { Id = 3, Nome = "Danúbio Azul", GeneroId = 5},
            };

            var query = from m in musicas
                        join g in generos
                        on m.GeneroId equals g.Id
                        /*objeto anonimo*/
                        select new { m, g };

            foreach (var item in query)
            {
                Console.WriteLine("{0}\t{1}\t{2}", item.m.Id, item.m.Nome, item.g.Nome);
            }

            Console.ReadKey();

        }

        /// <summary>
        /// Select com join - Linq to XML
        /// </summary>
        private static void Aula3()
        {
            XElement root = XElement.Load(@"c:/users/5a_pc1/documents/visual studio 2012/Projects/Linq/Linq.LojaMusica/Tunes.xml");

            //trazendo elementos do genero
            var querySQL =
                from g in root.Element("Generos").Elements("Genero")
                select g;


            var queryGeneros = from g in root.Element("Generos").Elements("Genero")
                               join m in root.Element("Musicas").Elements("Musica")
                               on g.Element("GeneroId").Value equals m.Element("GeneroId").Value
                               select new
                               {
                                   musica = m.Element("Nome").Value,
                                   genero = g.Element("Nome").Value
                               };


            foreach (var item in queryGeneros)
            {
                Console.WriteLine("{0}\t{1}", item.musica, item.genero);
            }

            Console.ReadKey();
        }

        /// <summary>
        /// Select com join - Linq to entities
        /// </summary>
        private static void Aula4()
        {
            using (var contexto = new TunesEntities())
            {
                var query = from g in contexto.Generos
                            select g;

                //trouxe muitos dados
                var queryFaixaGenero = from g in contexto.Generos
                                       join f in contexto.Faixas
                                       on g.GeneroId equals f.GeneroId
                                       select new { g, f };

                //traz somente 10 elementos
                queryFaixaGenero = queryFaixaGenero.Take(10);

                //configuração para ver tudo que é gerado no link para o sql
                contexto.Database.Log = Console.WriteLine;


                foreach (var item in queryFaixaGenero)
                {
                    Console.WriteLine("{0}\t{1}", item.f.Nome, item.g.Nome);
                }

                Console.ReadKey();
            }
        }

        /// <summary>
        /// Linq to entities - Where com Lambda
        /// </summary>
        private static void Aula5()
        {
            using (var contexto = new TunesEntities())
            {
                string textoBusca = "Led";

                //sintaxe de consulta
                //usada para consultas mais complexas
                var query = from a in contexto.Artistas
                            where a.Nome.Contains(textoBusca)
                            select a;

                foreach (var item in query)
                {
                    Console.WriteLine("{0}\t{1}", item.ArtistaId, item.Nome);
                }

                //sintaxe de metodo
                //select esta implicito
                //usada mais para consultas simples
                var query2 = contexto.Artistas.Where(a => a.Nome.Contains(textoBusca));

                Console.WriteLine();

                foreach (var item in query2)
                {
                    Console.WriteLine("{0}\t{1}", item.ArtistaId, item.Nome);
                }

                Console.ReadKey();

            }
        }

        /// <summary>
        /// Linq to entites  - Join
        /// </summary>
        private static void Aula6()
        {
            using (var contexto = new TunesEntities())
            {
                string textoBusca = "Led";

                //usando consulta com join para casos em que não é possivel ler 
                //as propriedades de navegação e onde não existe o relacionamento
                //pela chave que esta sendo comparada
                var query = from a in contexto.Artistas
                            join alb in contexto.Albums
                            on a.ArtistaId equals alb.ArtistaId
                            where a.Nome.Contains(textoBusca)
                            select new
                            {
                                NomeArtista = a.Nome,
                                NomeAlbum = alb.Titulo
                            };

                foreach (var item in query)
                {
                    Console.WriteLine("{0}\t{1}", item.NomeArtista, item.NomeAlbum);
                }

                Console.WriteLine();

                //consulta sem join usando navigation properties
                //consulta mais limpa, podemos usar sem o join encadeando as propriedades de navegação
                var query2 = from alb in contexto.Albums
                             where alb.Artista.Nome.Contains(textoBusca)
                             select new
                             {
                                 NomeArtista = alb.Artista.Nome,
                                 NomeAlbum = alb.Titulo
                             };

                foreach (var item in query2)
                {
                    Console.WriteLine("{0}\t{1}", item.NomeArtista, item.NomeAlbum);
                }

                Console.ReadKey();

            }
        }

        /// <summary>
        /// Linq to entities - Refinando Consultas com parametros
        /// </summary>
        private static void Aula7()
        {
            string textoBusca = "Led Zeppelin";
            string textoBuscaAlbum = "Graffiti";
            using (var contexto = new TunesEntities())
            {
                GetFaixas(textoBusca, contexto, textoBuscaAlbum);

                Console.ReadKey();
            }
        }

        /// <summary>
        /// Lin to entities orderby com sintaxe de consulta e sintaxe de metodo
        /// </summary>
        private static void Aula8()
        {
            //feito alteração no método getfaixas
            //nunca fazer filtro depois  de uma ordenação
        }

        /// <summary>
        /// Linq to entities com count
        /// </summary>
        private static void Aula9()
        {
            using (var contexto = new TunesEntities())
            {
                var query = from f in contexto.Faixas
                            where f.Album.Artista.Nome == "Led Zeppelin"
                            select f;

                //quantas faixas existem em um album

                //var quantidade = query.Count();

                var quantidade = contexto.Faixas.Count(f => f.Album.Artista.Nome == "Led Zeppelin");

                Console.WriteLine(quantidade);

                //foreach (var item in query)
                //{
                //    Console.WriteLine("{0}\t", item.Nome);
                //}

                Console.WriteLine();

                Console.ReadKey();
            }
        }

        /// <summary>
        /// Linq to entities com Sum
        /// </summary>
        private static void Aula10()
        {
            using (var contexto = new TunesEntities())
            {
                //calcular total de vendas por artista
                var query = from inf in contexto.ItemNotaFiscal
                            where inf.Faixa.Album.Artista.Nome.Contains("Led Zeppelin")
                            select new
                            {
                                totalItem = inf.Quantidade * inf.PrecoUnitario
                            };

                //preciso pegar o que retorna da query acima e somar
                var totalDoArtista = query.Sum(q => q.totalItem);

                Console.WriteLine(totalDoArtista);

                Console.ReadKey();
            }
        }

        /// <summary>
        /// Linq to entities groupby
        /// criar um relatório para listar os álbuns mais vendidos de um artista
        /// </summary>
        private static void Aula11()
        {
            using (var contexto = new TunesEntities())
            {
                var query = from inf in contexto.ItemNotaFiscal
                            where inf.Faixa.Album.Artista.Nome == "Led Zeppelin"
                            group inf by inf.Faixa.Album into agrupado
                            //aqui temos repetição da logica de soma então precisamos criar uma variavel
                            //variavel dentro de uma consulta linq - let(variavel interna dentro da consulta linq
                            orderby agrupado.Sum(a => a.Quantidade * a.PrecoUnitario) descending
                            select new
                            {
                                TituloAlbum = agrupado.Key.Titulo,
                                TotalPorAlbum = agrupado.Sum(q => q.Quantidade * q.PrecoUnitario)
                            };

                //com let
                var query2 = from inf in contexto.ItemNotaFiscal
                             where inf.Faixa.Album.Artista.Nome == "Led Zeppelin"
                             group inf by inf.Faixa.Album into agrupado
                             //aqui temos repetição da logica de soma então precisamos criar uma variavel
                             //variavel dentro de uma consulta linq - let(variavel interna dentro da consulta linq
                             let vendaPorAlbum = agrupado.Sum(q => q.Quantidade * q.PrecoUnitario)
                             orderby vendaPorAlbum descending
                             select new
                             {
                                 TituloAlbum = agrupado.Key.Titulo,
                                 TotalPorAlbum = vendaPorAlbum
                             };

                foreach (var item in query)
                {
                    Console.WriteLine("{0}\t{1}",
                        item.TituloAlbum.PadRight(40),
                        item.TotalPorAlbum
                       );
                }

                Console.ReadKey();

            }
        }

        /// <summary>
        /// Linq to entities min max e avg
        /// calcular o "preço da maior", "menor venda" e "venda média".
        /// </summary>
        private static void Aula12()
        {
            using (var contexto = new TunesEntities())
            {
                //para ver as consultas que estão sendo geradas
                contexto.Database.Log = Console.WriteLine;

                //faz 3 chamadas no banco
                var maiorVenda = contexto.NotaFiscals.Max(nf => nf.Total);
                var menorVenda = contexto.NotaFiscals.Min(nf => nf.Total);
                var mediaVenda = contexto.NotaFiscals.Average(nf => nf.Total);

                Console.WriteLine("A maior venda é de R$ {0}", maiorVenda);
                Console.WriteLine("A menor venda é de R$ {0}", menorVenda);
                Console.WriteLine("A media venda é de R$ {0}", mediaVenda);

                //para trazer todas as consultas de uma vez
                //o single transforma a consulta em um objeto
                var vendas = (from nf in contexto.NotaFiscals
                              group nf by 1 into agrupado
                              select new
                              {
                                  MaiorVenda = agrupado.Max(nf => nf.Total),
                                  MenorVenda = agrupado.Min(nf => nf.Total),
                                  VendaMedia = agrupado.Average(nf => nf.Total)
                              }).Single();

                Console.WriteLine("A maior venda é de R$ {0}", vendas.MaiorVenda);
                Console.WriteLine("A menor venda é de R$ {0}", vendas.MenorVenda);
                Console.WriteLine("A media venda é de R$ {0}", vendas.VendaMedia);

            }

            Console.ReadKey();
        }

        /// <summary>
        /// Linq métodos extensão
        /// cliente requisitou o cálculo da mediana das vendas.
        /// </summary>
        private static void Aula13()
        {
            using (var contexto = new TunesEntities())
            {
                var vendaMedia = contexto.NotaFiscals.Average(nf => nf.Total);

                Console.WriteLine("Venda Media {0}", vendaMedia);

                //para não usarmos dessa forma vamos criar um metodo de extensão
                var query =
                     from nf in contexto.NotaFiscals
                     select nf.Total;

                //usando o metodo de extensão criado
                var vendaMediana = contexto.NotaFiscals.Mediana(nf => nf.Total);

                Console.WriteLine("Mediana {0}", vendaMediana);

                Console.ReadKey();
            }
        }
        
        private static void GetFaixas(string textoBusca, TunesEntities contexto, string buscaAlbum)
        {
            var query = from f in contexto.Faixas
                        where f.Album.Artista.Nome.Contains(textoBusca)
                        && (!string.IsNullOrEmpty(buscaAlbum) ?
                        f.Album.Titulo.Contains(buscaAlbum) : true)
                        orderby f.Album.Titulo, f.Nome
                        select f;

            //if(!string.IsNullOrEmpty(buscaAlbum))
            //{
            //    query = query.Where(a => a.Album.Titulo.Contains(buscaAlbum));
            //}

            //aula 8 - thenby ordenação secundaria
            //query = query.OrderBy(q => q.Album.Titulo).ThenBy(q=> q.Nome);

            foreach (var item in query)
            {
                Console.WriteLine("{0}\t{1}", item.Album.Titulo.PadRight(40), item.Nome);
            }
        }

        public class Genero
        {
            public int Id { get; set; }
            public string Nome { get; set; }
        }

        public class Musica
        {
            public int Id { get; set; }
            public string Nome { get; set; }
            public int GeneroId { get; set; }
        }
        
    }

    /// <summary>
    /// A mediana consiste em pegar uma lista, ordenar linearmente seus
    /// valores e selecionar os elementos centrais. No caso de objetos em
    /// quantidade par, os dois itens que ocupam o centro são somados e
    /// divididos para resultar no valor da mediana. No caso de um valor
    /// ímpar, basta selecionar o item central
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    public static class LinqExtensions
    {
        public static decimal Mediana<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, decimal>> selector)
        {
            var contagem = source.Count();
            var functionSelector = selector.Compile();

            var queryOrdenada = source.Select(functionSelector).OrderBy(total => total);

            var elementoCentral1 = queryOrdenada.Skip(contagem / 2).First();

            var elementoCentral2 = queryOrdenada.Skip((contagem - 1) / 2).First();

            var mediana = (elementoCentral1 + elementoCentral2) / 2;

            return mediana;
        }
    }

}


