using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Web.Http.OData;
using Dapper;
using Hadco.Common;
using Hadco.Data;
using Hadco.Data.Entities;
using Hadco.Services;
using Hadco.Services.DataTransferObjects;
using Hadco.Web;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;

namespace Hadco.Tests.Hadco.Services.Unit
{
    /// <summary>
    /// Summary description for DailyApprovalService
    /// </summary>
    [TestFixture]
    public class DailyApprovalServiceTests
    {
        private Fixture fixture = new Fixture();
        private int currentUserID;
        private Mock<ClaimsPrincipal> currentUser;
        private Mock<IDailyApprovalRepository> dailyApprovalRepository;
        private Mock<IEmployeeService> employeeService;

        [SetUp]
        public void Init()
        {
            MappingProfile.Configure();
            fixture.Customize(new AutoMoqCustomization());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            currentUserID = fixture.Create<int>();
            SetupAuthorizedUser();

            dailyApprovalRepository = fixture.Freeze<Mock<IDailyApprovalRepository>>();
            dailyApprovalRepository.Reset();
            fixture.Inject(dailyApprovalRepository.Object);

            employeeService = fixture.Freeze<Mock<IEmployeeService>>();
            employeeService.Reset();
            fixture.Inject(employeeService.Object);
        }

        [Test]
        public void CanApproveIfDirectSupervisor()
        {          
            // Arrange
            var employeeID = fixture.Create<int>();
            var day = fixture.Create<DateTime>();
            var departmentID = fixture.Create<int>();
            var currentApprovals = fixture.CreateMany<DailyApproval>().ToList();
            var currentApproval = fixture.Build<DailyApproval>()
                                         .With(x => x.ApprovedBySupervisor, false)
                                         .With(x => x.EmployeeID, employeeID)
                                         .With(x => x.Day, day)
                                         .With(x => x.DepartmentID, departmentID).Create();
            currentApprovals.Add(currentApproval);

            dailyApprovalRepository.Setup(x => x.All).Returns(currentApprovals.AsQueryable());
            employeeService.Setup(x => x.EmployeeSupervisorExists(employeeID, currentUserID)).Returns(true);

            var changes = new Delta<DailyApprovalPatchDto>();
            changes.TrySetPropertyValue(nameof(DailyApprovalPatchDto.ApprovedBySupervisor), true);

            var sut = fixture.Create<DailyApprovalService>();

            // Act
            sut.Update(employeeID, day, departmentID, changes);

            // Assert
            dailyApprovalRepository.Verify(x => x.Update(It.Is<DailyApproval>(y => y.ApprovedBySupervisor)), Times.Once);
            dailyApprovalRepository.Verify(x => x.Update(It.Is<DailyApproval>(y => !y.ApprovedBySupervisor)), Times.Never);
            dailyApprovalRepository.Verify(x => x.Save(), Times.Never);
        }

        [Test]
        [TestCase(ProjectConstants.SupervisorRole)]
        [TestCase(ProjectConstants.AdminRole)]
        [TestCase(ProjectConstants.AdminRole, ProjectConstants.SupervisorRole)]
        [TestCase(ProjectConstants.SupervisorRole, ProjectConstants.AccountingRole)]
        [TestCase(ProjectConstants.AdminRole, ProjectConstants.BillingRole)]
        public void CanApproveSupervisorWithRole(params string[] roles)
        {
            // Arrange

            AddRolesToCurrentUser(roles);

            var changes = new Delta<DailyApprovalPatchDto>();
            changes.TrySetPropertyValue(nameof(DailyApprovalPatchDto.ApprovedBySupervisor), true);

            var employeeID = fixture.Create<int>();
            var day = fixture.Create<DateTime>();
            var departmentID = fixture.Create<int>();

            var currentApprovals = fixture.CreateMany<DailyApproval>().ToList();
            var currentApproval = fixture.Build<DailyApproval>()
                                         .With(x => x.ApprovedBySupervisor, false)
                                         .With(x => x.EmployeeID, employeeID)
                                         .With(x => x.Day, day)
                                         .With(x => x.DepartmentID, departmentID).Create();
            currentApprovals.Add(currentApproval);

            dailyApprovalRepository.Setup(x => x.All).Returns(currentApprovals.AsQueryable());
            employeeService.Setup(x => x.EmployeeSupervisorExists(employeeID, currentUserID)).Returns(false);

            var sut = fixture.Create<DailyApprovalService>();

            // Act
            sut.Update(employeeID, day, departmentID, changes);

            // Assert
            dailyApprovalRepository.Verify(x => x.Update(It.Is<DailyApproval>(y => y.ApprovedBySupervisor 
                                                                                    && y.ApprovedBySupervisorEmployeeID == currentUserID)), Times.Once());
            dailyApprovalRepository.Verify(x => x.Update(It.Is<DailyApproval>(y => !y.ApprovedBySupervisor)), Times.Never());
            dailyApprovalRepository.Verify(x => x.Save(), Times.Never);
        }

