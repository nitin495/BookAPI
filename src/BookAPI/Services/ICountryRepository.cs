using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookAPI.Models;

namespace BookAPI.Services
{
    public interface ICountryRepository
    {
        //get all countries //api/countries
        ICollection<Country> GetCountries();
        //get specific country //api//countries/{countryid}
        Country GetCountry(int countryId);
        //get country of an author //api//countries/author/{authorid}
        Country GetCountryOfAuthor(int authorId);
        //get authors from a country //api//countries/{countryid}/authors
        ICollection<Author> GetAuthorsFromCountry(int countryId);
        bool CountryExists(int countryId);
        bool AuthorExists(int auhorId);
        bool IsDuplicateCountryName(int countryId, string countryName);

        bool CreateCountry(Country country);
        bool UpdateCountry(Country country);
        bool DeleteCountry(Country country);
        bool Save();


    }
}
