using ITI_GProject.DTOs.ChoicesDTO;
using ITI_GProject.DTOs.QuestionsDTO;

namespace ITI_GProject.Profiles
{
    public class QuestionProfile : Profile
    {
        public QuestionProfile () 
        {

            CreateMap<Question, QuesDTO>().ReverseMap()
                .ForMember(way => way.choices, option => option.MapFrom(source => source.Choices)).ReverseMap()
                .ForMember(way => way.Choices, option => option.MapFrom(source => source.choices));

            CreateMap<Choice, ChoiceDTO>().ReverseMap();


            //CreateMap<Choice, ChoiceDTO>().ReverseMap();
            CreateMap<UpdateDTO, Choice>();
            CreateMap<CreateDTO, Choice>();




        }



    }
}
