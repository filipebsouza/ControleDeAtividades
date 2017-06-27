using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Domain.MetaData
{
    public partial class ca_atividadesPublicadasMD
    {
        [Required]
        [DisplayName("ID")]
        public int id { get; set; }

        [Required]
        [DisplayName("Atividade")]
        public int id_atividade { get; set; }

        [Required]
        [DisplayName("Publicação")]
        public int id_publicacao { get; set; }

    }
}
