﻿// <auto-generated />
using System;
using MiFloraGateway.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MiFloraGateway.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20200108200452_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("MiFloraGateway.Database.Device", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("IPAddress")
                        .IsRequired()
                        .HasMaxLength(45);

                    b.Property<string>("MACAddress")
                        .IsRequired()
                        .IsFixedLength(true)
                        .HasMaxLength(17);

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.HasIndex("MACAddress")
                        .IsUnique();

                    b.ToTable("Devices");
                });

            modelBuilder.Entity("MiFloraGateway.Database.DeviceSensorDistance", b =>
                {
                    b.Property<int>("DeviceId");

                    b.Property<int>("SensorId");

                    b.Property<DateTime>("When");

                    b.Property<int>("Rssi");

                    b.HasKey("DeviceId", "SensorId", "When");

                    b.HasIndex("SensorId");

                    b.ToTable("DeviceSensorDistances");
                });

            modelBuilder.Entity("MiFloraGateway.Database.DeviceTag", b =>
                {
                    b.Property<int>("DeviceId");

                    b.Property<string>("Tag")
                        .HasMaxLength(32);

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasMaxLength(32);

                    b.HasKey("DeviceId", "Tag");

                    b.ToTable("DevicesTags");
                });

            modelBuilder.Entity("MiFloraGateway.Database.LogEntry", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("DeviceId");

                    b.Property<TimeSpan>("Duration");

                    b.Property<string>("Event")
                        .IsRequired()
                        .HasMaxLength(21);

                    b.Property<string>("Message")
                        .HasMaxLength(200);

                    b.Property<string>("Result")
                        .IsRequired()
                        .HasMaxLength(10);

                    b.Property<int?>("SensorId");

                    b.Property<DateTime>("When");

                    b.HasKey("Id");

                    b.HasIndex("DeviceId");

                    b.HasIndex("SensorId");

                    b.ToTable("LogEntries");
                });

            modelBuilder.Entity("MiFloraGateway.Database.Plant", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Alias");

                    b.Property<string>("Display");

                    b.Property<string>("ImageUrl");

                    b.Property<string>("LatinName");

                    b.HasKey("Id");

                    b.HasIndex("LatinName")
                        .IsUnique()
                        .HasFilter("[LatinName] IS NOT NULL");

                    b.ToTable("Plants");
                });

            modelBuilder.Entity("MiFloraGateway.Database.PlantBasic", b =>
                {
                    b.Property<int>("PlantId");

                    b.Property<string>("Blooming");

                    b.Property<string>("Category");

                    b.Property<string>("Color");

                    b.Property<string>("FloralLanguage");

                    b.Property<string>("Origin");

                    b.Property<string>("Production");

                    b.HasKey("PlantId");

                    b.ToTable("PlantBasics");
                });

            modelBuilder.Entity("MiFloraGateway.Database.PlantMaintenance", b =>
                {
                    b.Property<int>("PlantId");

                    b.Property<string>("Fertilization");

                    b.Property<string>("Pruning");

                    b.Property<string>("Size");

                    b.Property<string>("Soil");

                    b.Property<string>("Sunlight");

                    b.Property<string>("Watering");

                    b.HasKey("PlantId");

                    b.ToTable("PlantMaintenance");
                });

            modelBuilder.Entity("MiFloraGateway.Database.PlantParameters", b =>
                {
                    b.Property<int>("PlantId");

                    b.HasKey("PlantId");

                    b.ToTable("PlantParameters");
                });

            modelBuilder.Entity("MiFloraGateway.Database.Sensor", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("MACAddress")
                        .IsRequired()
                        .IsFixedLength(true)
                        .HasMaxLength(17);

                    b.Property<string>("Name");

                    b.Property<int?>("PlantId");

                    b.HasKey("Id");

                    b.HasIndex("MACAddress");

                    b.HasIndex("PlantId");

                    b.ToTable("Sensors");
                });

            modelBuilder.Entity("MiFloraGateway.Database.SensorBatteryAndVersionReading", b =>
                {
                    b.Property<int>("SensorId");

                    b.Property<DateTime>("When");

                    b.Property<int>("Battery");

                    b.Property<string>("Version")
                        .IsRequired()
                        .HasMaxLength(12);

                    b.HasKey("SensorId", "When");

                    b.ToTable("SensorBatteryReadings");
                });

            modelBuilder.Entity("MiFloraGateway.Database.SensorDataReading", b =>
                {
                    b.Property<int>("SensorId");

                    b.Property<DateTime>("When");

                    b.Property<int>("Brightness");

                    b.Property<int>("Conductivity");

                    b.Property<int>("Moisture");

                    b.Property<float>("Temperature");

                    b.HasKey("SensorId", "When");

                    b.ToTable("SensorDataReadings");
                });

            modelBuilder.Entity("MiFloraGateway.Database.SensorTag", b =>
                {
                    b.Property<int>("SensorId");

                    b.Property<string>("Tag")
                        .HasMaxLength(32);

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasMaxLength(32);

                    b.HasKey("SensorId", "Tag");

                    b.ToTable("SensorTags");
                });

            modelBuilder.Entity("MiFloraGateway.Database.Setting", b =>
                {
                    b.Property<int>("Key");

                    b.Property<DateTime?>("LastChanged");

                    b.Property<string>("Value");

                    b.HasKey("Key");

                    b.ToTable("Settings");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("MiFloraGateway.Database.DeviceSensorDistance", b =>
                {
                    b.HasOne("MiFloraGateway.Database.Device", "Device")
                        .WithMany("SensorDistances")
                        .HasForeignKey("DeviceId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MiFloraGateway.Database.Sensor", "Sensor")
                        .WithMany("DeviceDistances")
                        .HasForeignKey("SensorId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MiFloraGateway.Database.DeviceTag", b =>
                {
                    b.HasOne("MiFloraGateway.Database.Device", "Device")
                        .WithMany("Tags")
                        .HasForeignKey("DeviceId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MiFloraGateway.Database.LogEntry", b =>
                {
                    b.HasOne("MiFloraGateway.Database.Device", "Device")
                        .WithMany("Logs")
                        .HasForeignKey("DeviceId");

                    b.HasOne("MiFloraGateway.Database.Sensor", "Sensor")
                        .WithMany("Logs")
                        .HasForeignKey("SensorId");
                });

            modelBuilder.Entity("MiFloraGateway.Database.PlantBasic", b =>
                {
                    b.HasOne("MiFloraGateway.Database.Plant", "Plant")
                        .WithOne("Basic")
                        .HasForeignKey("MiFloraGateway.Database.PlantBasic", "PlantId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MiFloraGateway.Database.PlantMaintenance", b =>
                {
                    b.HasOne("MiFloraGateway.Database.Plant", "Plant")
                        .WithOne("Maintenance")
                        .HasForeignKey("MiFloraGateway.Database.PlantMaintenance", "PlantId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MiFloraGateway.Database.PlantParameters", b =>
                {
                    b.HasOne("MiFloraGateway.Database.Plant", "Plant")
                        .WithOne("Parameters")
                        .HasForeignKey("MiFloraGateway.Database.PlantParameters", "PlantId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.OwnsOne("MiFloraGateway.Database.Range", "EnvironmentHumidity", b1 =>
                        {
                            b1.Property<int>("PlantParametersPlantId");

                            b1.Property<int>("Max");

                            b1.Property<int>("Min");

                            b1.HasKey("PlantParametersPlantId");

                            b1.ToTable("PlantParameters");

                            b1.HasOne("MiFloraGateway.Database.PlantParameters")
                                .WithOne("EnvironmentHumidity")
                                .HasForeignKey("MiFloraGateway.Database.Range", "PlantParametersPlantId")
                                .OnDelete(DeleteBehavior.Cascade);
                        });

                    b.OwnsOne("MiFloraGateway.Database.Range", "LightLux", b1 =>
                        {
                            b1.Property<int>("PlantParametersPlantId");

                            b1.Property<int>("Max");

                            b1.Property<int>("Min");

                            b1.HasKey("PlantParametersPlantId");

                            b1.ToTable("PlantParameters");

                            b1.HasOne("MiFloraGateway.Database.PlantParameters")
                                .WithOne("LightLux")
                                .HasForeignKey("MiFloraGateway.Database.Range", "PlantParametersPlantId")
                                .OnDelete(DeleteBehavior.Cascade);
                        });

                    b.OwnsOne("MiFloraGateway.Database.Range", "LightMmol", b1 =>
                        {
                            b1.Property<int>("PlantParametersPlantId");

                            b1.Property<int>("Max");

                            b1.Property<int>("Min");

                            b1.HasKey("PlantParametersPlantId");

                            b1.ToTable("PlantParameters");

                            b1.HasOne("MiFloraGateway.Database.PlantParameters")
                                .WithOne("LightMmol")
                                .HasForeignKey("MiFloraGateway.Database.Range", "PlantParametersPlantId")
                                .OnDelete(DeleteBehavior.Cascade);
                        });

                    b.OwnsOne("MiFloraGateway.Database.Range", "SoilFertility", b1 =>
                        {
                            b1.Property<int>("PlantParametersPlantId");

                            b1.Property<int>("Max");

                            b1.Property<int>("Min");

                            b1.HasKey("PlantParametersPlantId");

                            b1.ToTable("PlantParameters");

                            b1.HasOne("MiFloraGateway.Database.PlantParameters")
                                .WithOne("SoilFertility")
                                .HasForeignKey("MiFloraGateway.Database.Range", "PlantParametersPlantId")
                                .OnDelete(DeleteBehavior.Cascade);
                        });

                    b.OwnsOne("MiFloraGateway.Database.Range", "SoilHumidity", b1 =>
                        {
                            b1.Property<int>("PlantParametersPlantId");

                            b1.Property<int>("Max");

                            b1.Property<int>("Min");

                            b1.HasKey("PlantParametersPlantId");

                            b1.ToTable("PlantParameters");

                            b1.HasOne("MiFloraGateway.Database.PlantParameters")
                                .WithOne("SoilHumidity")
                                .HasForeignKey("MiFloraGateway.Database.Range", "PlantParametersPlantId")
                                .OnDelete(DeleteBehavior.Cascade);
                        });

                    b.OwnsOne("MiFloraGateway.Database.Range", "Temperature", b1 =>
                        {
                            b1.Property<int>("PlantParametersPlantId");

                            b1.Property<int>("Max");

                            b1.Property<int>("Min");

                            b1.HasKey("PlantParametersPlantId");

                            b1.ToTable("PlantParameters");

                            b1.HasOne("MiFloraGateway.Database.PlantParameters")
                                .WithOne("Temperature")
                                .HasForeignKey("MiFloraGateway.Database.Range", "PlantParametersPlantId")
                                .OnDelete(DeleteBehavior.Cascade);
                        });
                });

            modelBuilder.Entity("MiFloraGateway.Database.Sensor", b =>
                {
                    b.HasOne("MiFloraGateway.Database.Plant", "Plant")
                        .WithMany()
                        .HasForeignKey("PlantId");
                });

            modelBuilder.Entity("MiFloraGateway.Database.SensorBatteryAndVersionReading", b =>
                {
                    b.HasOne("MiFloraGateway.Database.Sensor", "Sensor")
                        .WithMany("BatteryAndVersionReadings")
                        .HasForeignKey("SensorId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MiFloraGateway.Database.SensorDataReading", b =>
                {
                    b.HasOne("MiFloraGateway.Database.Sensor", "Sensor")
                        .WithMany("DataReadings")
                        .HasForeignKey("SensorId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MiFloraGateway.Database.SensorTag", b =>
                {
                    b.HasOne("MiFloraGateway.Database.Sensor", "Sensor")
                        .WithMany("Tags")
                        .HasForeignKey("SensorId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
