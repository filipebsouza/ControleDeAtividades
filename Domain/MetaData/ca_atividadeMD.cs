using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Domain.MetaData
{
    public partial class ca_atividadeMD
    {
        [Required]
        [DisplayName("ID")]
        public int id { get; set; }

        [DisplayName("Usuário")]
        public int id_usuario { get; set; }

        [Required(ErrorMessage = "A Descrição é obrigatória!")]
        [DisplayName("Descrição")]
        public string descricao { get; set; }

        [Required(ErrorMessage = "A Prioridade é obrigatória!")]
        [DisplayName("Prioridade")]
        [Range(1, 5, ErrorMessage = "A prioridade é de 1 à 5.")]
        public int prioridade { get; set; }

        [DisplayName("Data de Início")]
        public DateTime? dt_inicio { get; set; }

        [DisplayName("Data Fim")]
        public DateTime? dt_fim { get; set; }

        [DisplayName("Sistema")]
        public string sistema { get; set; }

    }
}
