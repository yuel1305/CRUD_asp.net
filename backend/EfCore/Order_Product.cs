using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.EfCore
{
    [Table("order_product")]

    public class Order_Product
    {
        [Key]
        public int orderId { get; set; }
        public virtual Order Order { get; set; }

        [Key]
        public int productId { get; set; }
        public virtual Product Product { get; set; }
    }
}
