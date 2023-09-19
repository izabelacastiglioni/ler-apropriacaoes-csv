using LerApropriacoes.Data;
using System;
using System.Threading.Tasks;

namespace LerApropriacoesSIXOOB__
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Iniciação de criação de script para as apropriacoes SICOOB");

            String seguradora = "SICOOB";

            DateTime dataEventoInicial = new DateTime(2023, 09, 05);

            DateTime dataEventoFinal = new DateTime(2023, 09, 05);

            String mensagemErro = "%Não existe movimentacao anteriror.%";
            
            var integrador = new IntegradorData(dataEventoInicial, dataEventoFinal, mensagemErro);

            var eventos = await integrador.ListarEventos();

            var premio = new PremioData();

            await premio.GerarArquivosCorrecao(eventos, mensagemErro, dataEventoInicial, dataEventoFinal,seguradora);

        }
    }
}
