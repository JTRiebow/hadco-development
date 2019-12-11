using Hadco.Data;
using Hadco.Data.Entities;
using Hadco.Services.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Security.Authentication;
using System.Web.Http.Results;
using AutoMapper;
using Hadco.Common;
using Hadco.Common.Enums;

namespace Hadco.Services
{
    public class NoteService : GenericService<NoteDto, Note>, INoteService
    {
        INoteRepository _noteRepository;
        IPermissionsService _permissionService;
        private IDailyApprovalRepository _dailyApprovalRepository;
        public NoteService(INoteRepository noteRepository, IDailyApprovalRepository dailyApprovalRepository, IPrincipal currentUser, IPermissionsService permissionsService) : base(noteRepository, currentUser)
        {
            _noteRepository = noteRepository;
            _dailyApprovalRepository = dailyApprovalRepository;
            _permissionService = permissionsService;
        }

        public override NoteDto Insert(NoteDto dto)
        {
            dto.CreatedEmployeeID = CurrentUser.GetEmployeeID();
            dto.CreatedTime = DateTimeOffset.Now;

            var dailyApprovalExists = _dailyApprovalRepository.All
                .Any(x => x.EmployeeID == dto.EmployeeID && x.Day == dto.Day.Date && x.DepartmentID == dto.DepartmentID);

            if (!dailyApprovalExists)
            {
                _dailyApprovalRepository.Insert(new DailyApproval()
                {
                    EmployeeID = dto.EmployeeID,
                    Day = dto.Day,
                    DepartmentID = dto.DepartmentID
                });
            }
            return base.Insert(dto);
        }

        public override NoteDto Update(NoteDto dto)
        {
            var note = Find(dto.ID);
            if (note.NoteTypeID != NoteTypeName.Injury)
            {
                _permissionService.CheckPermission(AuthActivityID.EditTimerNote, note.DepartmentID);
            }
            else
            {
                _permissionService.CheckPermission(AuthActivityID.MarkTimerInjury, note.DepartmentID);
            }
            return base.Update(dto);
        }

        public IEnumerable<NoteDto> GetNotes(int employeeID, DateTime day, int departmentID)
        {
            var notes = _noteRepository.AllIncluding(x => x.NoteType).Where(x => x.EmployeeID == employeeID && x.Day == day && x.DepartmentID == departmentID);
            return Mapper.Map<IEnumerable<NoteDto>>(notes);
        }

        public NoteDto ResolveNote(int id)
        {
            var note = _noteRepository.Find(id);
            if (note == null)
            {
                return null;
            }
            var employeeID = CurrentUser.GetEmployeeID();
            if (!employeeID.HasValue)
            {
                throw new AuthenticationException();
            }
            _permissionService.CheckPermission(AuthActivityID.ResolveEmployeeTimerFlag, note.DepartmentID);

            note.Resolved = true;
            note.ResolvedEmployeeID = employeeID;
            note.ResolvedTime = DateTimeOffset.Now;
            _noteRepository.Save();
            return Mapper.Map<NoteDto>(note);
        }
    }

    public interface INoteService : IGenericService<NoteDto>
    {
        IEnumerable<NoteDto> GetNotes(int employeeID, DateTime day, int departmentID);
        NoteDto ResolveNote(int id);
    }
}
