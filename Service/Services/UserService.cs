using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Repository.Entities;
using Repository.Interfaces;
using Service.Dto;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class UserService: IService<UserDto>
    {
        private readonly IRepository<User> repository;
        private readonly IMapper mapper;
        public UserService(IRepository<User> repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }
        public UserDto AddItem(UserDto item)
        {
            item.password= BCrypt.Net.BCrypt.HashPassword(item.password);

            return mapper.Map<User, UserDto>(repository.AddItem(mapper.Map<UserDto, User>(item)));
        }

        public void DeleteItem(int id)
        {
            var user = repository.GetById(id);

            if (user == null)
                throw new NotImplementedException();
            repository.DeleteItem(id);
        }

        public List<UserDto> GetAll()
        {

            return mapper.Map<List<User>, List<UserDto>>(repository.GetAll());
        }

        public UserDto GetById(int id)
        {
            var user = repository.GetById(id);
            if(user==null)
                throw new NotImplementedException();
            return mapper.Map<User, UserDto>(user);
        }

        public void UpdateItem(int id, UserDto dto)
        {
            var user = repository.GetById(id);
            if (user == null)
                throw new NotImplementedException();
       
            user.Name = dto.Name;
            user.email = dto.email;
            user.password = dto.password;
            repository.UpdateItem(id, user);
            
        }
    }
}
