﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelListing.API.Data.Configurations
{
    public class CountryConfiguration : IEntityTypeConfiguration<Country>
    {
        public void Configure(EntityTypeBuilder<Country> builder)
        {
            builder.HasData(
                new Country
                {

                    Id = 1,
                    Name = "Costa Rica",
                    Shortname = "CR"

                },
                new Country
                {

                    Id = 2,
                    Name = "Jamaica",
                    Shortname = "JM"

                },
                new Country
                {

                    Id = 3,
                    Name = "Japan",
                    Shortname = "JP"

                });
        }
    }
}
