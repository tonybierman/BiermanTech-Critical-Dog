using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiermanTech.CriticalDog.Migrations
{
    /// <inheritdoc />
    public partial class FixSubjectUserIdForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subject_IdentityUser_UserId",
                table: "Subject");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Subject",
                type: "varchar(450)",
                maxLength: 450,
                nullable: false,
                collation: "utf8mb4_uca1400_ai_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_uca1400_ai_ci");

            migrationBuilder.AddForeignKey(
                name: "FK_Subject_AspNetUsers_UserId",
                table: "Subject",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subject_AspNetUsers_UserId",
                table: "Subject");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Subject",
                type: "varchar(255)",
                nullable: false,
                collation: "utf8mb4_uca1400_ai_ci",
                oldClrType: typeof(string),
                oldType: "varchar(450)",
                oldMaxLength: 450)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_uca1400_ai_ci");

            migrationBuilder.AddForeignKey(
                name: "FK_Subject_IdentityUser_UserId",
                table: "Subject",
                column: "UserId",
                principalTable: "IdentityUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
