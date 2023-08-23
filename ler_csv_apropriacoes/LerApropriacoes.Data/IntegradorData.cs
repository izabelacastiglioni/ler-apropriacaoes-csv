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

        public string DataEventoFinal { get; set; }

        public string MensagemErro { get; set; }
        public string ConnectionString { get; private set; }

        public IntegradorData(DateTime dataEventoInicial,string dataEventoFinal, string menssagemErro)
        {
            DataEventoInicial = dataEventoInicial;

            ConnectionString = ConfigurationManager.ConnectionStrings["IntegradorConnection"].ConnectionString;

            MensagemErro = menssagemErro;
        }

        public async Task<IEnumerable<EventoRecebido>> ListarJson()
        {

            using (var connection = new SqlConnection(ConnectionString))
            {

                var sql = @"select    distinct SUBSTRING(JSON_VALUE(x.value,'$.parcelaId.identificadorCobertura '), 0,13) as ItemCertificado,
		                            JSON_VALUE(x.value,'$.parcelaId.identificadorCobertura ') as ItemCertificadoApolice,
		                            JSON_VALUE(x.value,'$.parcelaId.numeroParcela ') as NumeroParcela,
		                            JSON_VALUE(x.value,'$.parcelaId.faturaId.identificadorExterno ') as FaturaId,
                                    SUBSTRING(JSON_VALUE(x.value,'$.parcelaId.identificadorCobertura '), 13,18) as ItemProduto
                                    from LogEventoRecebido ler
                                    inner join EventoRecebido er on er.Identificador = ler.Identificador
                                    cross apply openjson(DadosEvento,'$.parcelas') x
                                    where     StatusEventoLogado = 1
                                    and er.StatusId=5     
                                    and cast(er.DataEvento as date) = @dataEvento    
                                    and Mensagem like @mensagemErro ";

                var eventos = await connection.QueryAsync<EventoRecebido>(sql, (dataEvento: DataEvento, mensagemErro: MensagemErro), commandTimeout: 100000);


                return eventos;

            }
        }
    }
}
