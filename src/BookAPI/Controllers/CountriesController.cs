using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BookAPI.Services;
using BookAPI.Dtos;
using BookAPI.Models;

namespace BookAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController : Controller
    {
        ICountryRepository _countryRepository;
        public CountriesController(ICountryRepository countryRepository)
        {
            _countryRepository = countryRepository;
        }
        //API/countries
        [HttpGet]
        public IActionResult GetCountries()
        {
            var Countries = _countryRepository.GetCountries().ToList();
            if (!ModelState.IsValid)
                BadRequest(ModelState);
            var CountriesDtos = new List<CountryDto>();
            foreach (var country in Countries)
            {
                CountriesDtos.Add(new CountryDto
                { Id = country.Id,
                    Name = country.Name
                }
                                );
            }
            return Ok(CountriesDtos);
        }
        [HttpGet(Name = "GetCountry")]
        [Route("{CountryId}")]
        public IActionResult GetCountry(int CountryId)
        {
            if (!_countryRepository.CountryExists(CountryId))
                return NotFound();

            var Country = _countryRepository.GetCountry(CountryId);
            var CountryDto = new CountryDto() { Id = Country.Id,
                Name = Country.Name };
            if (!ModelState.IsValid)
                BadRequest(ModelState);
            return Ok(CountryDto);
        }

        [HttpGet]
        [Route("authors/{AuthorId}")]
        public IActionResult GetCountryOfAnAuthor(int AuthorId)
        {
            //TO Do validate author exists.
            if (!_countryRepository.AuthorExists(AuthorId))
                return NotFound();

            var Country = _countryRepository.GetCountryOfAuthor(AuthorId);

            if (!ModelState.IsValid)
                BadRequest(ModelState);
            var CountryDto = new CountryDto() { Id = Country.Id, Name = Country.Name };
            return Ok(CountryDto);
        }
        //api/countries/countryid/authors
        [HttpGet("{CountryId}/authors")]
        public IActionResult GetAuthorsOfACountry(int CountryId)
        {
            //TO Do validate author exists.
            if (!_countryRepository.CountryExists(CountryId))
                return NotFound();
            if (!ModelState.IsValid)
                BadRequest(ModelState);

            var authors = _countryRepository.GetAuthorsFromCountry(CountryId);
            IList<AuthorDto> authorDtos = new List<AuthorDto>();
            if (!ModelState.IsValid)
                BadRequest(ModelState);
            foreach (var author in authors)
            {
                authorDtos.Add(new AuthorDto
                {
                    FirstName = author.FirstName,
                    LastName = author.LastName
                });
            }


            return Ok(authorDtos);
        }
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(222)]
        [ProducesResponseType(500)]
        public IActionResult CreateCountry([FromBody]Country countryToCreate)
        {
            if (countryToCreate == null)
                return BadRequest(ModelState);
            var country = _countryRepository.GetCountries().Where(c => c.Name == countryToCreate.Name).FirstOrDefault();
            if (country != null)
            {
                ModelState.AddModelError("", $"Country with name {countryToCreate.Name} already exist.");
                return StatusCode(222, ModelState);
            }
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (!_countryRepository.CreateCountry(countryToCreate))
            {
                ModelState.AddModelError("", $"Error occur while saving country {countryToCreate.Name}.");
                return StatusCode(500, ModelState);
            }
            return CreatedAtRoute("GetCountry", new { CountryId = countryToCreate.Id }, countryToCreate);


        }
        [HttpPut("{countryId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(222)]
        [ProducesResponseType(500)]
        public IActionResult UpdateCountry(int countryId, [FromBody]Country countryToUpdate)
        {
            if (countryToUpdate == null)
                return BadRequest(ModelState);
            if (countryId != countryToUpdate.Id)
                return BadRequest(ModelState);
            if (!_countryRepository.CountryExists(countryId))
                return NotFound();
            if (_countryRepository.IsDuplicateCountryName(countryId, countryToUpdate.Name))
            {
                ModelState.AddModelError("", $"Country with name {countryToUpdate.Name} already exist.");
                return StatusCode(222, ModelState);
            }
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (!_countryRepository.UpdateCountry(countryToUpdate))
            {
                ModelState.AddModelError("", $"Error occur while updating country {countryToUpdate.Name}.");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }
        [HttpDelete("{countryId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        [ProducesResponseType(222)]
        [ProducesResponseType(500)]
        public IActionResult DeleteCountry(int countryId)
        {
            if (!_countryRepository.CountryExists(countryId))
                return NotFound();
            var countryToDelete = _countryRepository.GetCountry(countryId);
            if (_countryRepository.GetAuthorsFromCountry(countryId).Count() > 0)
            {
                ModelState.AddModelError("", $"Country {countryToDelete.Name} can not be deleted. It has authors associated with it");
                return StatusCode(409, ModelState);
            }
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (!_countryRepository.DeleteCountry(countryToDelete))
            {
                ModelState.AddModelError("", $"There is some error occur while deleting country {countryToDelete.Name}.");
                return StatusCode(500, ModelState);
            }
           return NoContent();
        }

        

    }
}
