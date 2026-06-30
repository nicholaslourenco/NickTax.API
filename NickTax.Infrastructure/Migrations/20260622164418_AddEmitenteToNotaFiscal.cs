using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NickTax.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEmitenteToNotaFiscal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CnpjEmitente",
                table: "notasfiscais",
                type: "character varying(14)",
                maxLength: 14,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NomeEmitente",
                table: "notasfiscais",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CnpjEmitente",
                table: "notasfiscais");

            migrationBuilder.DropColumn(
                name: "NomeEmitente",
                table: "notasfiscais");
        }
    }
}
