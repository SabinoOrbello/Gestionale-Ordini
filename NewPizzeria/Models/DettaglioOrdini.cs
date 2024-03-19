namespace NewPizzeria.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DettaglioOrdini")]
    public partial class DettaglioOrdini
    {
        [Key]
        public int OrderDetailId { get; set; }

        public int OrderId { get; set; }

        public int ProductId { get; set; }

        public int? Quantity { get; set; }

        public virtual Ordini Ordini { get; set; }

        public virtual Prodotti Prodotti { get; set; }
    }
}
