using System.Text.Json.Serialization;

namespace Documented.Api.V2.Models
{
    public class ListRequest
    {
        /// <summary>
        /// A página da consulta, indexadas à partir de 0.
        /// </summary>
        /// <example>0</example>
        [JsonPropertyName("_offset")]
        public int? Offset { get; set; }

        /// <summary>
        /// Quantos registros são respondidos por página
        /// </summary>
        /// <example>5</example>
        [JsonPropertyName("_limit")]
        public short? Limit { get; set; }

        /// <summary>
        /// Nome do atributo desejado para ordernação da página de consulta. Para inverter a seleção, use o sufixo **DESC**. O sufixo **ASC** é usado como padrão caso seja omitido.
        /// </summary>
        /// <example>Description ASC</example>
        [JsonPropertyName("_order")]
        public string Order { get; set; }
    }
}
