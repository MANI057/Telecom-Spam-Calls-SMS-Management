using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpamComplaintManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Baseline migration.
            // The dbo.Login table already exists in the database
            // and contains existing application data.
            //
            // Therefore, we intentionally do not create or modify
            // the Login table here.
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Intentionally empty.
            //
            // The Login table existed before this EF migration
            // was created, so EF must not delete it.
        }
    }
}