namespace NewPizzeria.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Ordini")]
    public partial class Ordini
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Ordini()
        {
            DettaglioOrdini = new HashSet<DettaglioOrdini>();
        }

        [Key]
        public int OrderId { get; set; }

        public int UserId { get; set; }

        public DateTime? OrderDate { get; set; }

        [StringLength(200)]
        public string ShippingAddress { get; set; }

        public string Notes { get; set; }

        [StringLength(50)]
        public string Status { get; set; }

        public int? ProductId { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DettaglioOrdini> DettaglioOrdini { get; set; }

        public virtual Utenti Utenti { get; set; }

        public virtual Prodotti Prodotti { get; set; }
    }
}
