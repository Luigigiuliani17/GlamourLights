namespace GlamourLights
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("shop_db.purchase")]
    public partial class purchase
    {
        public int purchaseId { get; set; }

        public int customer { get; set; }

        public int item { get; set; }

        public virtual customer customer1 { get; set; }

        public virtual item item1 { get; set; }
    }
}
