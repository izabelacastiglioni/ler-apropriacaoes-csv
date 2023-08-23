using LerCsvApropriacoes.Dominio;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using System;

namespace LerApropriacoes.Data
{
    public class IntegradorData
    {
        public DateTime DataEventoInicial { get; set; }

        public DateTime DataEventoFinal { get; set; }

        public string MensagemErro { get; set; }
        public string ConnectionString { get; private set; }

        public IntegradorData(DateTime dataEventoInicial,DateTime dataEventoFinal, string menssagemErro)
        {
            DataEventoInicial = dataEventoInicial;

            DataEventoFinal = dataEventoFinal;

            ConnectionString = ConfigurationManager.ConnectionStrings["IntegradorConnection"].ConnectionString;

            MensagemErro = menssagemErro;
        }

        public async Task<IEnumerable<EventoRecebido>> ListarEventos()
        {

            using (var connection = new SqlConnection(ConnectionString))
            {

                var sql = @"select distinct
		                            JSON_VALUE(x.value,'$.parcelaId.identificadorCobertura ') as ItemCertificadoApolice,
		                            JSON_VALUE(x.value,'$.parcelaId.numeroParcela ') as NumeroParcela
		                            from LogEventoRecebido ler
                                    inner join EventoRecebido er on er.Identificador = ler.Identificador
                                    cross apply openjson(DadosEvento,'$.parcelas') x
                                    where     StatusEventoLogado = 1
                                    and er.StatusId=5     
                                    and cast(er.DataEvento as date) between @dataEventoInicial and @dataEventoFinal
                                    and Mensagem like @mensagemErro
                                    ";

                //var eventos = await connection.QueryAsync<EventoRecebido>(sql, new { dataEventoInicial = DataEventoInicial.ToString("yyyy-MM-dd"), dataEventoFinal = DataEventoFinal.ToString("yyyy-MM-dd"), mensagemErro = MensagemErro}, commandTimeout: 100000);
                var eventos = await connection.QueryAsync<EventoRecebido>(sql, new { dataEventoInicial = DataEventoInicial.ToString("yyyy-MM-dd"), dataEventoFinal = DataEventoFinal.ToString("yyyy-MM-dd"), mensagemErro = MensagemErro }, commandTimeout: 100000);


                return eventos;

            }
        }
    }
}
