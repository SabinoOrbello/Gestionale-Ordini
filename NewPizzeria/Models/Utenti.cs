namespace NewPizzeria.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Utenti")]
    public partial class Utenti
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Utenti()
        {
            Ordini = new HashSet<Ordini>();
            Roles = new HashSet<Roles>();
        }

        [Key]
        public int UserId { get; set; }

        [StringLength(30)]
        public string Username { get; set; }

        [StringLength(50)]
        public string Password { get; set; }

        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(20)]
        public string Role { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Ordini> Ordini { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Roles> Roles { get; set; }
    }
}
