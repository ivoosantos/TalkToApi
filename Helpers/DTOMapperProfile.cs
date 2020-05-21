using AutoMapper;
using TalkToApi.V1.Models;
//using TalkToApi.V1.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TalkToApi.V1.Models.DTO;

namespace MimicAPI.Helpers
{
    public class DTOMapperProfile : Profile
    {
        public DTOMapperProfile()
        {
            CreateMap<ApplicationUser, UsuarioDTO>().ForMember(dest => dest.Nome, orig => orig.MapFrom(src => src.FullName));

            CreateMap<Mensagem, MensagemDTO>();

            //CreateMap<List<ApplicationUser>, List<UsuarioDTO>>();
            //CreateMap<PaginationList<Palavra>, PaginationList<PalavraDTO>>();
        }
    }
}
