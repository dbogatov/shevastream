using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Shevastream.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasDefaultSchema("shevastream")
                .HasAnnotation("ProductVersion", "1.0.0-rc2-20896");

            modelBuilder.Entity("Shevastream.Models.Enitites.BlogPost", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Active");

                    b.Property<int>("AuthorId");

                    b.Property<string>("Content");

                    b.Property<DateTime>("DatePosted");

                    b.Property<DateTime>("DateUpdated");

                    b.Property<string>("Preview");

                    b.Property<string>("Title");

                    b.Property<string>("TitleUrl");

                    b.Property<int>("Views");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.ToTable("BlogPost");
                });

            modelBuilder.Entity("Shevastream.Models.Enitites.Customer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Email");

                    b.Property<string>("Name");

                    b.Property<string>("Phone");

                    b.HasKey("Id");

                    b.ToTable("Customer");
                });

            modelBuilder.Entity("Shevastream.Models.Enitites.Feedback", b =>
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

                    b.ToTable("SSFeedback");
                });

            modelBuilder.Entity("Shevastream.Models.Enitites.LogEntry", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("TargetId");

                    b.Property<long>("Timestamp");

                    b.Property<int>("Type");

                    b.HasKey("Id");

                    b.ToTable("LogEntry");
                });

            modelBuilder.Entity("Shevastream.Models.Enitites.Order", b =>
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

                    b.HasIndex("AssigneeId");

                    b.HasIndex("CustomerId");

                    b.HasIndex("OrderStatusId");

                    b.HasIndex("PaymentMethodId");

                    b.HasIndex("ProductId");

                    b.HasIndex("ShipmentMethodId");

                    b.ToTable("Order");
                });

            modelBuilder.Entity("Shevastream.Models.Enitites.OrderProduct", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("OrderId");

                    b.Property<int>("ProductId");

                    b.Property<int>("Quantity");

                    b.HasKey("Id");

                    b.HasIndex("OrderId");

                    b.HasIndex("ProductId");

                    b.ToTable("OrderProduct");
                });

            modelBuilder.Entity("Shevastream.Models.Enitites.OrderStatus", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description");

                    b.HasKey("Id");

                    b.ToTable("OrderStatus");
                });

            modelBuilder.Entity("Shevastream.Models.Enitites.PaymentMethod", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("PaymentMethod");
                });

            modelBuilder.Entity("Shevastream.Models.Enitites.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Characteristics");

                    b.Property<int>("Cost");

                    b.Property<string>("Description");

                    b.Property<string>("ImageUrls");

                    b.Property<string>("Information");

                    b.Property<string>("Name");

                    b.Property<string>("VideoData");

                    b.HasKey("Id");

                    b.ToTable("Product");
                });

            modelBuilder.Entity("Shevastream.Models.Enitites.PushPair", b =>
                {
                    b.Property<string>("DeviceToken");

                    b.Property<int>("UserId");

                    b.HasKey("DeviceToken");

                    b.HasIndex("UserId");

                    b.ToTable("PushPair");
                });

            modelBuilder.Entity("Shevastream.Models.Enitites.ShipmentMethod", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Cost");

                    b.Property<bool>("CostTBD");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("ShipmentMethod");
                });

            modelBuilder.Entity("Shevastream.Models.Enitites.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("FullName");

                    b.Property<string>("ImageUrl");

                    b.Property<string>("NickName");

                    b.Property<string>("Occupation");

                    b.Property<string>("PassHash");

                    b.Property<string>("Position");

                    b.HasKey("Id");

                    b.ToTable("User");
                });

            modelBuilder.Entity("Shevastream.Models.Enitites.BlogPost", b =>
                {
                    b.HasOne("Shevastream.Models.Enitites.User")
                        .WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Shevastream.Models.Enitites.Order", b =>
                {
                    b.HasOne("Shevastream.Models.Enitites.User")
                        .WithMany()
                        .HasForeignKey("AssigneeId");

                    b.HasOne("Shevastream.Models.Enitites.Customer")
                        .WithMany()
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Shevastream.Models.Enitites.OrderStatus")
                        .WithMany()
                        .HasForeignKey("OrderStatusId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Shevastream.Models.Enitites.PaymentMethod")
                        .WithMany()
                        .HasForeignKey("PaymentMethodId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Shevastream.Models.Enitites.Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Shevastream.Models.Enitites.ShipmentMethod")
                        .WithMany()
                        .HasForeignKey("ShipmentMethodId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Shevastream.Models.Enitites.OrderProduct", b =>
                {
                    b.HasOne("Shevastream.Models.Enitites.Order")
                        .WithMany()
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Shevastream.Models.Enitites.Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Shevastream.Models.Enitites.PushPair", b =>
                {
                    b.HasOne("Shevastream.Models.Enitites.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
