namespace GlamourLights
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("shop_db.customer")]
    public partial class customer
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public customer()
        {
            purchase = new HashSet<purchase>();
        }

        public int customerId { get; set; }

        [Required]
        [StringLength(45)]
        public string firstName { get; set; }

        [Required]
        [StringLength(45)]
        public string lastName { get; set; }

        [Required]
        [StringLength(45)]
        public string cardNumber { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<purchase> purchase { get; set; }
    }
}
