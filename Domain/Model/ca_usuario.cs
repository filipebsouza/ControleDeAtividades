namespace Domain.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ca_usuario
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ca_usuario()
        {
            ca_atividade = new HashSet<ca_atividade>();
            ca_publicacao = new HashSet<ca_publicacao>();
        }

        public int id { get; set; }

        [Required]
        [StringLength(50)]
        public string nome { get; set; }

        [Required]
        [StringLength(50)]
        public string email { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ca_atividade> ca_atividade { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ca_publicacao> ca_publicacao { get; set; }
    }
}
