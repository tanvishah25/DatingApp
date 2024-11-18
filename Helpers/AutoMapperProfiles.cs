using AutoMapper;
using DatingApp.DTOs;
using DatingApp.Entities;

namespace DatingApp.Helpers
{
    public class AutoMapperProfiles :Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AppUser, MemberDto>()
                .ForMember(d=>d.PhotoUrl,o=>o.MapFrom(s=>s.Photos.FirstOrDefault(x=>x.IsMain)!.Url));
            CreateMap<Photo, PhotoDto>();
            CreateMap<MemberUpdateDto, AppUser>();
            CreateMap<RegisterDto,AppUser>();
            CreateMap<string,DateOnly>().ConvertUsing(s=> DateOnly.Parse(s));
            CreateMap<Message, MessageDto>()
                       .ForMember(d => d.SenderPhotoUrl,
                                  o => o.MapFrom(s => s.Sender.Photos.FirstOrDefault(x => x.IsMain)!.Url))
                       .ForMember(d => d.RecipientPhotoUrl,
                                  o => o.MapFrom(s => s.Recipient.Photos.FirstOrDefault(x => x.IsMain)!.Url));
        }
    }
}
