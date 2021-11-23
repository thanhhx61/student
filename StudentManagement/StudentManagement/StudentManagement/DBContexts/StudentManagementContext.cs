using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using StudentManagement.Entities;

#nullable disable

namespace StudentManagement.DBContexts
{
    public partial class StudentManagementContext : IdentityDbContext<User>
    {
        public StudentManagementContext()
        {
        }

        public StudentManagementContext(DbContextOptions<StudentManagementContext> options)
            : base(options)
        {
        }

        public virtual DbSet<BellNotification> BellNotifications { get; set; }
        public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<ListEvent> ListEvents { get; set; }
        public virtual DbSet<Message> Messages { get; set; }
        public virtual DbSet<SchoolYear> SchoolYears { get; set; }
        public virtual DbSet<Skill> Skills { get; set; }
        public virtual DbSet<UserSchoolYear> UserSchoolYears { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data Source=THANH_HO;Initial Catalog=StudentManagement;Integrated Security=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var tableName = entityType.GetTableName();
                if (tableName.StartsWith("AspNet"))
                {
                    entityType.SetTableName(tableName.Substring(6));
                }
            }
            modelBuilder.HasAnnotation("Relational:Collation", "Japanese_CI_AS");

            modelBuilder.Entity<BellNotification>(entity =>
            {
                entity.ToTable("BellNotification");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.HasOne(d => d.Notifi)
                    .WithMany(p => p.BellNotifications)
                    .HasForeignKey(d => d.NotifiId)
                    .HasConstraintName("FK_BellNotification_Messages");
            });

            modelBuilder.Entity<Event>(entity =>
            {
                entity.Property(e => e.Act).HasMaxLength(250);

                entity.Property(e => e.Activities).HasMaxLength(250);

                entity.Property(e => e.PowerDev).HasMaxLength(250);

                entity.Property(e => e.PowerExerted).HasMaxLength(250);

                entity.Property(e => e.SchoolYearId).HasMaxLength(250);

                entity.Property(e => e.Think)
                    .HasMaxLength(10)
                    .IsFixedLength(true);

                entity.HasOne(d => d.ListEvent)
                    .WithMany(p => p.Events)
                    .HasForeignKey(d => d.ListEventId)
                    .HasConstraintName("FK_Events_ListEvents");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Events)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_Events_Users");
            });

            modelBuilder.Entity<ListEvent>(entity =>
            {
                entity.Property(e => e.Icon).HasMaxLength(250);

                entity.Property(e => e.ListEventName).HasMaxLength(250);
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.HasKey(e => e.MessagesId);

                entity.Property(e => e.Content).HasMaxLength(500);

                entity.Property(e => e.Timestamp).HasColumnType("date");

                entity.HasOne(d => d.Event)
                    .WithMany(p => p.Messages)
                    .HasForeignKey(d => d.EventId)
                    .HasConstraintName("FK_Messages_Events");

                entity.HasOne(d => d.Skill)
                    .WithMany(p => p.Messages)
                    .HasForeignKey(d => d.SkillId)
                    .HasConstraintName("FK_Messages_Skills");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Messages)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_Messages_Users");
            });

            modelBuilder.Entity<SchoolYear>(entity =>
            {
                entity.Property(e => e.SchoolYearName).HasMaxLength(250);
            });

            modelBuilder.Entity<Skill>(entity =>
            {
                entity.Property(e => e.SkillName).HasMaxLength(250);

                entity.Property(e => e.Style).HasMaxLength(250);

                entity.Property(e => e.Tags).HasMaxLength(50);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Avatar).HasMaxLength(250);

                entity.Property(e => e.BellNotifiId).HasColumnName("bellNotifiId");

                entity.Property(e => e.CreateDate).HasColumnType("date");

                entity.Property(e => e.Email).HasMaxLength(250);

                entity.Property(e => e.LastLogin).HasColumnType("date");

                entity.Property(e => e.Phone).HasMaxLength(250);

                entity.Property(e => e.UserName).HasMaxLength(250);

                entity.HasOne(d => d.BellNotifi)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.BellNotifiId)
                    .HasConstraintName("FK_Users_BellNotification");
            });

            modelBuilder.Entity<UserSchoolYear>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.SchoolYearId });
                entity.Property(e => e.UserId).HasColumnName("UserId");
                entity.Property(e => e.SchoolYearId).HasColumnName("SchoolYearId");
                entity.HasOne(d => d.SchoolYear)
                    .WithMany(p => p.UserSchoolYears)
                    .HasForeignKey(d => d.SchoolYearId)
                    .HasConstraintName("FK_UserSchoolYears_SchoolYears");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserSchoolYears)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_UserSchoolYears_Users");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
