using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace proyectogranja1.Migrations
{
    /// <inheritdoc />
    public partial class n1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Empleado",
                columns: table => new
                {
                    EmpleadoID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CI = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Cargo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    EsVeterinario = table.Column<bool>(type: "boolean", nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empleado", x => x.EmpleadoID);
                });

            migrationBuilder.CreateTable(
                name: "Vaca",
                columns: table => new
                {
                    VacaID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CodigoVaca = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Raza = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Peso = table.Column<decimal>(type: "numeric(6,2)", nullable: false),
                    EstadoSalud = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vaca", x => x.VacaID);
                });

            migrationBuilder.CreateTable(
                name: "Distribucion",
                columns: table => new
                {
                    DistribucionID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmpleadoID = table.Column<int>(type: "integer", nullable: false),
                    Fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LitrosEntregados = table.Column<decimal>(type: "numeric(8,2)", nullable: false),
                    Destino = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Estado = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Distribucion", x => x.DistribucionID);
                    table.ForeignKey(
                        name: "FK_Distribucion_Empleado_EmpleadoID",
                        column: x => x.EmpleadoID,
                        principalTable: "Empleado",
                        principalColumn: "EmpleadoID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Produccion",
                columns: table => new
                {
                    ProduccionID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VacaID = table.Column<int>(type: "integer", nullable: false),
                    EmpleadoID = table.Column<int>(type: "integer", nullable: false),
                    Fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TotalLitros = table.Column<decimal>(type: "numeric(6,2)", nullable: false),
                    Calidad = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    FechaRegistro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produccion", x => x.ProduccionID);
                    table.ForeignKey(
                        name: "FK_Produccion_Empleado_EmpleadoID",
                        column: x => x.EmpleadoID,
                        principalTable: "Empleado",
                        principalColumn: "EmpleadoID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Produccion_Vaca_VacaID",
                        column: x => x.VacaID,
                        principalTable: "Vaca",
                        principalColumn: "VacaID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Distribucion_EmpleadoID",
                table: "Distribucion",
                column: "EmpleadoID");

            migrationBuilder.CreateIndex(
                name: "IX_Produccion_EmpleadoID",
                table: "Produccion",
                column: "EmpleadoID");

            migrationBuilder.CreateIndex(
                name: "IX_Produccion_VacaID",
                table: "Produccion",
                column: "VacaID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Distribucion");

            migrationBuilder.DropTable(
                name: "Produccion");

            migrationBuilder.DropTable(
                name: "Empleado");

            migrationBuilder.DropTable(
                name: "Vaca");
        }
    }
}
