using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HARD.CORE.DAT.Migrations
{
    /// <inheritdoc />
    public partial class AddUsuarioPerfilRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Empresas_Usuarios_UsuarioId",
                table: "Empresas");

            migrationBuilder.DropForeignKey(
                name: "FK_Menus_Perfiles_PerfilId",
                table: "Menus");

            migrationBuilder.DropForeignKey(
                name: "FK_Perfiles_Usuarios_UsuarioId",
                table: "Perfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Empresas_EmpresaActivoId",
                table: "Usuarios");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Perfiles_PerfilActivoId",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_EmpresaActivoId",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_PerfilActivoId",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Perfiles_UsuarioId",
                table: "Perfiles");

            migrationBuilder.DropIndex(
                name: "IX_Menus_PerfilId",
                table: "Menus");

            migrationBuilder.DropIndex(
                name: "IX_Empresas_UsuarioId",
                table: "Empresas");

            migrationBuilder.DropColumn(
                name: "EmpresaActivoId",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "PerfilActivoId",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Perfiles");

            migrationBuilder.DropColumn(
                name: "PerfilId",
                table: "Menus");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Empresas");

            migrationBuilder.CreateTable(
                name: "EmpresaUsuario",
                columns: table => new
                {
                    EmpresasId = table.Column<int>(type: "int", nullable: false),
                    UsuariosId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpresaUsuario", x => new { x.EmpresasId, x.UsuariosId });
                    table.ForeignKey(
                        name: "FK_EmpresaUsuario_Empresas_EmpresasId",
                        column: x => x.EmpresasId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmpresaUsuario_Usuarios_UsuariosId",
                        column: x => x.UsuariosId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MenuPerfil",
                columns: table => new
                {
                    MenusId = table.Column<int>(type: "int", nullable: false),
                    PerfilesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuPerfil", x => new { x.MenusId, x.PerfilesId });
                    table.ForeignKey(
                        name: "FK_MenuPerfil_Menus_MenusId",
                        column: x => x.MenusId,
                        principalTable: "Menus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MenuPerfil_Perfiles_PerfilesId",
                        column: x => x.PerfilesId,
                        principalTable: "Perfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PerfilUsuario",
                columns: table => new
                {
                    PerfilesId = table.Column<int>(type: "int", nullable: false),
                    UsuariosId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerfilUsuario", x => new { x.PerfilesId, x.UsuariosId });
                    table.ForeignKey(
                        name: "FK_PerfilUsuario_Perfiles_PerfilesId",
                        column: x => x.PerfilesId,
                        principalTable: "Perfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PerfilUsuario_Usuarios_UsuariosId",
                        column: x => x.UsuariosId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmpresaUsuario_UsuariosId",
                table: "EmpresaUsuario",
                column: "UsuariosId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuPerfil_PerfilesId",
                table: "MenuPerfil",
                column: "PerfilesId");

            migrationBuilder.CreateIndex(
                name: "IX_PerfilUsuario_UsuariosId",
                table: "PerfilUsuario",
                column: "UsuariosId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmpresaUsuario");

            migrationBuilder.DropTable(
                name: "MenuPerfil");

            migrationBuilder.DropTable(
                name: "PerfilUsuario");

            migrationBuilder.AddColumn<int>(
                name: "EmpresaActivoId",
                table: "Usuarios",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PerfilActivoId",
                table: "Usuarios",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioId",
                table: "Perfiles",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PerfilId",
                table: "Menus",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioId",
                table: "Empresas",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_EmpresaActivoId",
                table: "Usuarios",
                column: "EmpresaActivoId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_PerfilActivoId",
                table: "Usuarios",
                column: "PerfilActivoId");

            migrationBuilder.CreateIndex(
                name: "IX_Perfiles_UsuarioId",
                table: "Perfiles",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Menus_PerfilId",
                table: "Menus",
                column: "PerfilId");

            migrationBuilder.CreateIndex(
                name: "IX_Empresas_UsuarioId",
                table: "Empresas",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Empresas_Usuarios_UsuarioId",
                table: "Empresas",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Menus_Perfiles_PerfilId",
                table: "Menus",
                column: "PerfilId",
                principalTable: "Perfiles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Perfiles_Usuarios_UsuarioId",
                table: "Perfiles",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Empresas_EmpresaActivoId",
                table: "Usuarios",
                column: "EmpresaActivoId",
                principalTable: "Empresas",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Perfiles_PerfilActivoId",
                table: "Usuarios",
                column: "PerfilActivoId",
                principalTable: "Perfiles",
                principalColumn: "Id");
        }
    }
}
