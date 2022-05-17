using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProxyServer.Data.Migrations
{
    public partial class AddedServerKeyPairModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ServerKeyPair",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PrivatePublicKeyPair = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PublicKeyOnly = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServerKeyPair", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServerKeyPair");
        }
    }
}
