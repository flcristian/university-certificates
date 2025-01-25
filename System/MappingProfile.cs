using AutoMapper;
using UniversityCertificates.Register.DTOs;
using UniversityCertificates.Register.Models;
using UniversityCertificates.Students.DTOs;
using UniversityCertificates.Students.Models;

namespace UniversityCertificates.System;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<CreateStudentRequest, Student>();
        CreateMap<UpdateStudentRequest, Student>();
        CreateMap<CreateRegisterEntryRequest, RegisterEntry>();
        CreateMap<UpdateRegisterEntryRequest, RegisterEntry>();
    }
}
