using System;
using TLIS_Service.ServiceBase;
using Xunit;

namespace TLIS_UnitTest.StudentModule.Services
{
    public class StudentsUnit
    {
 
        private UnitOfWorkService _unitOfWorkService;

        public StudentsUnit()
        {
            UnitOfWorkService unitOfWorkService = new UnitOfWorkService(); 
            _unitOfWorkService = unitOfWorkService; 
        } 

        [Fact]
        public void sum()
        { 
            int expectedValue = _unitOfWorkService.StudentService.sum(3, 4);
            Assert.Equal(7, expectedValue);

        }
    }
}
