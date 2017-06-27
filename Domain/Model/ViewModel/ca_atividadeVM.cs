using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Domain.MetaData;

namespace Domain.Model.ViewModel
{
    public partial class ca_atividadeVM
    {
        [DisplayName("Usuário")]
        public string usuario { get; set; }

        [DisplayName("Descrição")]
        public string descricao { get; set; }

        [DisplayName("Prioridade")]
        public int prioridade { get; set; }

        [DisplayName("Início")]
        public string dt_inicio { get; set; }

        [DisplayName("Fim")]
        public string dt_fim { get; set; }

        [DisplayName("Sistema")]
        public string sistema { get; set; }

        [DisplayName("Operações")]
        public string operacoes { get; set; }
    }
}
