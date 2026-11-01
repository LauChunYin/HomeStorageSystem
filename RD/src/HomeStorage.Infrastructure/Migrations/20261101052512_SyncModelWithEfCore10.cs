using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeStorage.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SyncModelWithEfCore10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_items_location_location_id",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "fk_role_categories_role_role_id",
                table: "RoleCategories");

            migrationBuilder.AddForeignKey(
                name: "fk_items_locations_location_id",
                table: "Items",
                column: "location_id",
                principalTable: "Locations",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_role_categories_roles_role_id",
                table: "RoleCategories",
                column: "role_id",
                principalTable: "Roles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_items_locations_location_id",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "fk_role_categories_roles_role_id",
                table: "RoleCategories");

            migrationBuilder.AddForeignKey(
                name: "fk_items_location_location_id",
                table: "Items",
                column: "location_id",
                principalTable: "Locations",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_role_categories_role_role_id",
                table: "RoleCategories",
                column: "role_id",
                principalTable: "Roles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