        [Test]
        [TestCase(ProjectConstants.BillingRole)]
        [TestCase(ProjectConstants.AccountingRole)]
        [TestCase(ProjectConstants.ForemanRole)]
        public void CanNOTApproveSupervisorWithOUTRole(params string[] roles)
        {
            // Arrange

            AddRolesToCurrentUser(roles);

            var changes = new Delta<DailyApprovalPatchDto>();
            changes.TrySetPropertyValue(nameof(DailyApprovalPatchDto.ApprovedBySupervisor), true);

            var employeeID = fixture.Create<int>();
            var day = fixture.Create<DateTime>();
            var departmentID = fixture.Create<int>();

            var currentApprovals = fixture.CreateMany<DailyApproval>().ToList();
            var currentApproval = fixture.Build<DailyApproval>()
                                         .With(x => x.ApprovedBySupervisor, false)
                                         .With(x => x.EmployeeID, employeeID)
                                         .With(x => x.Day, day)
                                         .With(x => x.DepartmentID, departmentID).Create();
            currentApprovals.Add(currentApproval);

            //dailyApprovalRepository = fixture.Freeze<Mock<IDailyApprovalRepository>>();
            //fixture.Inject(dailyApprovalRepository.Object);
            dailyApprovalRepository.Setup(x => x.All).Returns(currentApprovals.AsQueryable());

            employeeService = fixture.Freeze<Mock<IEmployeeService>>();
            fixture.Inject(employeeService.Object);
            employeeService.Setup(x => x.EmployeeSupervisorExists(employeeID, currentUserID)).Returns(false);

            var sut = fixture.Create<DailyApprovalService>();

            // Act
            Assert.Throws<UnauthorizedDataAccessException>(() => sut.Update(employeeID, day, departmentID, changes));

            // Assert
            dailyApprovalRepository.Verify(x => x.Update(It.Is<DailyApproval>(y => y.ApprovedBySupervisor)), Times.Never());
            dailyApprovalRepository.Verify(x => x.Update(It.Is<DailyApproval>(y => !y.ApprovedBySupervisor)), Times.Never());
            dailyApprovalRepository.Verify(x => x.Save(), Times.Never);
        }

        [Test]
        [TestCase(ProjectConstants.SupervisorRole)]
        [TestCase(ProjectConstants.AdminRole)]
        [TestCase(ProjectConstants.AdminRole, ProjectConstants.SupervisorRole)]
        [TestCase(ProjectConstants.SupervisorRole, ProjectConstants.AccountingRole)]
        [TestCase(ProjectConstants.AdminRole, ProjectConstants.BillingRole)]
        [TestCase(ProjectConstants.BillingRole)]
        [TestCase(ProjectConstants.AccountingRole)]
        [TestCase(ProjectConstants.ForemanRole)]
        public void WillBeIgnoredIfSupervisorAlreadyApproved(params string[] roles)
        {
            // Arrange
            AddRolesToCurrentUser(roles);

            var changes = new Delta<DailyApprovalPatchDto>();
            changes.TrySetPropertyValue(nameof(DailyApprovalPatchDto.ApprovedBySupervisor), true);

            var employeeID = fixture.Create<int>();
            var day = fixture.Create<DateTime>();
            var departmentID = fixture.Create<int>();

            var currentApprovals = fixture.CreateMany<DailyApproval>().ToList();
            var currentApproval = fixture.Build<DailyApproval>()
                                         .With(x => x.ApprovedBySupervisor, true)
                                         .With(x => x.EmployeeID, employeeID)
                                         .With(x => x.Day, day)
                                         .With(x => x.DepartmentID, departmentID).Create();
            currentApprovals.Add(currentApproval);

            dailyApprovalRepository.Setup(x => x.All).Returns(currentApprovals.AsQueryable());
            employeeService.Setup(x => x.EmployeeSupervisorExists(employeeID, currentUserID)).Returns(false);

            var sut = fixture.Create<DailyApprovalService>();

            // Act
            Assert.DoesNotThrow(() => sut.Update(employeeID, day, departmentID, changes));

            // Assert
            dailyApprovalRepository.Verify(x => x.Update(It.Is<DailyApproval>(y => y.ApprovedBySupervisor)), Times.Once());
            dailyApprovalRepository.Verify(x => x.Update(It.Is<DailyApproval>(y => !y.ApprovedBySupervisor)), Times.Never());
            dailyApprovalRepository.Verify(x => x.Save(), Times.Never);
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

        private void AddRolesToCurrentUser(params string[] roles)
        {

            var claims = currentUser.Object.Claims.ToList();
            var roleClaims = roles.Select(role => new Claim(ClaimsExtensions.ROLE_KEY, role));
            claims.AddRange(roleClaims);
            currentUser.Setup(x => x.Claims)
                       .Returns(claims);
        }
    }
}
