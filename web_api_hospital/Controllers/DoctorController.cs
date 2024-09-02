using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;
using web_api_hospital.DTO.doctor;
using web_api_hospital.Model;

namespace web_api_hospital.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        private readonly TestTaskHospitalContext _context;

        public DoctorController(TestTaskHospitalContext context)
        {
            _context = context;
        }

        // GET: api/Doctors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DoctorListDto>>> GetDoctors(string? sortField = "Surname", string? sortDirection = "asc", int page = 1, int pageSize = 10)
        {
            // проверка страницы и и ее размера
            if (page < 1)
            {
                return BadRequest("Номер страницы должен быть больше 0");
            }
            if (pageSize < 1)
            {
                return BadRequest("Размер страницы должен быть больше 0");
            }

            // проверка поля сортировки
            sortField = sortField[0].ToString().ToUpper() + sortField.Substring(1).ToLower();
            if (!typeof(DoctorListDto).GetProperties(BindingFlags.Public | BindingFlags.Instance).Any(p => p.Name == sortField))
            {
                return BadRequest("Неверное поле для сортировки");
            }

            // проверка сортировки
            string[] sortDirectionList = new string[] { "asc", "desc" };
            if (!sortDirectionList.Contains(sortDirection.ToLower().Trim()))
            {
                return BadRequest("Неверное направление сортировки");
            }

            // получение списка врачей
            var doctors = _context.Doctors
                .Include(d => d.Plot)
                .Include(d => d.Office)
                .Include(d => d.Specialization)
                .Select(d => new DoctorListDto
                {
                    Id = d.Id,
                    Surname = d.Surname,
                    Firstname = d.Firstname,
                    Patronymic = d.Patronymic,
                    PlotName = d.Plot != null ? d.Plot.Name : null,
                    OfficeName = d.Office != null ? d.Office.Name : null,
                    SpecializationName = d.Specialization.Name
                });

            // сортировка
            if (sortDirection.ToLower().Trim() == "desc")
            {
                doctors = doctors.OrderByDescending(Helpers.SortHelper.GetSortExpression<DoctorListDto>(sortField));
            }
            else
            {
                doctors = doctors.OrderBy(Helpers.SortHelper.GetSortExpression<DoctorListDto>(sortField));
            }

            // постраничный вывод
            int skip = (page - 1) * pageSize;
            var doctors_page = await doctors.Skip(skip).Take(pageSize).ToListAsync();

            return Ok(doctors_page);
        }

        // GET: api/Doctors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DoctorDto>> GetDoctor(int id)
        {
            var doctor = await _context.Doctors
                .Select(d => new DoctorDto
                {
                    Id = d.Id,
                    Surname = d.Surname,
                    Firstname = d.Firstname,
                    Patronymic = d.Patronymic,
                    PlotId = d.PlotId,
                    OfficeId = d.OfficeId,
                    SpecializationId = d.SpecializationId
                })
                .FirstOrDefaultAsync(d => d.Id == id);

            if (doctor == null)
            {
                return NotFound();
            }

            return Ok(doctor);
        }

        // PUT: api/Doctors/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDoctor(int id, DoctorDto doctorDto)
        {
            if (id != doctorDto.Id)
            {
                return BadRequest();
            }

            if (!PlotExists(doctorDto.PlotId) || !OfficeExists(doctorDto.OfficeId) || !SpecializationExists(doctorDto.SpecializationId))
            {
                return BadRequest("Указанный участок, кабинет или специализация не существует");
            }

            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null)
            {
                return NotFound();
            }

            doctor.Surname = doctorDto.Surname;
            doctor.Firstname = doctorDto.Firstname;
            doctor.Patronymic = doctorDto.Patronymic;
            doctor.PlotId = doctorDto.PlotId;
            doctor.OfficeId = doctorDto.OfficeId;
            doctor.SpecializationId = doctorDto.SpecializationId;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DoctorExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok();
        }

        // POST: api/Doctors
        [HttpPost]
        public async Task<ActionResult<DoctorDto>> PostDoctor(DoctorDto doctorDto)
        {
            if (!PlotExists(doctorDto.PlotId) || !OfficeExists(doctorDto.OfficeId) || !SpecializationExists(doctorDto.SpecializationId))
            {
                return BadRequest("Указанный участок, кабинет или специализация не существует");
            }

            var doctor = new Doctor
            {
                Surname = doctorDto.Surname,
                Firstname = doctorDto.Firstname,
                Patronymic = doctorDto.Patronymic,
                PlotId = doctorDto.PlotId,
                OfficeId = doctorDto.OfficeId,
                SpecializationId = doctorDto.SpecializationId
            };

            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDoctor", new { id = doctor.Id }, doctorDto);
        }

        // DELETE: api/Doctors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDoctor(int id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null)
            {
                return NotFound();
            }

            _context.Doctors.Remove(doctor);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DoctorExists(int id)
        {
            return _context.Doctors.Any(e => e.Id == id);
        }

        private bool PlotExists(int? id)
        {
            return id == null || _context.Plots.Any(e => e.Id == id);
        }

        private bool OfficeExists(int? id)
        {
            return id == null || _context.Offices.Any(e => e.Id == id);
        }

        private bool SpecializationExists(int id)
        {
            return _context.Specializations.Any(e => e.Id == id);
        }
    }
}
