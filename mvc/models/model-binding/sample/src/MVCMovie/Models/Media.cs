//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Threading.Tasks;

//namespace MVCMovie.Views.Movies
//{
//    public class Media
//    {
//        [Key]
//        public int Id { get; set; }

//        [Required]
//        [StringLength(100)]
//        public string Title { get; set; }

//        [Required]
//        public DateTime ReleaseDate { get; set; }

//        [Required]
//        [StringLength(1000)]
//        public string Description { get; set; }

//        [DataType(DataType.Currency)]
//        [Required(AllowEmptyStrings = true)]
//        [Range(0, 999.99)]
//        public float Price { get; set; }

//        [Required]
//        [Vintage(1970)]
//        public Genre Genre { get; set; }

//        [RateWhenPublished]
//        public string Rating { get; set; }
//        public List<Review> Reviews { get; set; }
//    }
//}
