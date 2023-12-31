﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelListing.API.Data;
using HotelListing.API.Models.Country;
using AutoMapper;
using HotelListing.API.Contracts;
using Microsoft.AspNetCore.Authorization;
using HotelListing.API.Exceptions;

namespace HotelListing.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class CountriesController : ControllerBase
    {
        
        private readonly IMapper _mapper;
        private readonly ICountriesRepository _countriesRepository;
        private readonly ILogger<CountriesController> _logger;

        public CountriesController( IMapper mapper, ICountriesRepository countriesRepository, ILogger<CountriesController> logger)
        {
           
            this._mapper = mapper;
            this._countriesRepository = countriesRepository;
            this._logger = logger;
        }

        // GET: api/Countries
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetCountryModel>>> GetCountries()
        {
            var countries = await _countriesRepository.GetAllAsync();
            var records = _mapper.Map<List<GetCountryModel>>(countries);
            return Ok(records);
        }

        // GET: api/Countries/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CountryModel>> GetCountry(int id)
        {
            var country = await _countriesRepository.GetDetails(id);
           


            if (country == null)
            {
                throw new NotFoundException(nameof(GetCountry), id);  
                
            }

            var countryModel = _mapper.Map<CountryModel>(country);

            return countryModel;
        }

        // PUT: api/Countries/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutCountry(int id, UpdateCountryModel updateCountryModel)
        {
            if (id != updateCountryModel.Id)
            {
                return BadRequest("Invalid Record Id");
            }

            //_context.Entry(country).State = EntityState.Modified;

           

            var country = await _countriesRepository.GetAsync(id);

            if (country == null)
            {
                throw new NotFoundException(nameof(GetCountry), id);

            }

            _mapper.Map(updateCountryModel, country);

            try
            {
                await _countriesRepository.UpdateAsync(country);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await CountryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Countries
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Country>> PostCountry(CreateCountryModel CreateCountry)
        {

            //BEFORE REFACTORING, making code more efficient

            //var country = new Country
            //{

            //    Name = CreateCountry.Name,
            //    Shortname = CreateCountry.Shortname,

            //};

            //AFTER REFACTORING
            var country = _mapper.Map<Country>(CreateCountry);

            
           await _countriesRepository.AddAsync(country);
            

            return CreatedAtAction("GetCountry", new { id = country.Id }, country);
        }

        // DELETE: api/Countries/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteCountry(int id)
        {
            
            var country = await _countriesRepository.GetAsync(id);
            if (country == null)
            {
                throw new NotFoundException(nameof(GetCountry), id);

            }

            await _countriesRepository.DeleteAsync(id);
             
            return NoContent();
        }

        private async Task <bool> CountryExists(int id)
        {
            return  await _countriesRepository.Exists(id); 
        }
    }
}
