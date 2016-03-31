using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations;

namespace EShop.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20160331125629_Init")]
    partial class Init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0-rc1-16348")
                .HasAnnotation("Relational:DefaultSchema", "shevastream");

            modelBuilder.Entity("EShop.Models.Enitites.Customer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Email");

                    b.Property<string>("Name");

                    b.Property<string>("Phone");

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:TableName", "Customer");
                });

            modelBuilder.Entity("EShop.Models.Enitites.Feedback", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Body");

                    b.Property<string>("Email");

                    b.Property<string>("Name");

                    b.Property<string>("Subject");

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:TableName", "SSFeedback");
                });

            modelBuilder.Entity("EShop.Models.Enitites.LogEntry", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("TargetId");

                    b.Property<long>("Timestamp");

                    b.Property<int>("Type");

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:TableName", "LogEntry");
                });

            modelBuilder.Entity("EShop.Models.Enitites.Order", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Address");

                    b.Property<string>("AssigneeComment");

                    b.Property<int?>("AssigneeId");

                    b.Property<string>("Comment");

                    b.Property<int>("CustomerId");

                    b.Property<long>("DateCreated");

                    b.Property<long>("DateLastModified");

                    b.Property<int>("OrderStatusId");

                    b.Property<int>("PaymentMethodId");

                    b.Property<int>("ProductId");

                    b.Property<int>("Quantity");

                    b.Property<int>("ShipmentMethodId");

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:TableName", "Order");
                });

            modelBuilder.Entity("EShop.Models.Enitites.OrderStatus", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description");

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:TableName", "OrderStatus");
                });

            modelBuilder.Entity("EShop.Models.Enitites.PaymentMethod", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:TableName", "PaymentMethod");
                });

            modelBuilder.Entity("EShop.Models.Enitites.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Characteristics");

                    b.Property<int>("Cost");

                    b.Property<string>("Description");

                    b.Property<string>("ImageUrls");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:TableName", "Product");
                });

            modelBuilder.Entity("EShop.Models.Enitites.ShipmentMethod", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Cost");

                    b.Property<bool>("CostTBD");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:TableName", "ShipmentMethod");
                });

            modelBuilder.Entity("EShop.Models.Enitites.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("FullName");

                    b.Property<string>("NickName");

                    b.Property<string>("PassHash");

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:TableName", "User");
                });

            modelBuilder.Entity("EShop.Models.Enitites.Order", b =>
                {
                    b.HasOne("EShop.Models.Enitites.User")
                        .WithMany()
                        .HasForeignKey("AssigneeId");

                    b.HasOne("EShop.Models.Enitites.Customer")
                        .WithMany()
                        .HasForeignKey("CustomerId");

                    b.HasOne("EShop.Models.Enitites.OrderStatus")
                        .WithMany()
                        .HasForeignKey("OrderStatusId");

                    b.HasOne("EShop.Models.Enitites.PaymentMethod")
                        .WithMany()
                        .HasForeignKey("PaymentMethodId");

                    b.HasOne("EShop.Models.Enitites.Product")
                        .WithMany()
                        .HasForeignKey("ProductId");

                    b.HasOne("EShop.Models.Enitites.ShipmentMethod")
                        .WithMany()
                        .HasForeignKey("ShipmentMethodId");
                });
        }
    }
}
