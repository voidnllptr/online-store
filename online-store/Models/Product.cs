using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace online_store.Models;

public class Product
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Название товара обязательно")]
    [StringLength(100, ErrorMessage = "Название не должно превышать 100 символов")]
    [Display(Name = "Название")]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(500, ErrorMessage = "Описание не должно превышать 500 символов")]
    [Display(Name = "Описание")]
    public string? Description { get; set; }
    
    [Required(ErrorMessage = "Цена обязательна")]
    [Range(0.01, 1000000, ErrorMessage = "Цена должна быть больше 0")]
    [DataType(DataType.Currency)]
    [Column(TypeName = "decimal(18, 2)")]
    [Display(Name = "Цена")]
    public decimal Price { get; set; }
    
    [Display(Name = "Изображение")]
    public string? ImageUrl { get; set; }
    
    [Required(ErrorMessage = "Количество на складе обязательно")]
    [Range(0, 10000, ErrorMessage = "Количество должно быть неотрицательным")]
    [Display(Name = "На складе")]
    public int Stock { get; set; }
    
    [Display(Name = "Категория")]
    public int CategoryId { get; set; }
    
    public Category? Category { get; set; }
}
