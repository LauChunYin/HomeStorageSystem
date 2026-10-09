using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeStorage.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddItemStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_category_category_parent_id",
                table: "category");

            migrationBuilder.DropForeignKey(
                name: "fk_items_category_category_id",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "fk_items_category_category_id1",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "fk_items_location_location_id",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "fk_items_location_location_id1",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "ix_items_category_id1",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "ix_items_location_id1",
                table: "Items");

            migrationBuilder.DropPrimaryKey(
                name: "pk_location",
                table: "location");

            migrationBuilder.DropPrimaryKey(
                name: "pk_category",
                table: "category");

            migrationBuilder.DropColumn(
                name: "category_id1",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "location_id1",
                table: "Items");

            migrationBuilder.RenameTable(
                name: "location",
                newName: "Locations");

            migrationBuilder.RenameTable(
                name: "category",
                newName: "Categories");

            migrationBuilder.RenameIndex(
                name: "ix_category_parent_id",
                table: "Categories",
                newName: "ix_categories_parent_id");

            migrationBuilder.AddColumn<string>(
                name: "status",
                table: "Items",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "Locations",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "image_url",
                table: "Locations",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "parent_id",
                table: "Locations",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "Categories",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddPrimaryKey(
                name: "pk_locations",
                table: "Locations",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_categories",
                table: "Categories",
                column: "id");

            migrationBuilder.CreateTable(
                name: "RoleCategories",
                columns: table => new
                {
                    role_id = table.Column<int>(type: "integer", nullable: false),
                    category_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_role_categories", x => new { x.role_id, x.category_id });
                    table.ForeignKey(
                        name: "fk_role_categories_categories_category_id",
                        column: x => x.category_id,
                        principalTable: "Categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_role_categories_role_role_id",
                        column: x => x.role_id,
                        principalTable: "Roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoleLocations",
                columns: table => new
                {
                    role_id = table.Column<int>(type: "integer", nullable: false),
                    location_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_role_locations", x => new { x.role_id, x.location_id });
                    table.ForeignKey(
                        name: "fk_role_locations_locations_location_id",
                        column: x => x.location_id,
                        principalTable: "Locations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_role_locations_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "Roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_roles_role_code",
                table: "Roles",
                column: "role_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_permissions_permission_code",
                table: "Permissions",
                column: "permission_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_locations_name",
                table: "Locations",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "ix_locations_parent_id",
                table: "Locations",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "ix_categories_name",
                table: "Categories",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "ix_role_categories_category_id",
                table: "RoleCategories",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "ix_role_locations_location_id",
                table: "RoleLocations",
                column: "location_id");

            migrationBuilder.AddForeignKey(
                name: "fk_categories_categories_parent_id",
                table: "Categories",
                column: "parent_id",
                principalTable: "Categories",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_items_categories_category_id",
                table: "Items",
                column: "category_id",
                principalTable: "Categories",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_items_location_location_id",
                table: "Items",
                column: "location_id",
                principalTable: "Locations",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_locations_locations_parent_id",
                table: "Locations",
                column: "parent_id",
                principalTable: "Locations",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_categories_categories_parent_id",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "fk_items_categories_category_id",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "fk_items_location_location_id",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "fk_locations_locations_parent_id",
                table: "Locations");

            migrationBuilder.DropTable(
                name: "RoleCategories");

            migrationBuilder.DropTable(
                name: "RoleLocations");

            migrationBuilder.DropIndex(
                name: "ix_roles_role_code",
                table: "Roles");

            migrationBuilder.DropIndex(
                name: "ix_permissions_permission_code",
                table: "Permissions");

            migrationBuilder.DropPrimaryKey(
                name: "pk_locations",
                table: "Locations");

            migrationBuilder.DropIndex(
                name: "ix_locations_name",
                table: "Locations");

            migrationBuilder.DropIndex(
                name: "ix_locations_parent_id",
                table: "Locations");

            migrationBuilder.DropPrimaryKey(
                name: "pk_categories",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "ix_categories_name",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "status",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "parent_id",
                table: "Locations");

            migrationBuilder.RenameTable(
                name: "Locations",
                newName: "location");

            migrationBuilder.RenameTable(
                name: "Categories",
                newName: "category");

            migrationBuilder.RenameIndex(
                name: "ix_categories_parent_id",
                table: "category",
                newName: "ix_category_parent_id");

            migrationBuilder.AddColumn<int>(
                name: "category_id1",
                table: "Items",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "location_id1",
                table: "Items",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "location",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "image_url",
                table: "location",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "category",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AddPrimaryKey(
                name: "pk_location",
                table: "location",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_category",
                table: "category",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "ix_items_category_id1",
                table: "Items",
                column: "category_id1");

            migrationBuilder.CreateIndex(
                name: "ix_items_location_id1",
                table: "Items",
                column: "location_id1");

            migrationBuilder.AddForeignKey(
                name: "fk_category_category_parent_id",
                table: "category",
                column: "parent_id",
                principalTable: "category",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_items_category_category_id",
                table: "Items",
                column: "category_id",
                principalTable: "category",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_items_category_category_id1",
                table: "Items",
                column: "category_id1",
                principalTable: "category",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_items_location_location_id",
                table: "Items",
                column: "location_id",
                principalTable: "location",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_items_location_location_id1",
                table: "Items",
                column: "location_id1",
                principalTable: "location",
                principalColumn: "id");
        }
    }
}
