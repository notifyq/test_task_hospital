using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using web_api_hospital.DTO.doctor;
using web_api_hospital.DTO.patient;
using web_api_hospital.Model;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace web_api_hospital.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly TestTaskHospitalContext _context;

        public PatientController(TestTaskHospitalContext context)
        {
            _context = context;
        }

        // GET: api/Patients
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PatientListDto>>> GetPatients(string? sortField = "Surname",string? sortDirection = "asc", int page = 1,int pageSize = 10)
        {
            // кол-во страниц
            if (page < 1)
            {
                return BadRequest("Номер страницы должен быть больше 0");
            }
            if (pageSize < 1)
            {
                return BadRequest("Размер страницы должен быть больше 0");
            }
            // получаем список публичных свойств класса и проверяем совпадение на поле sortField

            sortField = sortField[0].ToString().ToUpper() + sortField.Substring(1).ToLower();
            if (!typeof(PatientListDto).GetProperties(BindingFlags.Public | BindingFlags.Instance).Any(p => p.Name == sortField))
            {
                return BadRequest("Неверное поле для сортировки");
            }
            // корректность сортировки
            string[] sortDirectionList = new string[]{ "asc", "desc" };
            if (!sortDirectionList.Contains(sortDirection.ToLower().Trim()))
            {
                return BadRequest("Неверное направление сортировки");
            }
              var patients = _context.Patients
                        .Include(p => p.Plot)
                        .Select(p => new PatientListDto
                        {
                            Id = p.Id,
                            Surname = p.Surname,
                            Firstname = p.Firstname,
                            Patronymic = p.Patronymic,
                            Address = p.Address,
                            Birthday = p.Birthday,
                            Gender = p.Gender,
                            PlotName = p.Plot.Name
                        });

            // cортировка
            if (sortDirection.ToLower().Trim() == "desc")
            {
                patients = patients.OrderByDescending(Helpers.SortHelper.GetSortExpression<PatientListDto>(sortField));
            }
            else
            {
                patients = patients.OrderBy(Helpers.SortHelper.GetSortExpression<PatientListDto>(sortField));
            }

            // постраничный вывод
            int skip = (page - 1) * pageSize;
            var patients_page = await patients.Skip(skip).Take(pageSize).ToListAsync();

            return Ok(patients_page);
        }

        // GET: api/Patients/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PatientDto>> GetPatient(int id)
        {
            var patient = await _context.Patients
                .Select(p => new PatientDto
                {
                    Id = p.Id,
                    Surname = p.Surname,
                    Firstname = p.Firstname,
                    Patronymic = p.Patronymic,
                    Address = p.Address,
                    Birthday = p.Birthday,
                    Gender = p.Gender,
                    PlotId = p.PlotId
                })
                .FirstOrDefaultAsync(p => p.Id == id);

            if (patient == null)
            {
                return NotFound();
            }

            return Ok(patient);
        }

        // PUT: api/Patients/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPatient(int id, PatientDto patientDto)
        {
            if (id != patientDto.Id)
            {
                return BadRequest();
            }

            if (!PlotExists(patientDto.PlotId))
            {
                return BadRequest("Указанный участок не существует.");
            }

            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
            {
                return NotFound();
            }

            patient.Surname = patientDto.Surname;
            patient.Firstname = patientDto.Firstname;
            patient.Patronymic = patientDto.Patronymic;
            patient.Address = patientDto.Address;
            patient.Birthday = patientDto.Birthday;
            patient.Gender = patientDto.Gender;
            patient.PlotId = patientDto.PlotId;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PatientExists(id))
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

        // POST: api/Patients
        [HttpPost]
        public async Task<ActionResult<PatientDto>> PostPatient(PatientDto patientDto)
        {
            if (!PlotExists(patientDto.PlotId))
            {
                return BadRequest("Указанный участок не существует.");
            }

            var patient = new Patient
            {
                Surname = patientDto.Surname,
                Firstname = patientDto.Firstname,
                Patronymic = patientDto.Patronymic,
                Address = patientDto.Address,
                Birthday = patientDto.Birthday,
                Gender = patientDto.Gender,
                PlotId = patientDto.PlotId
            };

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPatient", new { id = patient.Id }, patientDto);
        }

        // DELETE: api/Patients/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePatient(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
            {
                return NotFound();
            }

            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PatientExists(int id)
        {
            return _context.Patients.Any(e => e.Id == id);
        }
        private bool PlotExists(int id)
        {
            return _context.Plots.Any(e => e.Id == id);
        }
    }
}
