namespace Domain.Model
{
    using MetaData;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [MetadataType(typeof(ca_publicacaoMD))]
    public partial class ca_publicacao
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ca_publicacao()
        {
            ca_atividadesPublicadas = new HashSet<ca_atividadesPublicadas>();
        }

        public int id { get; set; }

        public int id_usuario { get; set; }

        public DateTime? dt_publicacao { get; set; }

        [Required]
        [StringLength(10)]
        public string sistema { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ca_atividadesPublicadas> ca_atividadesPublicadas { get; set; }

        public virtual ca_usuario ca_usuario { get; set; }
    }
}
