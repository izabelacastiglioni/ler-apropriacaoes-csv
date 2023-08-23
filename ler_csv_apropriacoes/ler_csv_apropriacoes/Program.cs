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

            DateTime dataEventoInicial = new DateTime(2023, 08, 01);

            DateTime dataEventoFinal = new DateTime(2023, 08, 02);

            String mensagemErro = "%Não existe movimentacao anteriror.%";

            string arquivo = $"..\\..\\..\\..\\Arquivos\\WI_417947.csv";

            var integrador = new IntegradorData(dataEventoInicial,dataEventoFinal, mensagemErro);

            var eventos = await integrador.ListarEventos();

            var csv = new CSVData(arquivo);

            await csv.LerArquivoCSV(eventos,mensagemErro,dataEventoInicial,dataEventoFinal);



        }
    }
}
