using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Infrastructure.Persistence.Migrations.FilmFreakDb
{
    /// <inheritdoc />
    public partial class AddExternalIdToCollectionItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExternalId",
                table: "CollectionItems",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CollectionItems_ReleaseId",
                table: "CollectionItems",
                column: "ReleaseId");

            migrationBuilder.AddForeignKey(
                name: "FK_CollectionItems_Releases_ReleaseId",
                table: "CollectionItems",
                column: "ReleaseId",
                principalTable: "Releases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CollectionItems_Releases_ReleaseId",
                table: "CollectionItems");

            migrationBuilder.DropIndex(
                name: "IX_CollectionItems_ReleaseId",
                table: "CollectionItems");

            migrationBuilder.DropColumn(
                name: "ExternalId",
                table: "CollectionItems");
        }
    }
}
