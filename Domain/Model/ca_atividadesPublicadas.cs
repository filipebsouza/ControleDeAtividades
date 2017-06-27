namespace Domain.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ca_atividadesPublicadas
    {
        public int id { get; set; }

        public int id_atividade { get; set; }

        public int id_publicacao { get; set; }

        public virtual ca_atividade ca_atividade { get; set; }

        public virtual ca_publicacao ca_publicacao { get; set; }
    }
}
