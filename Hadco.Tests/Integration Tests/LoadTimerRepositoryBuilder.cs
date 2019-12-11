using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Hadco.Data;
using Hadco.Data.Entities;
using Hadco.Services;
using Hadco.Services.DataTransferObjects;
using Moq;

namespace Hadco.Tests.Integration_Tests
{
    public class LoadTimerRepositoryBuilder
    {
        public LoadTimerRepository Build()
        {
            Mapper.CreateMap<LoadTimer, LoadTimerDto>().ReverseMap();

            return new LoadTimerRepository();
        }
    }
}
