﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NextGenRobotics.Models
{
    public class OrderDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderDetailsId { get; set; }

        [Required]
        public int OrderId { get; set; }

        [Required]
        public int ProductId { get; set; }

        public int TotalPrice { get; set; }

        [Required]
        public int Quantity { get; set; }

        // Navigation properties
        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }
    }
}