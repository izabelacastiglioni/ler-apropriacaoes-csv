using LerApropriacoes.Data;
using System;
using System.Threading.Tasks;

namespace DuplicidadeMovimentoPremio
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Iniciação de arquivos duplicados do premio");



            DateTime dataEventoInicial = new DateTime(2023, 11, 01);

            DateTime dataEventoFinal = new DateTime(2023, 11, 09);

    
            string arquivo = $"..\\..\\..\\..\\Arquivos\\WI_434920.csv";


            var csv = new CSVData(arquivo);

            await csv.LerArquivoCSVDuplicadosPremio( dataEventoInicial, dataEventoFinal);

        }
    }
}
