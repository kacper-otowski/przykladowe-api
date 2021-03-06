using Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using przykladowe_api.DTOs;
using System.Linq;
using System.Threading.Tasks;

namespace przykladowe_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorsController : ControllerBase
    {
        private readonly IRepositoryWrapper _repository;
        public DoctorsController(IRepositoryWrapper repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(
                (await _repository.Doctor.GetAll().ToListAsync())
                .Select(e => new DoctorGet
                            {
                                Id = e.IdDoctor,
                                FirstName = e.FirstName,
                                LastName = e.LastName,
                                Email = e.Email
                            }
                ));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var doctor = await _repository.Doctor.GetByCondition(e => e.IdDoctor == id).FirstOrDefaultAsync();

            if (doctor is null) 
                return NotFound();

            return Ok(
                new DoctorGet { 
                    Id = doctor.IdDoctor,
                    FirstName = doctor.FirstName,
                    LastName = doctor.LastName,
                    Email = doctor.Email
                }
            );
        }

        [HttpPost]
        public async Task<IActionResult> Create(DoctorPost body)
        {

            if(!ModelState.IsValid)
                return BadRequest();

            var doctor = new Entities.Models.Doctor
            {
                FirstName = body.FirstName,
                LastName = body.LastName,
                Email = body.Email
            };

            _repository.Doctor.CreateDoctor(doctor);
            await _repository.SaveAsync();

            return Created($"/api/doctors/{doctor.IdDoctor}", doctor);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, DoctorPut body)
        {

            if (!ModelState.IsValid)
                return BadRequest();

            var doctor = await _repository.Doctor.GetByIdAsync(id);

            if (doctor is null) 
                return NotFound();

            doctor.FirstName = body.FirstName;
            doctor.LastName = body.LastName;
            doctor.Email = body.Email;

            _repository.Doctor.UpdateDoctor(doctor);
            await _repository.SaveAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var doctor = await _repository.Doctor.GetByIdAsync(id);

            if (doctor is null) 
                return NotFound();

            _repository.Doctor.DeleteDoctor(doctor);
            await _repository.SaveAsync();

            return NoContent();
        }
    }
}
