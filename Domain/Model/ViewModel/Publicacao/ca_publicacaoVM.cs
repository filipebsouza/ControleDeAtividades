using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Domain.MetaData;

namespace Domain.Model.ViewModel
{
    public partial class ca_publicacaoVM
    {

        [DisplayName("Usuário")]
        public string usuario { get; set; }

        [DisplayName("Publicação")]
        public string dt_publicacao { get; set; }

        [DisplayName("Sistema")]
        public string sistema { get; set; }

        [DisplayName("Atividades")]
        public string atividades { get; set; }

        [DisplayName("Operações")]
        public string operacoes { get; set; }
    }
}
