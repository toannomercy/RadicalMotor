using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RadicalMotor.Migrations
{
    /// <inheritdoc />
    public partial class AddVehicleImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VehicleImages",
                columns: table => new
                {
                    ImageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChassisNumber = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleImages", x => x.ImageId);
                    table.ForeignKey(
                        name: "FK_VehicleImages_Vehicles_ChassisNumber",
                        column: x => x.ChassisNumber,
                        principalTable: "Vehicles",
                        principalColumn: "ChassisNumber",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VehicleImages_ChassisNumber",
                table: "VehicleImages",
                column: "ChassisNumber");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VehicleImages");
        }
    }
}
