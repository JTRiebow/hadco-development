using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using Hadco.Common;
using Hadco.Data;
using Hadco.Data.Entities;
using Hadco.Services;
using Hadco.Services.DataTransferObjects;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;

namespace Hadco.Tests.Hadco.Services.Unit
{
    [TestFixture]
    public class NoteServiceTests
    {
        private Fixture fixture = new Fixture();
        private int currentUserID;
        private Mock<ClaimsPrincipal> currentUser;
        private Mock<INoteRepository> noteRepository;
        private Mock<IDailyApprovalRepository> dailyApprovalRepository;

        [SetUp]
        public void Init()
        {
            MappingProfile.Configure();
            fixture.Customize(new AutoMoqCustomization());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            SetupAuthorizedUser();

            noteRepository = fixture.Freeze<Mock<INoteRepository>>();
            noteRepository.Reset();
            fixture.Inject(noteRepository.Object);

            dailyApprovalRepository = fixture.Freeze<Mock<IDailyApprovalRepository>>();
            dailyApprovalRepository.Reset();
            fixture.Inject(dailyApprovalRepository.Object);
        }

        [Test]
        public void InsertCreatesDailyApprovalIfDoesntExists()
        {
            var dailyApprovals = new List<DailyApproval>().AsQueryable();
            dailyApprovalRepository.Setup(x => x.All).Returns(dailyApprovals);

            var note = fixture.Build<NoteDto>()
                .Without(x => x.CreatedEmployee)
                .Without(x => x.NoteType)
                .Without(x => x.ResolvedEmployee).Create();

            var sut = fixture.Create<NoteService>();
            sut.Insert(note);

            dailyApprovalRepository.Verify(x =>
                x.Insert(It.Is<DailyApproval>(y => 
                    y.DepartmentID == note.DepartmentID && y.EmployeeID == note.EmployeeID && y.Day == note.Day)), Times.Once);
        }

        [Test]
        public void DoesNotInsertDailyApprovalIfAlreadyExists()
        {

            var note = fixture.Build<NoteDto>()
                              .Without(x => x.CreatedEmployee)
                              .Without(x => x.NoteType)
                              .Without(x => x.ResolvedEmployee).Create();

            var dailyApprovals = new List<DailyApproval>()
                                 {
                                     new DailyApproval()
                                     {
                                         EmployeeID = note.EmployeeID,
                                         Day = note.Day,
                                         DepartmentID = note.DepartmentID
                                     }
                                 }.AsQueryable();
            dailyApprovalRepository.Setup(x => x.All).Returns(dailyApprovals);


            var sut = fixture.Create<NoteService>();
            sut.Insert(note);

            dailyApprovalRepository.Verify(x =>
                                               x.Insert(It.Is<DailyApproval>(y =>
                                                                                 y.DepartmentID == note.DepartmentID && y.EmployeeID == note.EmployeeID && y.Day == note.Day)), Times.Never);
        }


        private void SetupAuthorizedUser()
        {
            currentUser = fixture.Create<Mock<ClaimsPrincipal>>();
            fixture.Inject<IPrincipal>(currentUser.Object);
            var claims = new List<Claim>()
                         {
                             new Claim(ClaimsExtensions.EMPLOYEEID_KEY, currentUserID.ToString())
                         };
            currentUser.Setup(x => x.Claims)
                       .Returns(claims);
        }

    }
}
