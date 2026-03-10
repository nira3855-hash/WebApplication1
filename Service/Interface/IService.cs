using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface IService<T>
    {
        Task<List<T>> GetAllAsync();       
        Task<T> GetByIdAsync(int id);    
        Task<T> AddItemAsync(T item);
        Task UpdateItemAsync(int id, T item);
        Task DeleteItemAsync(int id);
    }
}