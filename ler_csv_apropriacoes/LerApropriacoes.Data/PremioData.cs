using Dapper;
using LerCsvApropriacoes.Dominio;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LerApropriacoes.Data
{
    public class PremioData
    {

        public string ConnectionString { get; private set; }

        public PremioData()
        {

            ConnectionString = ConfigurationManager.ConnectionStrings["PremioConnection"].ConnectionString;
        }
        public async Task<IEnumerable<MovimentacaoPremio>> ListarMovimentos( string itemCetificado, int numeroParcela)
        {

            using (var connection = new SqlConnection(ConnectionString))
            {

                var sql = @"SELECT
                        cp.IdExterno as ItemCertificado,
                        p.Numero as Parcela,
                         m.Id as MovimentoId,
                        m.TipoMovimento,
                        e.Id as EventoId,
                        e.Identificador
                    FROM
                    ControleParcela cp
                    join Parcela p on cp.Id = p.ControleParcelaId
                    join Movimento m on p.Id = m.ParcelaId
                    join Evento e on e.Id = m.EventoId
                    where
                        cp.IdExterno = @itemCertificado and p.Numero = @numeroParcela";


                var movimentos = await connection.QueryAsync<MovimentacaoPremio>(sql, new { itemCertificado = itemCetificado, numeroParcela = numeroParcela }, commandTimeout: 100000);
                return movimentos;

            }
              

        }
        public async Task GerarArquivosCorrecao(IEnumerable<EventoRecebido> eventoRecebidos, string mensagem, DateTime dataInicial, DateTime dataFinal, string seguradora)
        {
            StreamWriter swMovimento = new StreamWriter($"..\\..\\..\\..\\Scripts\\{seguradora}-{mensagem}-{dataInicial.ToString("yyyy-MM-dd")}-a-{dataFinal.ToString("yyyy-MM-dd")}-movimento.sql");
            StreamWriter swParcela = new StreamWriter($"..\\..\\..\\..\\Scripts\\{seguradora}-{mensagem}-{dataInicial.ToString("yyyy-MM-dd")}-a-{dataFinal.ToString("yyyy-MM-dd")}-parcela.sql");
            StreamWriter swEvento = new StreamWriter($"..\\..\\..\\..\\Scripts\\{seguradora}-{mensagem}-{dataInicial.ToString("yyyy-MM-dd")}-a-{dataFinal.ToString("yyyy-MM-dd")}-evento.sql");
            StreamWriter swApropriacao = new StreamWriter($"..\\..\\..\\..\\Scripts\\{seguradora}-{mensagem}-{dataInicial.ToString("yyyy-MM-dd")}-a-{dataFinal.ToString("yyyy-MM-dd")}-apropriacoes.sql");
            List<string> eventoIdAnterior = new List<string>();
            List<string> identificadorAnterior = new List<string>();

            foreach(var evento in eventoRecebidos)
            {

                var movimentos = await ListarMovimentos(evento.ItemCertificadoApolice, int.Parse(evento.NumeroParcela));

                foreach (var movimento in movimentos )
                {
                    if (mensagem == "%Não existe movimentacao anteriror.%")
                    {
                        swMovimento.WriteLine($"DELETE Movimento WHERE Id = '{movimento.MovimentoId}';");

                        if (!eventoIdAnterior.Contains(movimento.EventoId.ToString()))
                        {
                            swParcela.WriteLine($"DELETE FROM Parcela WHERE EventoId = '{movimento.EventoId}';");
                            swEvento.WriteLine($"DELETE FROM Evento WHERE Id = '{movimento.EventoId}';");
                            eventoIdAnterior.Add(movimento.EventoId.ToString()); ;
                        }

                        if (movimento.TipoMovimento == "Baixa")
                        {
                            if (!identificadorAnterior.Contains(movimento.Identificador.ToString()))
                            {
                                swApropriacao.WriteLine(movimento.Identificador);
                                identificadorAnterior.Add(movimento.Identificador.ToString());
                            }
                        }
                    }
                    if ((mensagem == "%Impossivel validar movimento de Apropriacao precedido de Cancelamento%") || (mensagem == "%A soma do Valor de contribuição e do desconto não corresponde ao valor de contribuição emitido.%"))
                    {
                        if ((movimento.TipoMovimento == "Reemissao") || (movimento.TipoMovimento == "Baixa") || (movimento.TipoMovimento == "CancelamentoParcela"))
                        {
                            swMovimento.WriteLine($"DELETE Movimento WHERE Id = '{movimento.MovimentoId}';");

                            if (movimento.TipoMovimento == "Baixa")
                            {
                                if (!eventoIdAnterior.Contains(movimento.EventoId.ToString()))
                                {
                                    swParcela.WriteLine($"DELETE FROM Parcela WHERE EventoId = '{movimento.EventoId}';");
                                    swEvento.WriteLine($"DELETE FROM Evento WHERE Id = '{movimento.EventoId}';");
                                    eventoIdAnterior.Add(movimento.EventoId.ToString());
                                }

                                if (!identificadorAnterior.Contains(movimento.Identificador.ToString()))
                                {
                                    swApropriacao.WriteLine(movimento.Identificador);
                                    identificadorAnterior.Add(movimento.Identificador.ToString());
                                }
                            }
                        }
                    }
                }
                    

              
            }

            swMovimento.Close();
            swParcela.Close();
            swEvento.Close();
            swApropriacao.Close();


        }
    }
}
