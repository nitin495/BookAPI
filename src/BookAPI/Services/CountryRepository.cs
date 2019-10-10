using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookAPI.Models;

namespace BookAPI.Services
{
    public class CountryRepository : ICountryRepository
    {
        private BookDbContext _countryContext;
        public CountryRepository(BookDbContext bookDbContext)
        {
            _countryContext = bookDbContext;
        }

        public bool AuthorExists(int auhorId)
        {
            return _countryContext.Authors.Any(a => a.Id == auhorId);


        }

        public bool CountryExists(int countryId)
        {
            return _countryContext.Countries.Any(c => c.Id == countryId);
        }

        public bool CreateCountry(Country country)
        {
            _countryContext.Add(country);
            return Save();
        }

        public bool DeleteCountry(Country country)
        {
            _countryContext.Remove(country);
            return Save();
        }

        public ICollection<Author> GetAuthorsFromCountry(int countryId)
        {
            return _countryContext.Authors.Where(c => c.Country.Id == countryId).ToList();
        }

        public ICollection<Country> GetCountries()
        {
            return _countryContext.Countries.OrderBy(c => c.Name).ToList();
        }

        public Country GetCountry(int countryId)
        {
            //return _countryContext.Countries.FirstOrDefault(c => c.Id == countryId);
            return _countryContext.Countries.Where(c=>c.Id==countryId).FirstOrDefault();
        }

        public Country GetCountryOfAuthor(int authorId)
        {
            return _countryContext.Authors.Where(a => a.Id == authorId).Select(c => c.Country).FirstOrDefault();
            //return _countryContext.Countries.Where(c=>c.Authors.Where(a=>a.Id==authorId))


        }

        public bool IsDuplicateCountryName(int countryId, string countryName)
        {
            var country=_countryContext.Countries.Where(c => c.Name.Trim().ToUpper() == countryName.Trim().ToUpper() && c.Id!=countryId).FirstOrDefault();
            return country == null ? false : true;
        }

        public bool Save()
        {
            var savedChanges = _countryContext.SaveChanges();
            return savedChanges >= 0 ? true : false;
        }

        public bool UpdateCountry(Country country)
        {
            _countryContext.Update(country);
            return Save();
        }
    }
}
