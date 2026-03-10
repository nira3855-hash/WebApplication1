using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Service.Dto;

namespace Service.Interface
{
    public interface OrderDetailIService
    {
        // מסיים הזמנה – מחזיר DTO
        Task<OrderDetailDto> CompleteOrderItemAsync(OrderDetailCreateDto item);

        // מוסיף פריט לסל קניות – מחזיר DTO
        Task<OrderDetailDto> AddToCartItemAsync(OrderDetailCreateDto item);

        // מעדכן פריט קיים לפי ID
        Task UpdateItemAsync(int id, OrderDetailDto item);

        // מחזיר פריט לפי ID
        Task<OrderDetailDto> GetByIdAsync(int id);

        // מחזיר את כל הפריטים
        Task<List<OrderDetailDto>> GetAllAsync();

        // מוחק פריט לפי ID
        Task DeleteItemAsync(int id);
    }
}