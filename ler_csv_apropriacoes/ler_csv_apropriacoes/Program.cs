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

            DateTime dataEventoInicial = new DateTime(2023, 08, 28);

            DateTime dataEventoFinal = new DateTime(2023, 08, 31);

            String mensagemErro = "%Impossivel validar movimento de Apropriacao precedido de Cancelamento%";

            string arquivo = $"..\\..\\..\\..\\Arquivos\\WI_420895.csv";

            var integrador = new IntegradorData(dataEventoInicial,dataEventoFinal, mensagemErro);

            var eventos = await integrador.ListarEventos();

            var csv = new CSVData(arquivo);

            await csv.LerArquivoCSV(eventos,mensagemErro,dataEventoInicial,dataEventoFinal);



        }
    }
}
