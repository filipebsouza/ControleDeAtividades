using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Domain.MetaData
{
    public partial class ca_usuarioMD
    {
        [Required]
        [DisplayName("ID")]
        public int id { get; set; }

        [DisplayName("Nome")]
        [Required(ErrorMessage = "O Nome é obrigatório!")]
        [StringLength(100, ErrorMessage = "O Nome deve ter no máximo 100 caracteres.")]
        public string nome { get; set; }

        [DisplayName("Email")]
        [Required(ErrorMessage = "O Email é obrigatório!")]
        [StringLength(100, ErrorMessage="O Email deve ter no máximo 100 caracteres.")]
        public string email { get; set; }

    }
}
