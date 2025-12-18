using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace online_store.Models;

public class Category
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Название категории обязательно")]
    [StringLength(50, ErrorMessage = "Название не должно превышать 50 символов")]
    [Display(Name = "Название")]
    public string Name { get; set; } = string.Empty;
    
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
