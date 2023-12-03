using LerApropriacoes.Data;
using System;
using System.Threading.Tasks;

namespace ler_csv_apropriacoes
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Iniciação de leitura de CSV de apropriações");

            DateTime dataEventoInicial = new DateTime(2023, 11, 01);

            DateTime dataEventoFinal = new DateTime(2023, 11, 31);

            String seguradora = "MAG";

            String mensagemErro = "%A soma do Valor de contribuição e do desconto não corresponde ao valor de contribuição emitido.%";

            //string arquivo = $"..\\..\\..\\..\\Arquivos\\formatado2.csv";

            var integrador = new IntegradorData(dataEventoInicial,dataEventoFinal, mensagemErro);

            var eventos = await integrador.ListarEventos();

            var premio = new PremioData();

            await premio.GerarArquivosCorrecao(eventos, mensagemErro, dataEventoInicial, dataEventoFinal, seguradora);

            // var csv = new CSVData(arquivo);

            //await csv.LerArquivoCSV(eventos,dataEventoInicial,dataEventoFinal);



        }
    }
}
