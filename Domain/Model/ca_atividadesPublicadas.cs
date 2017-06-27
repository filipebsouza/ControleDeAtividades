namespace Domain.Model
{
    using MetaData;
    using System.ComponentModel.DataAnnotations;

    [MetadataType(typeof(ca_atividadesPublicadasMD))]
    public partial class ca_atividadesPublicadas
    {
        public int id { get; set; }

        public int id_atividade { get; set; }

        public int id_publicacao { get; set; }

        public virtual ca_atividade ca_atividade { get; set; }

        public virtual ca_publicacao ca_publicacao { get; set; }
    }
}
