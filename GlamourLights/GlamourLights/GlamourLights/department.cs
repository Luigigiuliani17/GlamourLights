namespace GlamourLights
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("shop_db.department")]
    public partial class department
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public department()
        {
            shelf = new HashSet<shelf>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int departmentId { get; set; }

        [Required]
        [StringLength(45)]
        public string departmentName { get; set; }

        [Required]
        [StringLength(45)]
        public string genderType { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<shelf> shelf { get; set; }
    }
}
