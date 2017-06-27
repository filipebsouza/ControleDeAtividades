namespace Domain.Model
{
    using MetaData;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [MetadataType(typeof(ca_atividadeMD))]
    public partial class ca_atividade
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ca_atividade()
        {
            ca_atividadesPublicadas = new HashSet<ca_atividadesPublicadas>();
        }

        public int id { get; set; }

        public int? id_usuario { get; set; }

        [Column(TypeName = "ntext")]
        [Required]
        public string descricao { get; set; }

        public int prioridade { get; set; }

        public DateTime? dt_inicio { get; set; }

        public DateTime? dt_fim { get; set; }

        [Required]
        [StringLength(10)]
        public string sistema { get; set; }

        public virtual ca_usuario ca_usuario { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ca_atividadesPublicadas> ca_atividadesPublicadas { get; set; }
    }
}
