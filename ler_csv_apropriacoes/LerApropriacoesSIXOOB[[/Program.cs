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

            DateTime dataEventoInicial = new DateTime(2023, 11, 01);

            DateTime dataEventoFinal = new DateTime(2023, 11, 28);

            String mensagemErro = "%Impossivel validar movimento de Apropriacao precedido de Cancelamento%";
            
            var integrador = new IntegradorData(dataEventoInicial, dataEventoFinal, mensagemErro);

            var eventos = await integrador.ListarEventos();

            var premio = new PremioData();

            await premio.GerarArquivosCorrecao(eventos, mensagemErro, dataEventoInicial, dataEventoFinal,seguradora);

        }
    }
}
