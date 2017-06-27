namespace Domain.Model
{
    using System.ComponentModel.DataAnnotations;

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
