namespace GlamourLights
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("shop_db.item")]
    public partial class item
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public item()
        {
            purchase = new HashSet<purchase>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int itemId { get; set; }

        [Required]
        [StringLength(45)]
        public string name { get; set; }

        [StringLength(200)]
        public string description { get; set; }

        public int quantity { get; set; }

        [Required]
        [StringLength(45)]
        public string color { get; set; }

        [Required]
        [StringLength(45)]
        public string fabric { get; set; }

        [Required]
        [StringLength(45)]
        public string gender { get; set; }

        [Required]
        [StringLength(45)]
        public string type { get; set; }

        public int? shelf { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<purchase> purchase { get; set; }

        public virtual shelf shelf1 { get; set; }
    }
}
