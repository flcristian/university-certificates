using AutoMapper;
using UniversityCertificates.Students.DTOs;
using UniversityCertificates.Students.Models;

namespace UniversityCertificates.System;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<CreateStudentRequest, Student>();
        CreateMap<UpdateStudentRequest, Student>();
    }
}
