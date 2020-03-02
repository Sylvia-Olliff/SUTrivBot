using Microsoft.EntityFrameworkCore.Migrations;

namespace SUTrivBot.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Guilds",
                columns: table => new
                {
                    GuildId = table.Column<string>(nullable: false),
                    Disabled = table.Column<bool>(nullable: false),
                    RestrictTrivMaster = table.Column<bool>(nullable: false),
                    LockedChannelsBlobbed = table.Column<string>(nullable: true),
                    AdminRoleName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guilds", x => x.GuildId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Guilds");
        }
    }
}
