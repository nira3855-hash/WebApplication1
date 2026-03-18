using AutoMapper;
using Repository.Entities;
using Service.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Dto
{
    public class MyMapper: Profile
    {
        string path = Directory.GetCurrentDirectory() + "\\images\\";
        public MyMapper()
        {


            CreateMap<Event, EventDto>();
                //.ForMember(dest => dest.TotalTickets,
                //           opt => opt.MapFrom(src => src.Hall != null ? src.Hall.numOfSeats : 0));
            CreateMap<EventDto, Event>();
            CreateMap<Event, ProducerEventDto>().ReverseMap();
            CreateMap<ProducerEventDto,EventDto >().ReverseMap();

            CreateMap<UserLogin, User>();
            CreateMap<User, UserLogin>();
            CreateMap<User, UserDto>();
            CreateMap<User, UserRegisterDto>();
            CreateMap<UserRegisterDto, User>();
            CreateMap<UserRegisterDto, UserDto>();
            CreateMap<UserDto, UserRegisterDto>();
            CreateMap<UserDto, User>();

            CreateMap<Producer, ProducerDto>();
            CreateMap<ProducerDto, Producer>();

            CreateMap<Hall,HallDto>();
            CreateMap<HallDto, Hall>();
          

            CreateMap<HallSeat, HallSeatDto>();
            CreateMap<HallSeatDto, HallSeat>();


            CreateMap<OrderDetail, OrderDetailDto>();
            CreateMap<OrderDetailCreateDto, OrderDetail>().ForMember(dest=>dest.PriceAtPurchase,opt=>opt.Ignore());

           
        }
        public byte[] fromStringToByte(string mypath)
        {
            return File.ReadAllBytes(path + mypath);
        }
    }
}
