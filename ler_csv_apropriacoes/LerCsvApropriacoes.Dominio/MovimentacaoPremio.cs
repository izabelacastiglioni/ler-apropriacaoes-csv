using System;

namespace LerCsvApropriacoes.Dominio
{
    public class MovimentacaoPremio
    {
        public string ItemCertificado { get; set; }
        public string Parcela { get; set; }
        public Guid MovimentoId { get; set; }
        public string TipoMovimento { get; set; }
        public Guid EventoId { get; set; }
        public Guid Identificador { get; set; }

    }
}
