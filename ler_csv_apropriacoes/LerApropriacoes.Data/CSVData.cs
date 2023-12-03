using CsvHelper;
using CsvHelper.Configuration;
using LerCsvApropriacoes.Dominio;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LerApropriacoes.Data
{
    public class CSVData
    {

        public string Arquivo { get; set; }

        public CSVData(string arquivo)
        {
            Arquivo = arquivo;
        }

        public async Task LerArquivoCSV(IEnumerable<EventoRecebido> eventoRecebidos,DateTime dataInicial,DateTime dataFinal)
        {
            StreamWriter swMovimento = new StreamWriter($"..\\..\\..\\..\\Scripts\\{dataInicial.ToString("yyyy-MM-dd")}-a-{dataFinal.ToString("yyyy-MM-dd")}-movimento.sql");
            StreamWriter swParcela = new StreamWriter($"..\\..\\..\\..\\Scripts\\{dataInicial.ToString("yyyy-MM-dd")}-a-{dataFinal.ToString("yyyy-MM-dd")}-parcela.sql");
            StreamWriter swEvento = new StreamWriter($"..\\..\\..\\..\\Scripts\\{dataInicial.ToString("yyyy-MM-dd")}-a-{dataFinal.ToString("yyyy-MM-dd")}-evento.sql");
            StreamWriter swApropriacao = new StreamWriter($"..\\..\\..\\..\\Scripts\\{dataInicial.ToString("yyyy-MM-dd")}-a-{dataFinal.ToString("yyyy-MM-dd")}-apropriacoes.sql");
            List<string> eventoIdAnterior = new List<string>();
            List<string> identificadorAnterior = new List<string>();

            var config = new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = ";", PrepareHeaderForMatch = header => header.Header.ToLower() };
            using (var reader = new StreamReader(Arquivo, Encoding.UTF8))
            using (var csv = new CsvReader(reader, config))
            {

                var records = csv.GetRecords<MovimentacaoPremio>().ToList();

                var distintos = records.Distinct().GroupBy(x=> new { x.MovimentoId,x.EventoId}).Select(x=>x.FirstOrDefault()).ToList();

                foreach (var dist in distintos)
                {
                    foreach (var evento in eventoRecebidos)
                    {
                      
                        if ((dist.ItemCertificado == evento.ItemCertificadoApolice) && (dist.Parcela == evento.NumeroParcela))
                        {
                            if(evento.MenssagemErro.Contains("Não existe movimentacao anteriror."))
                            {
                                swMovimento.WriteLine($"DELETE Movimento WHERE Id = '{dist.MovimentoId}';");

                                if (!eventoIdAnterior.Contains(dist.EventoId.ToString()))
                                {
                                    swParcela.WriteLine($"DELETE FROM Parcela WHERE EventoId = '{dist.EventoId}';");
                                    swEvento.WriteLine($"DELETE FROM Evento WHERE Id = '{dist.EventoId}';");
                                    eventoIdAnterior.Add(dist.EventoId.ToString());
                                }

                                if(dist.TipoMovimento == "Baixa")
                                {
                                    if (!identificadorAnterior.Contains(dist.Identificador.ToString()))
                                    {
                                        swApropriacao.WriteLine(dist.Identificador.ToString());
                                       identificadorAnterior.Add(dist.Identificador.ToString());
                                    }
                                }
                            }
                            if( (evento.MenssagemErro.Contains("Impossivel validar movimento de Apropriacao precedido de Cancelamento")) || (evento.MenssagemErro.Contains("A soma do Valor de contribuição e do desconto não corresponde ao valor de contribuição emitido.")))
                            {
                                if ((dist.TipoMovimento == "Reemissao") || (dist.TipoMovimento == "Baixa") || (dist.TipoMovimento == "CancelamentoParcela") || (dist.TipoMovimento == "CancelamentoAjusteParcela")||(dist.TipoMovimento == "CancelamentoPorDesapropriacao")||(dist.TipoMovimento== "AjusteParcela"))
                                {
                                    swMovimento.WriteLine($"DELETE Movimento WHERE Id = '{dist.MovimentoId}';");                                   

                                    if (dist.TipoMovimento == "Baixa")
                                    {
                                        if (!eventoIdAnterior.Contains(dist.EventoId.ToString()))
                                        {
                                            swParcela.WriteLine($"DELETE FROM Parcela WHERE EventoId = '{dist.EventoId}';");
                                            swEvento.WriteLine($"DELETE FROM Evento WHERE Id = '{dist.EventoId}';");
                                            eventoIdAnterior.Add(dist.EventoId.ToString());
                                        }

                                        if (!identificadorAnterior.Contains(dist.Identificador.ToString()))
                                        {
                                            swApropriacao.WriteLine(dist.Identificador.ToString());
                                            identificadorAnterior.Add(dist.Identificador.ToString());
                                        }
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

            Console.WriteLine("Arquivo de deletar movimentação gerado com sucesso!!");

        }


        public async Task LerArquivoCSVDuplicadosPremio(DateTime dataInicial, DateTime dataFinal)
        {
            StreamWriter swMovimento = new StreamWriter($"..\\..\\..\\..\\Scripts\\{dataInicial.ToString("yyyy-MM-dd")}-a-{dataFinal.ToString("yyyy-MM-dd")}-movimentoDuplicados.sql");
         
           List<string>  movimentosAnteriores  = new List<string>();
            List<string> eventosAnteriores = new List<string>();

            var config = new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = ",", PrepareHeaderForMatch = header => header.Header.ToLower() };
            using (var reader = new StreamReader(Arquivo, Encoding.UTF8))
            using (var csv = new CsvReader(reader, config))
            {

                var records = csv.GetRecords<MovimentacaoPremio>().ToList();

                var duplicados = records.GroupBy(x => new { x.ItemCertificado, x.Parcela, x.TipoMovimento }).SelectMany(g => g.Skip(1)); ;

                foreach (var duplicado in duplicados)
                {             
                    
                        swMovimento.WriteLine($"DELETE Movimento WHERE Id = '{duplicado.MovimentoId}';");                      
                       
                    
                                      
                }

                swMovimento.Close();
                        
            }

            Console.WriteLine("Arquivo de deletar movimentação gerado com sucesso!!");

        }
    }
}
