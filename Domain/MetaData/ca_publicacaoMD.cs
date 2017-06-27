using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Domain.MetaData
{
    public partial class ca_publicacaoMD
    {
        [Required]
        [DisplayName("ID")]
        public int id { get; set; }

        [Required(ErrorMessage = "O Usuário é obrigatório!")]
        [DisplayName("Usuário")]
        public int id_usuario { get; set; }

        [DisplayName("Data da Publicação")]
        public DateTime? dt_publicacao { get; set; }

        [DisplayName("Sistema")]
        public string sistema { get; set; }

    }
}
