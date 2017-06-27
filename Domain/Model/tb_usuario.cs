namespace Domain.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tb_usuario
    {
        [Key]
        public int cod_usuario { get; set; }

        [Required]
        [StringLength(30)]
        public string nome { get; set; }

        [Required]
        [StringLength(50)]
        public string email { get; set; }

        [Required]
        [StringLength(50)]
        public string senha { get; set; }

        public int situacao { get; set; }
    }
}
