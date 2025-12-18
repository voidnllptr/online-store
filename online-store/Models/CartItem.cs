using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace online_store.Models;

public class CartItem
{
    public int Id { get; set; }
    
    [Display(Name = "Товар")]
    public int ProductId { get; set; }
    
    public Product? Product { get; set; }
    
    [Required(ErrorMessage = "Укажите количество")]
    [Range(1, 100, ErrorMessage = "Количество должно быть от 1 до 100")]
    [Display(Name = "Количество")]
    public int Quantity { get; set; } = 1;
    
    [Display(Name = "Идентификатор сессии")]
    public string? SessionId { get; set; }
    
    [Display(Name = "Пользователь")]
    public string? UserId { get; set; }
    
    [Display(Name = "Дата создания")]
    public DateTime DateCreated { get; set; }
}
