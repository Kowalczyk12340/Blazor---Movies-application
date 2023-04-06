using BlazorMovies.Server.Helpers;
using BlazorMovies.Shared.DTOs;
using BlazorMovies.Shared.Entities;
using BlazorMovies.Shared.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazorMovies.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public class PeopleController : ControllerBase
    {
        private readonly IPersonRepository _personRepository;

        public PeopleController(IPersonRepository personRepository)
        {
            this._personRepository = personRepository;
        }

        [HttpGet]
        public async Task<ActionResult<List<Person>>> Get([FromQuery] PaginationDTO paginationDTO)
        {
            var paginatedResponse = await _personRepository.GetPeople(paginationDTO);

            HttpContext.InsertPaginationParametersInResponse(paginatedResponse.TotalAmountPages);

            return paginatedResponse.Response;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Person>> Get(int id)
        {
            var person = await _personRepository.GetPersonById(id);
            if (person == null) { return NotFound(); }
            return person;
        }

        [HttpGet("search/{searchText}")]
        public async Task<ActionResult<List<Person>>> FilterByName(string searchText)
        {
            return await _personRepository.GetPeopleByName(searchText);
        }

        [HttpPost]
        public async Task<ActionResult<int>> Post(Person person)
        {
            await _personRepository.CreatePerson(person);
            return person.Id;
        }

        [HttpPut]
        public async Task<ActionResult> Put(Person person)
        {
            var personDB = await _personRepository.GetPersonById(person.Id);

            if (personDB == null) { return NotFound(); }

            await _personRepository.UpdatePerson(person);
            return NoContent();

        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var person = await _personRepository.GetPersonById(id);
            if (person == null)
            {
                return NotFound();
            }

            await _personRepository.DeletePerson(id);
            return NoContent();
        }
    }
}